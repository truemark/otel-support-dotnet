using System.Diagnostics;

namespace TrueMark.Otel.Helper
{
    public class MetricTagHolder<T> where T : unmanaged
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }

        public T Value { get; set; }

        public TagList TagList { get; }

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
