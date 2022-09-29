using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class SingleValue<T>
    {
        public SingleValue(IRulesProvider provider, T value, int priority = 0)
        {
            this.provider = provider;
            this.value = value;
            this.priority = priority;
        }

        public IRulesProvider provider { get; }
        public T value { get; }
        public int priority { get; }

        public static T Resolve(IEnumerable<SingleValue<T>> values)
        {
            // Remove null values.
            SingleValue<T>[] validValues = values.Where(value => value != null).ToArray();

            // Sort by priority.
            SingleValue<T>[] sortedValues = validValues.OrderByDescending(value => value.priority).ToArray();

            // Return the highest priority value.
            SingleValue<T> highestPriorityValue = sortedValues.Length > 0 ? sortedValues[0] : null;
            T value = highestPriorityValue != null ? highestPriorityValue.value : default;

            // Explain the decision.

            #region Verbose output

            if (Console.verbose)
            {
                if (highestPriorityValue == null)
                {
                    Console.WriteLine($"Value is {(value == null ? "null" : value)} by default.");
                }
                else
                {
                    Console.WriteLine($"Value is {value} from {highestPriorityValue.provider.rulesProviderName} with priority {highestPriorityValue.priority}.");

                    if (sortedValues.Length > 1)
                    {
                        Console.WriteLine("Other values were:");

                        for (int i = 1; i < sortedValues.Length; i++)
                        {
                            Console.WriteLine($"{sortedValues[i]} from {highestPriorityValue.provider.rulesProviderName} with priority {highestPriorityValue.priority}.");
                        }
                    }
                }
            }

            #endregion

            return value;
        }
    }
}
