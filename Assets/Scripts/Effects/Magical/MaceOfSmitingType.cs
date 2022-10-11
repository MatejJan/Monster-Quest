using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Mace of Smithing", menuName = "Effects/Magical/Mace of Smithing")]
    public class MaceOfSmitingType : EffectType
    {
        public override Effect Create(object parent)
        {
            return new MaceOfSmiting(this, parent);
        }
    }

    public class MaceOfSmiting : Effect, IAttackRollModifierRule, IDamageRollModifierRule, IDamageRollRule, IDamageRule
    {
        private DamageRoll _lastDamageRoll;

        public MaceOfSmiting(EffectType type, object parent) : base(type, parent) { }

        public IntegerValue GetAttackRollModifier(Actions.Attack attack)
        {
            return GetRollModifier(attack);
        }

        public IntegerValue GetDamageRollModifier(Actions.Attack attack)
        {
            return GetRollModifier(attack);
        }

        public ArrayValue<DamageRoll> GetDamageRolls(Hit hit)
        {
            // Only provide information to attacks with this weapon.
            if (hit.attack.weapon != parent) return null;

            // When you roll a 20 on an attack roll made with this weapon, the target takes an extra 2d6 bludgeoning damage, or 4d6 bludgeoning damage if it’s a construct.
            if (!hit.wasCritical) return null;

            // Create the roll and store it so we can apply extra consequences after it has been dealt.
            string roll = IsTargetAConstruct(hit.attack.target) ? "4d6" : "2d6";
            _lastDamageRoll = new DamageRoll(roll, DamageType.Bludgeoning, true);

            return new ArrayValue<DamageRoll>(this, new[] { _lastDamageRoll });
        }

        public void ReactToDamageDealt(DamageAmount damageAmount)
        {
            // React only to extra damage dealt with this weapon.
            if (damageAmount.roll != _lastDamageRoll) return;

            // If a construct has 25 hit points or fewer after taking this damage, it is destroyed.
            Creature target = damageAmount.hit.target;

            if (IsTargetAConstruct(target) && target.hitPoints <= 25)
            {
                Console.WriteLine($"{(parent as Item)?.type.definiteName.ToUpperFirst()} deals a fatal blow and shatters the construct to pieces.");

                target.Die();
            }
        }

        private IntegerValue GetRollModifier(Actions.Attack attack)
        {
            // Only provide information to attacks with this weapon.
            if (attack.weapon != parent) return null;

            // You gain a +1 bonus to attack and damage rolls made with this magic weapon. The bonus increases to +3 when you use the mace to attack a construct.
            return new IntegerValue(this, modifierValue: IsTargetAConstruct(attack.target) ? 3 : 1);
        }

        private bool IsTargetAConstruct(Creature target)
        {
            Monster monster = target as Monster;

            if (monster == null) return false;

            return monster.type.type == MonsterType.TypeCategory.Construct;
        }
    }
}
