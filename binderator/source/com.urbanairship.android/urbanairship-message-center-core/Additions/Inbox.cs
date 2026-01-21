/*
 Copyright Airship and Contributors
*/

using Android.OS;
using UrbanAirship;
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

		public ICancelable FetchMessages(Action<bool> callback) => FetchMessages(new FetchMessagesCallbackImpl(callback));

		public ICancelable FetchMessages(Looper looper, Action<bool> callback)
		{
			return FetchMessages(looper, new FetchMessagesCallbackImpl(callback));
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
			var pendingMessages = GetMessagesPendingResult(new MessagePredicate(predicate));
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

		public void GetUnreadCount(Action<int> callback) => UnreadCountPendingResult.AddResultCallback(
			new ResultCallback((result) => callback.Invoke(((Java.Lang.Integer)result!).IntValue()))
		);

		public void GetCount(Action<int> callback) => CountPendingResult.AddResultCallback(
			new ResultCallback((result) => callback.Invoke(((Java.Lang.Integer)result!).IntValue()))
		);

		public void GetUnreadMessages(Action<List<Message>> callback)
		{
			var pendingMessages = GetUnreadMessagesPendingResult(null);
			pendingMessages.AddResultCallback(
				new ResultCallback((result) => callback.Invoke(CastToList(result)))
			);
		}

		public void GetReadMessages(Action<List<Message>> callback)
		{
			var pendingMessages = GetReadMessagesPendingResult(null);
			pendingMessages.AddResultCallback(
				new ResultCallback((result) => callback.Invoke(CastToList(result)))
			);
		}

		public void GetReadCount(Action<int> callback) => ReadCountPendingResult.AddResultCallback(
			new ResultCallback((result) => callback.Invoke(((Java.Lang.Integer)result!).IntValue()))
		);

		internal class Listener : Java.Lang.Object, IInboxListener
		{
			private readonly Action listener;

			public Listener(Action listener)
			{
				this.listener = listener;
			}

			public void OnInboxUpdated() => listener.Invoke();
		}

		internal class FetchMessagesCallbackImpl : Java.Lang.Object, IFetchMessagesCallback
		{
			private readonly Action<bool> callback;

			public FetchMessagesCallbackImpl(Action<bool> callback)
			{
				this.callback = callback;
			}

			public void OnFinished(bool success) => callback.Invoke(success);
		}

		public class MessagePredicate : Java.Lang.Object, IPredicate
		{
			private readonly Func<Message, bool> predicate;

			public MessagePredicate(Func<Message, bool> predicate)
			{
				this.predicate = predicate;
			}

			public bool Apply(Java.Lang.Object value)
			{
				if (value is Message message)
				{
					return predicate.Invoke(message);
				}
				return false;
			}
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
