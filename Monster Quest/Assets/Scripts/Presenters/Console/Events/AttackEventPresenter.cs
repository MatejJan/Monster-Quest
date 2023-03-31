using System.Collections;
using MonsterQuest.Effects;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Console
{
    public class AttackEventPresenter : IEventPresenter<AttackEvent>
    {
        public IEnumerator Present(AttackEvent attackEvent)
        {
            // Describe the outcome of the attack.
            string descriptionVerb;

            Creature attacker = attackEvent.attackAction.attacker;
            Creature target = attackEvent.attackAction.target;
            Attack effect = attackEvent.attackAction.effect;
            Item weapon = attackEvent.attackAction.weapon;

            if (!string.IsNullOrEmpty(effect.attackType.descriptionVerb))
            {
                descriptionVerb = effect.attackType.descriptionVerb;
            }
            else
            {
                descriptionVerb = effect is RangedAttack ? "shoots" : "attacks";
            }

            string text = $"{attacker.definiteName.ToUpperFirst()} {descriptionVerb} ";

            string descriptionObject = string.IsNullOrEmpty(effect.attackType.descriptionObject) ? weapon?.indefiniteName : effect.attackType.descriptionObject;

            if (effect is RangedAttack)
            {
                if (descriptionObject is not null)
                {
                    text += $"{descriptionObject} ";
                }

                text += $"at {target.definiteName} ";
            }
            else
            {
                text += $"{target.definiteName} ";

                if (descriptionObject is not null)
                {
                    text += $"with {descriptionObject} ";
                }
            }

            text += $"and {(attackEvent.wasHit ? $"{(attackEvent.wasCritical ? "gets a critical hit!" : "hits.")}" : $"{(attackEvent.wasCritical ? "gets a critical miss" : "misses")}.")}";

            MonsterQuest.Console.WriteLine(text);

            yield return null;
        }
    }
}
