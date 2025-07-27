using Microsoft.Maui.Controls;
using System;

namespace AirshipDotNet.Controls
{
    /// <summary>
    /// A view for displaying Airship message center messages.
    /// </summary>
    public class MessageView : View
    {
        /// <summary>
        /// Bindable property for MessageId.
        /// </summary>
        public static readonly BindableProperty MessageIdProperty = BindableProperty.Create(
            nameof(MessageId),
            typeof(string),
            typeof(MessageView),
            default(string));

        /// <summary>
        /// Gets or sets the message ID to display.
        /// </summary>
        public string MessageId
        {
            get => (string)GetValue(MessageIdProperty);
            set => SetValue(MessageIdProperty, value);
        }

        /// <summary>
        /// Occurs when the message starts loading.
        /// </summary>
        public event EventHandler<MessageLoadStartedEventArgs>? LoadStarted;

        /// <summary>
        /// Occurs when the message finishes loading.
        /// </summary>
        public event EventHandler<MessageLoadFinishedEventArgs>? LoadFinished;

        /// <summary>
        /// Occurs when the message fails to load.
        /// </summary>
        public event EventHandler<MessageLoadFailedEventArgs>? LoadFailed;

        /// <summary>
        /// Occurs when the message view is closed.
        /// </summary>
        public event EventHandler<MessageClosedEventArgs>? Closed;

        internal void SendLoadStarted() => LoadStarted?.Invoke(this, new MessageLoadStartedEventArgs());
        internal void SendLoadFinished() => LoadFinished?.Invoke(this, new MessageLoadFinishedEventArgs());
        internal void SendLoadFailed(string error) => LoadFailed?.Invoke(this, new MessageLoadFailedEventArgs { Error = error });
        internal void SendClosed() => Closed?.Invoke(this, new MessageClosedEventArgs());
    }

    /// <summary>
    /// Event args for when a message starts loading.
    /// </summary>
    public class MessageLoadStartedEventArgs : EventArgs
    {
    }

    /// <summary>
    /// Event args for when a message finishes loading.
    /// </summary>
    public class MessageLoadFinishedEventArgs : EventArgs
    {
    }

    /// <summary>
    /// Event args for when a message fails to load.
    /// </summary>
    public class MessageLoadFailedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Error { get; set; } = string.Empty;
    }

    /// <summary>
    /// Event args for when a message is closed.
    /// </summary>
    public class MessageClosedEventArgs : EventArgs
    {
    }
}