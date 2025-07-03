using AirshipDotNet.MessageCenter.Controls;
using AndroidX.AppCompat.App;
using AndroidX.Lifecycle;
using Com.Urbanairship.Messagecenter.UI.View;
using Java.Lang;
using Microsoft.Maui.Handlers;
using MessageView = Com.Urbanairship.Messagecenter.UI.View.MessageView;
using Object = Java.Lang.Object;

namespace AirshipDotNet.MessageCenter.Handlers;

/// <summary>
/// Handler responsible for displaying a single Message Center message via the platform MessageWebView.
/// </summary>
[Preserve(AllMembers = true)]
public partial class MessageViewHandler() : ViewHandler<IMessageView, MessageView>(MessageViewMapper)
{
    private MessageViewModel? viewModel = null;
    
    public static PropertyMapper<IMessageView, MessageViewHandler> MessageViewMapper = new(ViewHandler.ViewMapper)
    {
        [nameof(IMessageView.MessageId)] = MapMessageId
    };

    protected override MessageView CreatePlatformView()
    {
        // Get the ViewModel
        var activity = (AppCompatActivity) Platform.CurrentActivity!;
        var factory = MessageViewModel.Factory();
        var modelType = Class.FromType(typeof(MessageViewModel));
        viewModel = (MessageViewModel) new ViewModelProvider(activity, factory).Get(modelType);
        
        // Create the MessageView
        var messageView = new MessageView(Context);

        // Render VM States
        viewModel.StatesLiveData.Observe(activity, new StateObserver(state => messageView.Render(state)));

        // Listen for message loads to mark messages read
        messageView.Listener = new MessageViewListener(viewModel);
        
        return messageView;
    }
    
    private class StateObserver(Action<MessageViewState> action) : Object, IObserver
    {
        public void OnChanged(Object? value)
        {
            if (value is MessageViewState state)
            {
                action(state);
            }
        }
    } 
    
    private class MessageViewListener(MessageViewModel viewModel) : Object, MessageView.IListener
    {
        public void OnMessageLoaded(UrbanAirship.MessageCenter.Message message) => 
            viewModel.MarkMessagesRead(message);

        public void OnMessageLoadError(MessageViewState.Error.Type error) { /* Noop */ }
        public void OnRetryClicked() { /* Noop */ }
    }

    private static void MapMessageId(MessageViewHandler handler, IMessageView entry)
    {
        if (entry.MessageId != null)
        {
            handler.LoadMessage(entry.MessageId);
        }
    }

    protected void LoadMessage(string messageId) => viewModel?.LoadMessage(messageId);
}

