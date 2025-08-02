using Microsoft.Maui;
using Microsoft.Maui.Handlers;

namespace AirshipDotNet.MessageCenter.Controls
{
    /// <summary>
    /// Handler for MessageView control.
    /// </summary>
    public partial class MessageViewHandler
    {
        /// <summary>
        /// Property mapper for MessageView.
        /// </summary>
        public static IPropertyMapper<MessageView, IViewHandler> PropertyMapper = new PropertyMapper<MessageView, IViewHandler>(ViewHandler.ViewMapper)
        {
            [nameof(MessageView.MessageId)] = (handler, view) => MapMessageId(handler, view)
        };

        /// <summary>
        /// Command mapper for MessageView.
        /// </summary>
        public static CommandMapper<MessageView, IViewHandler> CommandMapper = new CommandMapper<MessageView, IViewHandler>(ViewHandler.ViewCommandMapper);

        static partial void MapMessageId(IViewHandler handler, MessageView view);
    }
}