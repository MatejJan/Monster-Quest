using System;
using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Console
{
    public class LifeStatusEventPresenter : IEventPresenter<LifeStatusEvent>
    {
        public IEnumerator Present(LifeStatusEvent lifeStatusEvent)
        {
            string definiteName = lifeStatusEvent.creature.definiteName;

            switch (lifeStatusEvent.newLifeStatus)
            {
                case LifeStatus.UnconsciousUnstable when lifeStatusEvent.previousLifeStatus == LifeStatus.Conscious:
                    MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} falls unconscious.");

                    break;

                case LifeStatus.UnconsciousUnstable when lifeStatusEvent.previousLifeStatus == LifeStatus.UnconsciousStable:
                    MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} destabilizes.");

                    break;

                case LifeStatus.UnconsciousStable when lifeStatusEvent.previousLifeStatus == LifeStatus.Conscious:
                    MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} gets knocked out.");

                    break;

                case LifeStatus.UnconsciousStable when lifeStatusEvent.previousLifeStatus == LifeStatus.UnconsciousUnstable:
                    MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} stabilizes.");

                    break;

                case LifeStatus.Dead when lifeStatusEvent.previousLifeStatus == LifeStatus.Conscious:
                    MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} instantly dies.");

                    break;

                case LifeStatus.Dead:
                    MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} dies.");

                    break;

                case LifeStatus.Conscious:
                    MonsterQuest.Console.WriteLine($"{definiteName.ToUpperFirst()} regains consciousness.");

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return null;
        }
    }
}
