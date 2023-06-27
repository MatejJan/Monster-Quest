using System;
using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Narrative
{
    public class LifeStatusEventPresenter : EventPresenter, IEventPresenter<LifeStatusEvent>
    {
        public LifeStatusEventPresenter(IOutput output) : base(output) { }

        public IEnumerator Present(LifeStatusEvent lifeStatusEvent)
        {
            string definiteName = lifeStatusEvent.creature.definiteName;

            switch (lifeStatusEvent.newLifeStatus)
            {
                case LifeStatus.UnconsciousUnstable when lifeStatusEvent.previousLifeStatus == LifeStatus.Conscious:
                    output.WriteLine($"{definiteName.ToUpperFirst()} falls unconscious.");

                    break;

                case LifeStatus.UnconsciousUnstable when lifeStatusEvent.previousLifeStatus == LifeStatus.UnconsciousStable:
                    output.WriteLine($"{definiteName.ToUpperFirst()} destabilizes.");

                    break;

                case LifeStatus.UnconsciousStable when lifeStatusEvent.previousLifeStatus == LifeStatus.Conscious:
                    output.WriteLine($"{definiteName.ToUpperFirst()} gets knocked out.");

                    break;

                case LifeStatus.UnconsciousStable when lifeStatusEvent.previousLifeStatus == LifeStatus.UnconsciousUnstable:
                    output.WriteLine($"{definiteName.ToUpperFirst()} stabilizes.");

                    break;

                case LifeStatus.Dead when lifeStatusEvent.previousLifeStatus == LifeStatus.Conscious:
                    output.WriteLine($"{definiteName.ToUpperFirst()} instantly dies.");

                    break;

                case LifeStatus.Dead:
                    output.WriteLine($"{definiteName.ToUpperFirst()} dies.");

                    break;

                case LifeStatus.Conscious:
                    output.WriteLine($"{definiteName.ToUpperFirst()} regains consciousness.");

                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            yield return null;
        }
    }
}
