using System.Collections;
using System.Linq;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Console
{
    public class DeathSavingThrowEventPresenter : IEventPresenter<DeathSavingThrowEvent>
    {
        public IEnumerator Present(DeathSavingThrowEvent deathSavingThrowEvent)
        {
            string definiteName = deathSavingThrowEvent.creature.definiteName;

            if (deathSavingThrowEvent.rollResult is not null)
            {
                switch (deathSavingThrowEvent.rollResult)
                {
                    case 1:
                        MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} critically fails a death saving throw.");

                        break;

                    case 20:
                        // Critical successes regain consciousness with 1 HP.
                        MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} critically succeeds a death saving throw.");

                        break;

                    case < 10:
                        MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} fails a death saving throw.");

                        break;

                    default:
                        MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} succeeds a death saving throw.");

                        break;
                }

                yield break;
            }

            if (!deathSavingThrowEvent.succeeded)
            {
                switch (deathSavingThrowEvent.amount)
                {
                    case 1:
                        MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} suffers a death saving throw failure.");

                        break;

                    case 2:
                        MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} suffers two death saving throw failures.");

                        break;
                }
            }

            if (deathSavingThrowEvent.deathSavingThrows.Count(deathSavingThrow => deathSavingThrow) == 3)
            {
                MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} succeeded 3 times and they stabilize.");
            }

            yield return null;
        }
    }
}
