using System;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;

namespace MonsterQuest
{
    public class Damage
    {
        public Damage(Hit hit, DamageRoll roll, int amount)
        {
            this.hit = hit;
            this.roll = roll;
            this.amount = amount;

            type = GetDamageType();
        }

        public Hit hit { get; }
        public DamageRoll roll { get; }
        public int amount { get; }
        public DamageType type { get; }

        private DamageType GetDamageType()
        {
            // Damage type must be either magical or nonmagical.
            DamageType damageType = roll.type;

            if ((damageType & (DamageType.Magical | DamageType.Nonmagical)) == 0)
            {
                // Damage type is magical if it comes from a magic weapon.
                IEnumerable<Effect> parentEffects;

                object attackEffectParent = hit.attack.effect.parent;

                if (attackEffectParent is Item item)
                {
                    parentEffects = item.effects;
                }
                else if (attackEffectParent is Creature creature)
                {
                    parentEffects = creature.effects;
                }
                else
                {
                    throw new ArgumentException("An attack must come either from an item or a creature.");
                }

                bool weaponIsMagical = parentEffects.Any(effect => effect is MagicWeapon);

                damageType |= weaponIsMagical ? DamageType.Magical : DamageType.Nonmagical;
            }

            return damageType;
        }

        public Damage CloneWithAmount(int newAmount)
        {
            return new Damage(hit, roll, newAmount);
        }

        public override string ToString()
        {
            return $"{amount} {roll.type.ToString().ToLowerInvariant()} damage";
        }
    }
}
