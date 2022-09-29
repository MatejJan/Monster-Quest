using System.Linq;
using MonsterQuest.Effects;
using Attack = MonsterQuest.Actions.Attack;

namespace MonsterQuest
{
    public abstract partial class Creature : IAttackRollMethodRule
    {
        public MultipleValue<AttackRollMethod> GetAttackRollMethod(Attack attack)
        {
            // Only provide information for the current attacker.
            if (attack.attacker != this) return null;

            // Attacker has a disadvantage if they are wearing armor (including a shield) they are not proficient in.
            Item[] armorItems = items.Where(item => item.GetEffect<Armor>() != null).ToArray();

            DebugHelper.StartLog("Determining armor proficiency … ");
            ArmorCategory[] proficientArmorCategories = attack.battle.GetRuleValues((IArmorProficiencyRule rule) => rule.GetArmorProficiency(this)).Resolve();
            DebugHelper.EndLog();

            foreach (Item armorItem in armorItems)
            {
                // The attacker must be proficient in the armor category to avoid the disadvantage.
                ArmorType armorType = armorItem.GetEffect<Armor>().armorType;

                if (!proficientArmorCategories.Contains(armorType.category))
                {
                    return new MultipleValue<AttackRollMethod>(this, AttackRollMethod.Disadvantage);
                }
            }

            return null;
        }
    }
}
