/*
 Copyright Airship and Contributors
*/

using Android.OS;
using Android.Runtime;
using UrbanAirship;

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
				new ResultCallback((result) =>
				{
					// A null result means "no such message". Anything else that isn't a Message
					// means the peer didn't marshal as expected; log it rather than throwing an
					// InvalidCastException back across the JNI callback.
					var message = result as Message;
					if (result != null && message == null)
					{
						UALog.E("Unexpected result type reading inbox message " + messageId + ": " + result.Class?.Name);
					}
					callback.Invoke(message);
				})
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

				// Returning false here silently drops the message from the filtered results,
				// so make the reason visible rather than reporting an empty inbox.
				UALog.E("Unexpected result type in inbox message predicate: " + value?.Class?.Name);
				return false;
			}
		}

		// PendingResult<List<Message>> erases to Object over JNI, so the result arrives as a
		// bare Java.Lang.Object. Casting that peer to Java.Util.IList would depend on the
		// runtime type map, and Release builds trim Java.Util.ArrayList (and its AbstractList
		// base) out of Mono.Android, leaving the cast null and the list silently empty.
		//
		// Construct JavaList<T> around the handle directly. JavaList<T>.FromJniHandle is not
		// safe here: for a runtime class the type map doesn't cover, Mono.Android may already
		// have a non-generic Android.Runtime.JavaList peer registered for the handle, and
		// FromJniHandle hard-casts that peer to JavaList<T> and throws.
		private static List<Message> CastToList(Java.Lang.Object? result)
		{
			var list = new List<Message>();

			if (result == null)
			{
				return list;
			}

			try
			{
				var javaList = new JavaList<Message>(result.Handle, JniHandleOwnership.DoNotTransfer);
				foreach (var message in javaList)
				{
					if (message != null)
					{
						list.Add(message);
					}
				}
			}
			catch (Exception e)
			{
				UALog.E("Failed to read the inbox message list returned by the Android SDK: " + e);
				list.Clear();
			}

			return list;
		}
	}
}
