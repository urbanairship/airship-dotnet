/*
 Copyright Airship and Contributors
*/

using System;
using System.Collections;
using System.Collections.Generic;

using Android.OS;
using Java.Lang;
using Java.Util;
using Object = Java.Lang.Object;

namespace UrbanAirship.MessageCenter
{
    public partial class Inbox
	{
		private Dictionary<Action, Listener> eventHandlers = new Dictionary<Action, Listener>();
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

		public void GetMessages(Func<Message, bool> predicate, Action<List<Message>> callback)
		{
			var pendingMessages = GetMessagesPendingResult(new Predicate(predicate));
			pendingMessages.AddResultCallback(
				new ResultCallback((result) => callback.Invoke(CastToList(result)))
			);
		}
		
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
			private Action<Java.Lang.Object?> action;

			internal ResultCallback(Action<Java.Lang.Object?> action)
			{
				this.action = action;
			}

			public void OnResult(Java.Lang.Object? result) => action.Invoke(result);
		}
		
		private List<Message> CastToList(Object? result)
		{
			var list = new List<Message>();

			if (result is IEnumerable items)
			{
				foreach (var item in items)
				{
					list.Add((Message)item);
				}
			}
			
			return list;
		}
	}
}