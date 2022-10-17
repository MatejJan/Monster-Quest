using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class DamageAmountAlterationValue
    {
        public DamageAmountAlterationValue(IRulesProvider provider, bool vulnerability = false, bool resistance = false, bool immunity = false)
        {
            this.provider = provider;

            this.vulnerability = vulnerability;
            this.resistance = resistance;
            this.immunity = immunity;
        }

        public IRulesProvider provider { get; }

        public bool vulnerability { get; }
        public bool resistance { get; }
        public bool immunity { get; }

        public static DamageAlteration Resolve(IEnumerable<DamageAmountAlterationValue> values)
        {
            // Remove null values.
            DamageAmountAlterationValue[] validValues = values.Where(value => value is not null).ToArray();

            // Categorize values.
            DamageAmountAlterationValue[] vulnerabilityValues = validValues.Where(value => value.vulnerability).ToArray();
            DamageAmountAlterationValue[] resistanceValues = validValues.Where(value => value.resistance).ToArray();
            DamageAmountAlterationValue[] immunityValues = validValues.Where(value => value.immunity).ToArray();

            // There must be at least one value provider in the category for the category to be active.
            DamageAlteration result = new() { vulnerability = vulnerabilityValues.Length > 0, resistance = resistanceValues.Length > 0, immunity = immunityValues.Length > 0 };

            #region Verbose output

            if (Console.verbose)
            {
                List<string> categories = new();
                if (result.vulnerability) categories.Add("vulnerable");
                if (result.resistance) categories.Add("resistant");
                if (result.immunity) categories.Add("immune");

                if (categories.Count > 0)
                {
                    Console.WriteLine($"The target is {StringHelper.JoinWithAnd(categories)} to this damage.");

                    foreach (DamageAmountAlterationValue value in vulnerabilityValues)
                    {
                        Console.WriteLine($"Vulnerable from {value.provider.rulesProviderName}.");
                    }

                    foreach (DamageAmountAlterationValue value in resistanceValues)
                    {
                        Console.WriteLine($"Resistant from {value.provider.rulesProviderName}.");
                    }

                    foreach (DamageAmountAlterationValue value in immunityValues)
                    {
                        Console.WriteLine($"Immune from {value.provider.rulesProviderName}.");
                    }
                }
                else
                {
                    Console.WriteLine("The target is not vulnerable, resistant, or immune to this damage.");
                }
            }

            #endregion

            return result;
        }
    }
}
