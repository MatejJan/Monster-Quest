using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class MultipleValue<T>
    {
        public MultipleValue(IRulesProvider provider, T value)
        {
            this.provider = provider;
            this.value = value;
        }

        public IRulesProvider provider { get; }
        public T value { get; }

        public static T[] Resolve(IEnumerable<MultipleValue<T>> values)
        {
            // Remove null values.
            MultipleValue<T>[] validValues = values.Where(value => value is not null).ToArray();

            // Collect all values.
            HashSet<T> items = new();

            foreach (MultipleValue<T> value in validValues)
            {
                items.Add(value.value);
            }

            #region Verbose output

            Console.Indent(true);

            Console.WriteLine($"Values are {(items.Count > 0 ? EnglishHelper.JoinWithAnd(items) : "empty")}.");

            foreach (MultipleValue<T> value in validValues)
            {
                Console.WriteLine($"{value.value} from {value.provider.rulesProviderName}.");
            }

            Console.Outdent();

            #endregion

            return items.ToArray();
        }
    }
}
