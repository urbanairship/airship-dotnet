using AirshipDotNet;
using AirshipDotNet.MessageCenter;
using AirshipDotNet.MessageCenter.Controls;

namespace MauiSample;

public partial class MessagePage : ContentPage
{
    public event EventHandler<MessageLoadStartedEventArgs> LoadStarted;
    public event EventHandler<MessageLoadFinishedEventArgs> LoadFinished;
    public event EventHandler<MessageLoadFailedEventArgs> LoadFailed;
    public event EventHandler<MessageClosedEventArgs> Closed;

    private string _messageId = "";

    public MessagePage()
    {
        InitializeComponent();
    }

    public string MessageId
    {
        get => _messageId;
        set
        {
            if (value != _messageId)
            {
                _messageId = value;
                messageView.MessageId = _messageId;
            }
        }
    }

    void MessageView_LoadStarted(object sender, MessageLoadStartedEventArgs args) =>
        LoadStarted?.Invoke(this, args);

    void MessageView_LoadFailed(object sender, MessageLoadFailedEventArgs args) =>
        LoadFailed?.Invoke(this, args);

    async void MessageView_LoadFinished(object sender, MessageLoadFinishedEventArgs args)
    {
        LoadFinished?.Invoke(this, args);

        try
        {
            await AirshipDotNet.Airship.Instance.MessageCenter().MarkRead(MessageId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error marking message as read: {ex.Message}");
        }
    }

    void MessageView_Closed(object sender, MessageClosedEventArgs args) =>
        Closed?.Invoke(this, args);
}
