using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest
{
    public class CoverValue
    {
        public CoverValue(IRulesProvider provider, Cover provideCover = Cover.None, Cover removeCoverUpTo = Cover.None)
        {
            this.provider = provider;
            this.provideCover = provideCover;
            this.removeCoverUpTo = removeCoverUpTo;
        }

        public IRulesProvider provider { get; }
        public Cover provideCover { get; }
        public Cover removeCoverUpTo { get; }

        public static Cover Resolve(IEnumerable<CoverValue> values)
        {
            // Remove null values.
            CoverValue[] validValues = values.Where(value => value != null).ToArray();

            // Categorize values.
            CoverValue[] providerValues = validValues.Where(value => value.provideCover > Cover.None).OrderByDescending(value => value.provideCover).ToArray();
            CoverValue[] removerValues = validValues.Where(value => value.removeCoverUpTo > Cover.None).OrderByDescending(value => value.removeCoverUpTo).ToArray();

            // Get the maximum cover from the providers.
            Cover providedCover = providerValues.Length > 0 ? providerValues[0].provideCover : Cover.None;

            // See if any value can remove this cover.
            Cover removeCoverUpTo = removerValues.Length > 0 ? removerValues[0].removeCoverUpTo : Cover.None;

            Cover cover = removeCoverUpTo >= providedCover ? Cover.None : providedCover;

            #region Verbose output

            if (Console.verbose)
            {
                if (providerValues.Length == 0)
                {
                    Console.WriteLine("There is no cover.");
                }
                else
                {
                    if (cover == Cover.None)
                    {
                        Console.WriteLine($"Cover was removed by {removerValues[0].provider.rulesProviderName} (up to {removerValues[0].removeCoverUpTo}).");

                        Console.WriteLine("Covers were:");

                        foreach (CoverValue value in providerValues)
                        {
                            Console.WriteLine($"{value.provideCover} from {value.provider.rulesProviderName}.");
                        }

                        if (removerValues.Length > 1)
                        {
                            Console.WriteLine("Other cover removers were:");

                            for (int i = 1; i < removerValues.Length; i++)
                            {
                                Console.WriteLine($"{providerValues[i].provider.rulesProviderName} (up to {providerValues[i].removeCoverUpTo}).");
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Cover is {cover} from {providerValues[0].provider.rulesProviderName}.");

                        if (providerValues.Length > 1)
                        {
                            Console.WriteLine("Other covers were:");

                            for (int i = 1; i < removerValues.Length; i++)
                            {
                                Console.WriteLine($"{providerValues[i].provideCover} from {providerValues[i].provider.rulesProviderName}.");
                            }
                        }

                        if (removerValues.Length > 0)
                        {
                            Console.WriteLine("Cover removers were:");

                            foreach (CoverValue value in removerValues)
                            {
                                Console.WriteLine($"{value.provider.rulesProviderName} (up to {value.removeCoverUpTo}).");
                            }
                        }
                    }
                }
            }

            #endregion

            return cover;
        }
    }
}
