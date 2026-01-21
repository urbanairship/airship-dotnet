/*
 Copyright Airship and Contributors
*/

using System;
using System.Collections.Generic;

namespace UrbanAirship.MessageCenter
{
	public partial class User
	{
		private Dictionary<Action<bool>, Listener> eventHandlers = new();

		public event Action<bool> OnUserUpdated
		{
			add
			{
				var listener = new Listener(value);
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

		internal class Listener : Java.Lang.Object, IListener
		{
			private readonly Action<bool> listener;

			public Listener(Action<bool> listener)
			{
				this.listener = listener;
			}

			public void OnUserUpdated(bool success) => listener.Invoke(success);
		}
	}
}
