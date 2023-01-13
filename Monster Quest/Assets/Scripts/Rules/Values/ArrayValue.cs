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
            ArrayValue<T>[] validValues = values.Where(value => value is not null).ToArray();

            // Find overriding values.
            ArrayValue<T>[] overrideValues = validValues.Where(value => value.overrideItems is not null).OrderByDescending(value => value.priority).ToArray();
            ArrayValue<T>[] addValues = validValues.Where(value => value.addItems is not null).ToArray();
            ArrayValue<T>[] removeValues = validValues.Where(value => value.removeItems is not null).ToArray();

            // Overrides always take precedence.
            if (overrideValues.Length > 0)
            {
                T[] overrideItems = overrideValues[0].overrideItems.ToArray();

                #region Verbose output

                if (Console.verbose)
                {
                    Console.WriteLine($"Values are {EnglishHelper.JoinWithAnd(overrideItems)} from {overrideValues[0].provider} with priority {overrideValues[0].priority}.");

                    if (overrideValues.Length > 1)
                    {
                        Console.WriteLine("Other overriding values were:");

                        for (int i = 1; i < overrideValues.Length; i++)
                        {
                            Console.WriteLine($"{EnglishHelper.JoinWithAnd(overrideValues[i].overrideItems)} from {overrideValues[i].provider.rulesProviderName} with priority {overrideValues[i].priority}.");
                        }
                    }

                    if (addValues.Length + removeValues.Length > 0)
                    {
                        Console.WriteLine("Discarded items were:");

                        foreach (ArrayValue<T> addValue in addValues)
                        {
                            Console.WriteLine($"{EnglishHelper.JoinWithAnd(addValue.addItems)} added from {addValue.provider.rulesProviderName}.");
                        }

                        foreach (ArrayValue<T> removeValue in removeValues)
                        {
                            Console.WriteLine($"{EnglishHelper.JoinWithAnd(removeValue.addItems)} removed from {removeValue.provider.rulesProviderName}.");
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
                    Console.WriteLine($"Values are {EnglishHelper.JoinWithAnd(items)}.");
                }
                else
                {
                    Console.WriteLine("Values are empty.");
                }

                foreach (ArrayValue<T> addValue in addValues)
                {
                    if (addValue.addItems.Any())
                    {
                        Console.WriteLine($"{EnglishHelper.JoinWithAnd(addValue.addItems)} added from {addValue.provider.rulesProviderName}.");
                    }
                }

                foreach (ArrayValue<T> removeValue in removeValues)
                {
                    if (removeValue.addItems.Any())
                    {
                        Console.WriteLine($"{EnglishHelper.JoinWithAnd(removeValue.addItems)} removed from {removeValue.provider.rulesProviderName}.");
                    }
                }
            }

            #endregion

            return items.ToArray();
        }
    }
}
