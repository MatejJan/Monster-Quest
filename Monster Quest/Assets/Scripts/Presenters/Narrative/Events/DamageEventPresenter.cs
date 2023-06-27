using System.Collections;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Narrative
{
    public class DamageEventPresenter : EventPresenter, IEventPresenter<DamageEvent>
    {
        public DamageEventPresenter(IOutput output) : base(output) { }

        public IEnumerator Present(DamageEvent damageEvent)
        {
            string definiteName = damageEvent.creature.definiteName;

            foreach (DamageEvent.DamageAmountResult damageAmountResult in damageEvent.damageAmountResults)
            {
                DamageAmount damageAmount = damageAmountResult.damageAmount;
                DamageAmount modifiedDamageAmount = damageAmountResult.modifiedDamageAmount;
                DamageAmount finalDamageAmount = damageAmountResult.finalDamageAmount;
                DamageAlteration damageAlteration = damageAmountResult.damageAlteration;

                if (damageAmountResult.savingThrowResult is not null)
                {
                    if (damageAmountResult.savingThrowResult.success)
                    {
                        output.WriteLine($"{definiteName.ToUpperFirst()} is hit with {damageAmount} and succeeds on a saving throw to halve the damage.");
                    }
                    else
                    {
                        output.WriteLine($"{definiteName.ToUpperFirst()} is hit with {damageAmount} and fails on a saving throw to resist it.");
                    }
                }

                string text = "";

                if (damageAmount.roll.savingThrowAbility != Ability.None)
                {
                    text += $"{definiteName.ToUpperFirst()} receives {modifiedDamageAmount}";
                }
                else
                {
                    text += $"{definiteName.ToUpperFirst()} is hit with {modifiedDamageAmount}";
                }

                if (damageAlteration.immunity)
                {
                    text += " but is immune and takes no damage.";
                }
                else if (damageAlteration.resistance && damageAlteration.vulnerability)
                {
                    text += $". {definiteName.ToUpperFirst()} is both resistant and vulnerable to it and takes {finalDamageAmount.value} damage.";
                }
                else if (damageAlteration.resistance)
                {
                    text += $" but due to their resistance only takes {finalDamageAmount.value} damage.";
                }
                else if (damageAlteration.vulnerability)
                {
                    text += $" but due to their vulnerability takes {finalDamageAmount.value} damage.";
                }
                else
                {
                    text += ".";
                }

                output.WriteLine(text);
            }

            if (damageEvent.hitPointsEnd > 0)
            {
                output.WriteLine($"{definiteName.ToUpperFirst()} has {damageEvent.hitPointsEnd} HP left.");
            }

            yield return null;
        }
    }
}
