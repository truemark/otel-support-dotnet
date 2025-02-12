using System.Diagnostics;

namespace TrueMark.OtelSupport.Metrics
{
    /// <summary>
    /// Represents a holder for metric tags and their associated values.
    /// </summary>
    /// <typeparam name="T">The type of the value associated with the metric tag.</typeparam>
    public class MetricTagHolder<T> where T : unmanaged
    {
        /// <summary>
        /// Gets or sets the name of the metric tag.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the metric tag.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the unit of the metric tag.
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// Gets or sets the value associated with the metric tag.
        /// </summary>
        public T Value { get; set; }

        /// <summary>
        /// Gets the list of tags associated with the metric.
        /// </summary>
        public TagList TagList { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="MetricTagHolder{T}"/> class.
        /// </summary>
        /// <param name="name">The name of the metric tag.</param>
        /// <param name="description">The description of the metric tag.</param>
        /// <param name="unit">The unit of the metric tag.</param>
        /// <param name="tagList">The list of tags associated with the metric.</param>
        /// <param name="value">The value associated with the metric tag.</param>
        public MetricTagHolder(string name, string description, string unit, TagList tagList = new TagList(), T value = default)
        {
            Name = name;
            Description = description;
            Unit = unit;
            TagList = tagList;
            Value = value;
        }
    }
}