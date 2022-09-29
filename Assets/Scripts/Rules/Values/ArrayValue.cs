using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class ArrayValue<T>
    {
        public ArrayValue(IRulesProvider provider, IEnumerable<T> addItems = null, IEnumerable<T> removeItems = null, IEnumerable<T> overrideItems = null, int priority = 0)
        {
            this.provider = provider;
            this.addItems = addItems;
            this.overrideItems = overrideItems;
            this.removeItems = removeItems;
            this.priority = priority;
        }

        public IRulesProvider provider { get; }
        public IEnumerable<T> addItems { get; }
        public IEnumerable<T> overrideItems { get; }
        public IEnumerable<T> removeItems { get; }
        public int priority { get; }

        public static T[] Resolve(IEnumerable<ArrayValue<T>> values)
        {
            // Remove null values.
            ArrayValue<T>[] validValues = values.Where(value => value != null).ToArray();

            // Find overriding values.
            ArrayValue<T>[] overrideValues = validValues.Where(value => value.overrideItems != null).OrderByDescending(value => value.priority).ToArray();
            ArrayValue<T>[] addValues = validValues.Where(value => value.addItems != null).ToArray();
            ArrayValue<T>[] removeValues = validValues.Where(value => value.removeItems != null).ToArray();

            // Overrides always take precedence.
            if (overrideValues.Length > 0)
            {
                T[] overrideItems = overrideValues[0].overrideItems.ToArray();

                #region Verbose output

                if (Console.verbose)
                {
                    Console.WriteLine($"Values are {StringHelpers.JoinWithAnd(overrideItems)} from {overrideValues[0].provider} with priority {overrideValues[0].priority}.");

                    if (overrideValues.Length > 1)
                    {
                        Console.WriteLine("Other overriding values were:");

                        for (int i = 1; i < overrideValues.Length; i++)
                        {
                            Console.WriteLine($"{StringHelpers.JoinWithAnd(overrideValues[i].overrideItems)} from {overrideValues[i].provider.rulesProviderName} with priority {overrideValues[i].priority}.");
                        }
                    }

                    if (addValues.Length + removeValues.Length > 0)
                    {
                        Console.WriteLine("Discarded items were:");

                        foreach (ArrayValue<T> addValue in addValues)
                        {
                            Console.WriteLine($"{StringHelpers.JoinWithAnd(addValue.addItems)} added from {addValue.provider.rulesProviderName}.");
                        }

                        foreach (ArrayValue<T> removeValue in removeValues)
                        {
                            Console.WriteLine($"{StringHelpers.JoinWithAnd(removeValue.addItems)} removed from {removeValue.provider.rulesProviderName}.");
                        }
                    }
                }

                #endregion

                return overrideItems;
            }

            HashSet<T> items = new();

            foreach (ArrayValue<T> addValue in addValues)
            {
                items.UnionWith(addValue.addItems);
            }

            foreach (ArrayValue<T> removeValue in removeValues)
            {
                items.ExceptWith(removeValue.removeItems);
            }

            #region Verbose output

            if (Console.verbose)
            {
                if (items.Count > 0)
                {
                    Console.WriteLine($"Values are {StringHelpers.JoinWithAnd(items)}.");
                }
                else
                {
                    Console.WriteLine("Values are empty.");
                }

                foreach (ArrayValue<T> addValue in addValues)
                {
                    if (addValue.addItems.Any())
                    {
                        Console.WriteLine($"{StringHelpers.JoinWithAnd(addValue.addItems)} added from {addValue.provider.rulesProviderName}.");
                    }
                }

                foreach (ArrayValue<T> removeValue in removeValues)
                {
                    if (removeValue.addItems.Any())
                    {
                        Console.WriteLine($"{StringHelpers.JoinWithAnd(removeValue.addItems)} removed from {removeValue.provider.rulesProviderName}.");
                    }
                }
            }

            #endregion

            return items.ToArray();
        }
    }
}
