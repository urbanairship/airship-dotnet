/* Copyright Airship and Contributors */

namespace AirshipDotNet.Channel
{
    /// <summary>
    /// Interface used to subscribe/unsubscribe from a list.
    /// </summary>
    public class SubscriptionListEditor
    {
        private readonly List<SubscriptionListOperation> operations = new();
        private readonly Action<List<SubscriptionListOperation>> onApply;

        //@cond IGNORE
        public SubscriptionListEditor(Action<List<SubscriptionListOperation>> onApply)
        {
            this.onApply = onApply;
        }
        //@endcond

        /// <summary>
        /// ASubscribe to a given tag list.
        /// </summary>
        /// <returns>The subscriotion list editor.</returns>
        /// <param name="list">The subscription list identifier to subscribe to.</param>
        public SubscriptionListEditor Subscribe(string list)
        {
            operations.Add(new SubscriptionListOperation(OperationType.SUBSCRIBE, list));
            return this;

        }

        /// <summary>
        /// Unsubscribe from a given list.
        /// </summary>
        /// <returns>The subscriotion list editor.</returns>
        /// <param name="list">The subscription list identifier to unsubscribe from.</param>
        public SubscriptionListEditor Unsubscribe(string list)
        {
            operations.Add(new SubscriptionListOperation(OperationType.UNSUBSCRIBE, list));
            return this;
        }

        /// <summary>
        /// Apply the tag group changes.
        /// </summary>
        public void Apply()
        {
            onApply?.Invoke(operations);
        }

        //@cond IGNORE
        public class SubscriptionListOperation
        {
            public OperationType OperationType;
            public string List;

            internal SubscriptionListOperation(OperationType operation, string list)
            {
                OperationType = operation;
                List = list;
            }
        }

        public enum OperationType { SUBSCRIBE, UNSUBSCRIBE }
        //@endcond
    }
}
