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

		internal class Listener(Action<bool> listener) : Java.Lang.Object, IListener
		{
			public void OnUserUpdated(bool success) => listener.Invoke(success);
		}
	}
}
