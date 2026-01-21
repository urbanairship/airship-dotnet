using System;

namespace UrbanAirship.Actions
{
	public partial class ActionRegistry
	{
		// NOTE: In SDK version 20.x, the Predicate property on Entry became read-only.
		// To set a predicate, use ActionRegistry.UpdateEntry(name, predicate) instead.
		// The FuncPredicate helper class is still available to wrap Func<> delegates.

		/// <summary>
		/// Helper class to wrap a Func delegate as an IActionPredicate
		/// </summary>
		public class FuncPredicate : Java.Lang.Object, IActionPredicate
		{
			private readonly Func<ActionArguments, bool> predicate;

			public FuncPredicate(Func<ActionArguments, bool> predicate)
			{
				this.predicate = predicate ?? throw new ArgumentNullException(nameof(predicate));
			}

			public bool Apply(ActionArguments arguments)
			{
				return predicate.Invoke(arguments);
			}
		}
	}
}
