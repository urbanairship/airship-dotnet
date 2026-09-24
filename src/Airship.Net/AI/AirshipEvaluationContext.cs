/* Copyright Airship and Contributors */

using System.Collections.Generic;
using System.Linq;

namespace AirshipDotNet.AI
{
    /// <summary>
    /// A single piece of app-supplied context for an AI evaluation.
    /// </summary>
    public class AirshipEvaluationContextItem
    {
        /// <summary>
        /// Self-describing context text, for example <c>"Favorite category: hiking"</c>.
        /// It is inserted into the model prompt as-is.
        /// </summary>
        public string Content { get; }

        /// <summary>
        /// Relative importance, where <b>lower is more important</b>. When the prompt exceeds
        /// the model's input window, items with the highest value are dropped first. Negative
        /// values rank above the <c>0</c> default.
        /// </summary>
        public double Priority { get; }

        /// <summary>
        /// Creates a context item.
        /// </summary>
        /// <param name="content">The context text.</param>
        /// <param name="priority">Relative importance; lower is more important.</param>
        public AirshipEvaluationContextItem(string content, double priority = 0.0)
        {
            Content = content;
            Priority = priority;
        }
    }

    /// <summary>
    /// App-supplied context for an AI evaluation, as an ordered list of prioritized items.
    /// </summary>
    /// <remarks>
    /// Context goes to the model that runs the evaluation and nowhere else — Airship does not
    /// receive, store, or report it. Where the model runs is your choice: the built-in model
    /// keeps it on the device, while a model you configure may send it elsewhere.
    /// </remarks>
    public class AirshipEvaluationContext
    {
        /// <summary>
        /// The context items, in presentation order.
        /// </summary>
        public IReadOnlyList<AirshipEvaluationContextItem> Items { get; }

        /// <summary>
        /// An empty context, contributing nothing. The evaluation still runs.
        /// </summary>
        public static AirshipEvaluationContext Empty { get; } = new();

        /// <summary>
        /// Creates a context from the given items.
        /// </summary>
        /// <param name="items">The context items, or null for an empty context.</param>
        public AirshipEvaluationContext(IEnumerable<AirshipEvaluationContextItem>? items = null)
        {
            Items = items?.ToList() ?? new List<AirshipEvaluationContextItem>();
        }

        /// <summary>
        /// Creates a context from plain strings, all at the default priority.
        /// </summary>
        /// <param name="contents">The context strings.</param>
        public static AirshipEvaluationContext FromStrings(params string[] contents) =>
            new(contents.Select(c => new AirshipEvaluationContextItem(c)));
    }
}
