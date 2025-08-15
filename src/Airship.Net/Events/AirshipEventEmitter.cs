/* Copyright Airship and Contributors */

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AirshipDotNet.Events
{
    /// <summary>
    /// Central event management system with queuing support.
    /// Ensures events are never lost even when no listeners are attached.
    /// </summary>
    public class AirshipEventEmitter : IDisposable
    {
        private static readonly Lazy<AirshipEventEmitter> _instance = new(() => new AirshipEventEmitter());
        
        /// <summary>
        /// Gets the shared instance of the AirshipEventEmitter.
        /// </summary>
        public static AirshipEventEmitter Shared => _instance.Value;

        private readonly ConcurrentDictionary<AirshipEventType, ConcurrentQueue<AirshipEvent>> _pendingEvents;
        private readonly ConcurrentDictionary<AirshipEventType, List<EventHandler<EventArgs>>> _listeners;
        private readonly ReaderWriterLockSlim _listenerLock;
        private readonly int _maxQueueSize;

        /// <summary>
        /// Event fired when there are pending events for a type.
        /// </summary>
        public event EventHandler<AirshipEventType>? PendingEventAvailable;

        /// <summary>
        /// Initializes a new instance of the AirshipEventEmitter class.
        /// </summary>
        /// <param name="maxQueueSize">Maximum number of events to queue per type (default: 100).</param>
        private AirshipEventEmitter(int maxQueueSize = 100)
        {
            _pendingEvents = new ConcurrentDictionary<AirshipEventType, ConcurrentQueue<AirshipEvent>>();
            _listeners = new ConcurrentDictionary<AirshipEventType, List<EventHandler<EventArgs>>>();
            _listenerLock = new ReaderWriterLockSlim();
            _maxQueueSize = maxQueueSize;
        }

        /// <summary>
        /// Emits an event, either dispatching it to listeners or queuing it.
        /// </summary>
        /// <param name="eventType">The type of event to emit.</param>
        /// <param name="eventArgs">The event arguments.</param>
        public void Emit(AirshipEventType eventType, EventArgs eventArgs)
        {
            // AirshipEvent doesn't inherit from EventArgs, so we always wrap
            var airshipEvent = new AirshipEvent(eventType, eventArgs);
            
            bool hasListeners = false;
            _listenerLock.EnterReadLock();
            try
            {
                hasListeners = _listeners.TryGetValue(eventType, out var listeners) && listeners.Count > 0;
            }
            finally
            {
                _listenerLock.ExitReadLock();
            }

            if (hasListeners)
            {
                // Dispatch immediately
                DispatchEvent(eventType, airshipEvent);
            }
            else
            {
                // Queue the event
                QueueEvent(eventType, airshipEvent);
            }
        }

        /// <summary>
        /// Emits an event with raw data.
        /// </summary>
        /// <param name="eventType">The type of event to emit.</param>
        /// <param name="data">The raw event data.</param>
        public void Emit(AirshipEventType eventType, object? data = null)
        {
            var airshipEvent = new AirshipEvent(eventType, data);
            
            bool hasListeners = false;
            _listenerLock.EnterReadLock();
            try
            {
                hasListeners = _listeners.TryGetValue(eventType, out var listeners) && listeners.Count > 0;
            }
            finally
            {
                _listenerLock.ExitReadLock();
            }

            if (hasListeners)
            {
                DispatchEvent(eventType, airshipEvent);
            }
            else
            {
                QueueEvent(eventType, airshipEvent);
            }
        }

        /// <summary>
        /// Adds a listener for a specific event type.
        /// </summary>
        /// <param name="eventType">The type of event to listen for.</param>
        /// <param name="handler">The event handler.</param>
        public void AddListener(AirshipEventType eventType, EventHandler<EventArgs> handler)
        {
            _listenerLock.EnterWriteLock();
            try
            {
                if (!_listeners.TryGetValue(eventType, out var listeners))
                {
                    listeners = new List<EventHandler<EventArgs>>();
                    _listeners[eventType] = listeners;
                }
                listeners.Add(handler);
            }
            finally
            {
                _listenerLock.ExitWriteLock();
            }

            // Process any pending events for this type
            _ = ProcessPendingEvents(eventType);
        }

        /// <summary>
        /// Removes a listener for a specific event type.
        /// </summary>
        /// <param name="eventType">The type of event.</param>
        /// <param name="handler">The event handler to remove.</param>
        public void RemoveListener(AirshipEventType eventType, EventHandler<EventArgs> handler)
        {
            _listenerLock.EnterWriteLock();
            try
            {
                if (_listeners.TryGetValue(eventType, out var listeners))
                {
                    listeners.Remove(handler);
                    if (listeners.Count == 0)
                    {
                        _listeners.TryRemove(eventType, out _);
                    }
                }
            }
            finally
            {
                _listenerLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Processes all pending events for a specific event type.
        /// </summary>
        /// <param name="eventType">The type of events to process.</param>
        /// <param name="handler">Optional handler function that returns true if event was handled.</param>
        public async Task ProcessPendingEvents(AirshipEventType eventType, Func<AirshipEvent, bool>? handler = null)
        {
            if (!_pendingEvents.TryGetValue(eventType, out var queue))
            {
                return;
            }

            List<AirshipEvent> eventsToProcess = new();
            List<AirshipEvent> unhandledEvents = new();
            
            // Dequeue all pending events
            while (queue.TryDequeue(out var pendingEvent))
            {
                eventsToProcess.Add(pendingEvent);
            }

            // Process each event
            foreach (var pendingEvent in eventsToProcess)
            {
                bool handled = false;
                
                if (handler != null)
                {
                    // Use custom handler if provided - run synchronously to avoid threading issues
                    handled = handler(pendingEvent);
                }
                else
                {
                    // Use default dispatch - run synchronously
                    DispatchEvent(eventType, pendingEvent);
                    handled = HasListeners(eventType);
                }
                
                // Re-queue unhandled events
                if (!handled)
                {
                    unhandledEvents.Add(pendingEvent);
                }
            }
            
            // Re-queue unhandled events
            foreach (var unhandled in unhandledEvents)
            {
                queue.Enqueue(unhandled);
            }
            
            // Use Task.CompletedTask to satisfy async signature without unnecessary threading
            await Task.CompletedTask;
        }
        
        /// <summary>
        /// Checks if there are listeners for a specific event type.
        /// </summary>
        private bool HasListeners(AirshipEventType eventType)
        {
            _listenerLock.EnterReadLock();
            try
            {
                return _listeners.TryGetValue(eventType, out var listeners) && listeners.Count > 0;
            }
            finally
            {
                _listenerLock.ExitReadLock();
            }
        }

        /// <summary>
        /// Processes all pending events for all event types.
        /// </summary>
        public async Task ProcessAllPendingEvents()
        {
            var tasks = _pendingEvents.Keys.Select(eventType => ProcessPendingEvents(eventType));
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Clears all pending events for a specific event type.
        /// </summary>
        /// <param name="eventType">The type of events to clear.</param>
        public void ClearPendingEvents(AirshipEventType eventType)
        {
            if (_pendingEvents.TryGetValue(eventType, out var queue))
            {
                while (queue.TryDequeue(out _)) { }
            }
        }

        /// <summary>
        /// Clears all pending events.
        /// </summary>
        public void ClearAllPendingEvents()
        {
            foreach (var queue in _pendingEvents.Values)
            {
                while (queue.TryDequeue(out _)) { }
            }
        }

        /// <summary>
        /// Gets the count of pending events for a specific type.
        /// </summary>
        public int GetPendingEventCount(AirshipEventType eventType)
        {
            return _pendingEvents.TryGetValue(eventType, out var queue) ? queue.Count : 0;
        }
        
        /// <summary>
        /// Takes all pending events for the specified event types and returns them.
        /// This removes the events from the queue.
        /// </summary>
        /// <param name="eventTypes">The event types to take.</param>
        /// <returns>List of pending events.</returns>
        public List<AirshipEvent> TakePending(params AirshipEventType[] eventTypes)
        {
            var result = new List<AirshipEvent>();
            
            foreach (var eventType in eventTypes)
            {
                if (_pendingEvents.TryGetValue(eventType, out var queue))
                {
                    while (queue.TryDequeue(out var pendingEvent))
                    {
                        result.Add(pendingEvent);
                    }
                }
            }
            
            return result;
        }
        
        /// <summary>
        /// Gets all pending events without removing them from the queue.
        /// </summary>
        /// <param name="eventType">The event type.</param>
        /// <returns>List of pending events.</returns>
        public List<AirshipEvent> GetPending(AirshipEventType eventType)
        {
            if (_pendingEvents.TryGetValue(eventType, out var queue))
            {
                return queue.ToList();
            }
            return new List<AirshipEvent>();
        }

        private void QueueEvent(AirshipEventType eventType, AirshipEvent airshipEvent)
        {
            var queue = _pendingEvents.GetOrAdd(eventType, _ => new ConcurrentQueue<AirshipEvent>());
            
            // Enforce max queue size (FIFO eviction)
            while (queue.Count >= _maxQueueSize && queue.TryDequeue(out _))
            {
                // Remove oldest event
            }
            
            queue.Enqueue(airshipEvent);
            
            // Notify that a pending event is available
            PendingEventAvailable?.Invoke(this, eventType);
        }

        private void DispatchEvent(AirshipEventType eventType, AirshipEvent airshipEvent)
        {
            List<EventHandler<EventArgs>>? listeners = null;
            
            _listenerLock.EnterReadLock();
            try
            {
                if (_listeners.TryGetValue(eventType, out var list))
                {
                    listeners = new List<EventHandler<EventArgs>>(list);
                }
            }
            finally
            {
                _listenerLock.ExitReadLock();
            }

            if (listeners != null && listeners.Count > 0)
            {
                var eventArgs = airshipEvent.ToEventArgs();
                foreach (var listener in listeners)
                {
                    try
                    {
                        listener?.Invoke(this, eventArgs);
                    }
                    catch (Exception ex)
                    {
                        // Log error but continue dispatching to other listeners
                        System.Diagnostics.Debug.WriteLine($"Error dispatching event {eventType}: {ex}");
                    }
                }
            }
        }
        
        private bool _disposed = false;
        
        /// <summary>
        /// Disposes of the AirshipEventEmitter and releases resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// Protected implementation of Dispose pattern.
        /// </summary>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    // Dispose managed resources
                    _listenerLock?.Dispose();
                    
                    // Clear all collections
                    ClearAllPendingEvents();
                    _listeners.Clear();
                }
                
                _disposed = true;
            }
        }
    }
}