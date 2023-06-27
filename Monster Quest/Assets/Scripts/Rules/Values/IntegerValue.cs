using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class IntegerValue
    {
        public enum ResolutionStrategy
        {
            Maximum,
            Minimum
        }

        public IntegerValue(IRulesProvider provider, int? baseValue = null, int? modifierValue = null, int? overrideValue = null)
        {
            this.provider = provider;
            this.baseValue = baseValue;
            this.modifierValue = modifierValue;
            this.overrideValue = overrideValue;
        }

        public IRulesProvider provider { get; }
        public int? baseValue { get; }
        public int? modifierValue { get; }
        public int? overrideValue { get; }

        public static int Resolve(IEnumerable<IntegerValue> values, ResolutionStrategy resolutionStrategy = ResolutionStrategy.Maximum)
        {
            // Remove null values.
            IntegerValue[] validValues = values.Where(value => value is not null).ToArray();

            // Categorize values.
            IntegerValue[] baseValues = validValues.Where(value => value.baseValue.HasValue).OrderByDescending(value => value.baseValue).ToArray();
            IntegerValue[] modifierValues = validValues.Where(value => value.modifierValue.HasValue).OrderByDescending(value => value.modifierValue).ToArray();
            IntegerValue[] overrideValues = validValues.Where(value => value.overrideValue.HasValue).OrderByDescending(value => value.overrideValue).ToArray();

            // If minimum values are required we need to reverse base and override values.
            if (resolutionStrategy == ResolutionStrategy.Minimum)
            {
                Array.Reverse(baseValues);
                Array.Reverse(overrideValues);
            }

            // Overrides always take precedence.
            if (overrideValues.Length > 0)
            {
                int value = overrideValues[0].overrideValue.GetValueOrDefault();

                #region Verbose output

                Console.Indent(true);

                Console.WriteLine($"Value is {value} as overriden by {overrideValues[0].provider.rulesProviderName}.");

                if (overrideValues.Length > 1)
                {
                    Console.WriteLine("Other overriding values were:");

                    for (int i = 1; i < overrideValues.Length; i++)
                    {
                        Console.WriteLine($"{overrideValues[i].overrideValue.GetValueOrDefault()} from {overrideValues[i].provider.rulesProviderName}.");
                    }
                }

                if (baseValues.Length > 0)
                {
                    Console.WriteLine("Discarded base values were:");

                    foreach (IntegerValue integerValue in baseValues)
                    {
                        Console.WriteLine($"{integerValue.baseValue.GetValueOrDefault()} from {integerValue.provider.rulesProviderName}.");
                    }
                }

                if (modifierValues.Length > 0)
                {
                    Console.WriteLine("Discarded modifiers were:");

                    foreach (IntegerValue integerValue in modifierValues)
                    {
                        int modifier = integerValue.modifierValue.GetValueOrDefault();
                        Console.WriteLine($"{modifier:+#;-#;0} from {integerValue.provider}.");
                    }
                }

                Console.Outdent();

                #endregion

                return value;
            }

            // Determine the base for the value.
            int baseValue = baseValues.Length > 0 ? baseValues[0].baseValue.GetValueOrDefault() : 0;

            // Determine the modifier value.
            int modifierValue = modifierValues.Sum(value => value.modifierValue.GetValueOrDefault());

            // Determine the result.
            int result = baseValue + modifierValue;

            #region Verbose output

            Console.Indent(true);

            if (modifierValues.Length > 0)
            {
                Console.WriteLine($"Value is {result} ({baseValue}{modifierValue:+#;-#;+0}).");
            }
            else if (baseValues.Length > 0)
            {
                Console.WriteLine($"Value is {result}.");
            }
            else
            {
                Console.WriteLine($"Value is {result} by default.");
            }

            if (baseValues.Length > 0)
            {
                Console.WriteLine("Base values were:");

                foreach (IntegerValue integerValue in baseValues)
                {
                    Console.WriteLine($"{integerValue.baseValue.GetValueOrDefault()} from {integerValue.provider.rulesProviderName}.");
                }
            }

            if (modifierValues.Length > 0)
            {
                Console.WriteLine("Modifiers were:");

                foreach (IntegerValue integerValue in modifierValues)
                {
                    int modifier = integerValue.modifierValue.GetValueOrDefault();
                    Console.WriteLine($"{modifier:+#;-#;0} from {integerValue.provider.rulesProviderName}.");
                }
            }

            Console.Outdent();

            #endregion

            return result;
        }
    }
}
