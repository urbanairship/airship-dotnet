using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace AirshipDotNet.Controls
{
    /// <summary>
    /// Handler for MessageView control.
    /// </summary>
    public partial class MessageViewHandler
    {
        /// <summary>
        /// Property mapper for MessageView.
        /// </summary>
        public static PropertyMapper<MessageView, MessageViewHandler> PropertyMapper = new PropertyMapper<MessageView, MessageViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(MessageView.MessageId)] = MapMessageId
        };

        /// <summary>
        /// Command mapper for MessageView.
        /// </summary>
        public static CommandMapper<MessageView, MessageViewHandler> CommandMapper = new CommandMapper<MessageView, MessageViewHandler>(ViewHandler.ViewCommandMapper);

        /// <summary>
        /// Initializes a new instance of the MessageViewHandler class.
        /// </summary>
        public MessageViewHandler() : base(PropertyMapper, CommandMapper)
        {
        }

        /// <summary>
        /// Initializes a new instance of the MessageViewHandler class with custom mappers.
        /// </summary>
        public MessageViewHandler(IPropertyMapper mapper, CommandMapper commandMapper = null) : base(mapper, commandMapper ?? CommandMapper)
        {
        }

        static partial void MapMessageId(MessageViewHandler handler, MessageView view);
    }
}