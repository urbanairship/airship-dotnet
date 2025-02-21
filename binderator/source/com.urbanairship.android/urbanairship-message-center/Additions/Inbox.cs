/*
 Copyright Airship and Contributors
*/

using Android.OS;
using IList = Java.Util.IList;

namespace UrbanAirship.MessageCenter
{
    public partial class Inbox
	{
		private readonly Dictionary<Action, Listener> eventHandlers = new();
		
		public event Action OnInboxUpdated
		{
			add
			{
				Listener listener = new Listener(value);
				AddListener(listener);
				eventHandlers.Add(value, listener);
			}

			remove
			{
				if (eventHandlers.ContainsKey(value))
				{
					RemoveListener(eventHandlers[value]);
					eventHandlers.Remove(value);
				}
			}
		}

		public ICancelable FetchMessages(Action<bool> callback) => FetchMessages(new FetchMessagesCallback(callback));

		public ICancelable FetchMessages(Looper looper, Action<bool> callback)
		{
			return FetchMessages(looper, new FetchMessagesCallback (callback));
		}

		public void GetMessages(Action<List<Message>> callback)
		{
			var pendingMessages = GetMessagesPendingResult(null);
			pendingMessages.AddResultCallback(
				new ResultCallback((result) => callback.Invoke(CastToList(result)))
			);
		}
		
		public void GetMessages(Func<Message, bool> predicate, Action<List<Message>> callback)
		{
			var pendingMessages = GetMessagesPendingResult(new Predicate(predicate));
			pendingMessages.AddResultCallback(
				new ResultCallback((result) => callback.Invoke(CastToList(result)))
			);
		}
		
		public void GetMessage(string messageId, Action<Message?> callback)
		{
			var pendingMessage = GetMessagePendingResult(messageId);
			pendingMessage.AddResultCallback(
				new ResultCallback((result) => callback.Invoke((Message?)result))
			);
		}
		
		public void UnreadCount(Action<int> callback) => UnreadCountPendingResult.AddResultCallback(
			new ResultCallback((result) => callback.Invoke((int)result!))
		);

		public void Count(Action<int> callback) => CountPendingResult.AddResultCallback(
			new ResultCallback((result) => callback.Invoke((int)result!))
		);
		
		internal class Listener(Action listener) : Java.Lang.Object, IInboxListener
        {
	        public void OnInboxUpdated() => listener.Invoke();
        }

		internal class FetchMessagesCallback(Action<bool> callback) : Java.Lang.Object, IFetchMessagesCallback
		{
			public void OnFinished (bool success) => callback.Invoke (success);
		}

		public class Predicate(Func<Message, bool> predicate) : Java.Lang.Object, IPredicate
		{
			public bool Apply(Message message) => predicate.Invoke (message);

			public bool Apply(Java.Lang.Object p0) => throw new NotImplementedException();
		}
		
		private class ResultCallback : Java.Lang.Object, IResultCallback
		{
			private readonly Action<Java.Lang.Object?> action;

			internal ResultCallback(Action<Java.Lang.Object?> action) => this.action = action;

			public void OnResult(Java.Lang.Object? result) => action.Invoke(result);
		}
		
		private List<Message> CastToList(Java.Lang.Object? result)
		{
			var list = new List<Message>();

			var enumerable = (result as IList)?.ToEnumerable();
			if (enumerable == null) return list;

			list.AddRange(enumerable.Cast<Message>());
			return list;
		}
	}
}