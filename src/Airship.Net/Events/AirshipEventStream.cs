/* Copyright Airship and Contributors */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AirshipDotNet.Events
{
    /// <summary>
    /// Represents an event stream that can handle multiple event types.
    /// Similar to Flutter's AirshipEventStream implementation.
    /// </summary>
    public class AirshipEventStream : IDisposable
    {
        private readonly List<AirshipEventType> _eventTypes;
        private readonly string _streamName;
        private readonly List<EventHandler<EventArgs>> _handlers;
        private readonly ReaderWriterLockSlim _lock;
        private bool _isListening;

        /// <summary>
        /// Event name mappings similar to Flutter's EVENT_NAME_MAP.
        /// Multiple event types can map to the same stream.
        /// </summary>
        public static readonly Dictionary<AirshipEventType, string> EventNameMap = new()
        {
            { AirshipEventType.BackgroundNotificationResponse, "com.airship.dotnet/event/notification_response" },
            { AirshipEventType.ForegroundNotificationResponse, "com.airship.dotnet/event/notification_response" },
            { AirshipEventType.NotificationResponse, "com.airship.dotnet/event/notification_response" },
            { AirshipEventType.ChannelCreated, "com.airship.dotnet/event/channel_created" },
            { AirshipEventType.DeepLinkReceived, "com.airship.dotnet/event/deep_link_received" },
            { AirshipEventType.DisplayMessageCenter, "com.airship.dotnet/event/display_message_center" },
            { AirshipEventType.DisplayPreferenceCenter, "com.airship.dotnet/event/display_preference_center" },
            { AirshipEventType.MessageCenterUpdated, "com.airship.dotnet/event/message_center_updated" },
            { AirshipEventType.PushTokenReceived, "com.airship.dotnet/event/push_token_received" },
            { AirshipEventType.PushReceived, "com.airship.dotnet/event/push_received" },
            { AirshipEventType.BackgroundPushReceived, "com.airship.dotnet/event/background_push_received" },
            { AirshipEventType.NotificationStatusChanged, "com.airship.dotnet/event/notification_status_changed" },
            { AirshipEventType.PendingEmbeddedUpdated, "com.airship.dotnet/event/pending_embedded_updated" },
            { AirshipEventType.AuthorizedNotificationSettingsChanged, "com.airship.dotnet/event/ios_authorized_notification_settings_changed" }
        };

        /// <summary>
        /// Initializes a new instance of the AirshipEventStream class.
        /// </summary>
        /// <param name="eventTypes">The event types this stream handles.</param>
        /// <param name="streamName">The name of the stream.</param>
        public AirshipEventStream(List<AirshipEventType> eventTypes, string streamName)
        {
            _eventTypes = eventTypes;
            _streamName = streamName;
            _handlers = new List<EventHandler<EventArgs>>();
            _lock = new ReaderWriterLockSlim();
            _isListening = false;
        }

        /// <summary>
        /// Gets the stream name.
        /// </summary>
        public string StreamName => _streamName;

        /// <summary>
        /// Gets the event types handled by this stream.
        /// </summary>
        public IReadOnlyList<AirshipEventType> EventTypes => _eventTypes.AsReadOnly();

        /// <summary>
        /// Registers a handler for this stream.
        /// </summary>
        public void Register(EventHandler<EventArgs> handler)
        {
            _lock.EnterWriteLock();
            try
            {
                _handlers.Add(handler);
                
                // When first handler is added, start listening
                if (_handlers.Count == 1 && !_isListening)
                {
                    _isListening = true;
                    OnListenerAdded();
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Unregisters a handler from this stream.
        /// </summary>
        public void Unregister(EventHandler<EventArgs> handler)
        {
            _lock.EnterWriteLock();
            try
            {
                _handlers.Remove(handler);
                
                // When last handler is removed, stop listening
                if (_handlers.Count == 0 && _isListening)
                {
                    _isListening = false;
                    OnListenerRemoved();
                }
            }
            finally
            {
                _lock.ExitWriteLock();
            }
        }

        /// <summary>
        /// Processes pending events for this stream.
        /// </summary>
        public async Task ProcessPendingEvents()
        {
            foreach (var eventType in _eventTypes)
            {
                await AirshipEventEmitter.Shared.ProcessPendingEvents(eventType, (eventData) =>
                {
                    return Notify(eventData);
                });
            }
        }

        /// <summary>
        /// Notifies all handlers of an event.
        /// </summary>
        /// <param name="eventData">The event data to notify.</param>
        /// <returns>True if at least one handler processed the event.</returns>
        public bool Notify(AirshipEvent eventData)
        {
            List<EventHandler<EventArgs>> handlersCopy;
            
            _lock.EnterReadLock();
            try
            {
                if (_handlers.Count == 0)
                    return false;
                    
                handlersCopy = new List<EventHandler<EventArgs>>(_handlers);
            }
            finally
            {
                _lock.ExitReadLock();
            }

            bool handled = false;
            var eventArgs = eventData.ToEventArgs();
            
            foreach (var handler in handlersCopy)
            {
                try
                {
                    handler?.Invoke(this, eventArgs);
                    handled = true;
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error notifying handler for {_streamName}: {ex}");
                }
            }
            
            return handled;
        }

        /// <summary>
        /// Called when the first listener is added.
        /// </summary>
        private void OnListenerAdded()
        {
            // Process any pending events when listener is added
            _ = ProcessPendingEvents();
        }

        /// <summary>
        /// Called when the last listener is removed.
        /// </summary>
        private void OnListenerRemoved()
        {
            // Stream is no longer active
        }

        /// <summary>
        /// Creates event streams from the event name mappings.
        /// </summary>
        public static Dictionary<AirshipEventType, AirshipEventStream> GenerateEventStreams()
        {
            // Group event types by stream name
            var streamGroups = new Dictionary<string, List<AirshipEventType>>();
            
            foreach (var mapping in EventNameMap)
            {
                if (!streamGroups.ContainsKey(mapping.Value))
                {
                    streamGroups[mapping.Value] = new List<AirshipEventType>();
                }
                streamGroups[mapping.Value].Add(mapping.Key);
            }

            // Create streams and map to event types
            var streamMap = new Dictionary<AirshipEventType, AirshipEventStream>();
            
            foreach (var group in streamGroups)
            {
                var stream = new AirshipEventStream(group.Value, group.Key);
                
                foreach (var eventType in group.Value)
                {
                    streamMap[eventType] = stream;
                }
            }
            
            return streamMap;
        }
        
        private bool _disposed = false;
        
        /// <summary>
        /// Disposes of the AirshipEventStream and releases resources.
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
                    _lock?.Dispose();
                    
                    // Clear handlers
                    _handlers.Clear();
                }
                
                _disposed = true;
            }
        }
    }
}