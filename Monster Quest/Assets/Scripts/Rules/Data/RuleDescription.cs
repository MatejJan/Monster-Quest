using System.Collections.Generic;

namespace MonsterQuest
{
    public class RuleDescription
    {
        public RuleDescription(RuleCategory category, string name, string type, string description)
        {
            this.category = category;
            this.name = name;
            this.type = type;
            this.description = description;
        }

        public RuleCategory category { get; }
        public string name { get; }
        public string type { get; }
        public string description { get; }

        public override string ToString()
        {
            List<string> parts = new();

            if (category != RuleCategory.None)
            {
                parts.Add($"{category}:");
            }

            parts.Add($"{name.ToStartCase()}.");

            if (type != null)
            {
                parts.Add($"{type.ToStartCase()}:");
            }

            parts.Add($"{description}");

            return string.Join(" ", parts);
        }
    }
}
