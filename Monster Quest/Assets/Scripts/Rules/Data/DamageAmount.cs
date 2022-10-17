using System;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;

namespace MonsterQuest
{
    public class DamageAmount
    {
        public DamageAmount(Hit hit, DamageRoll roll, int value)
        {
            this.hit = hit;
            this.roll = roll;
            this.value = value;

            SetDamageType();
        }

        public Hit hit { get; }
        public DamageRoll roll { get; }
        public int value { get; }
        public DamageType type { get; private set; }

        public DamageAmount CloneWithValue(int newValue)
        {
            return new DamageAmount(hit, roll, newValue);
        }

        private void SetDamageType()
        {
            // Damage type must be either magical or nonmagical.
            type = roll.type;

            if ((type & (DamageType.Magical | DamageType.Nonmagical)) != 0) return;

            // Damage type is magical if it comes from a magic weapon.
            IEnumerable<Effect> parentEffects;

            object attackEffectParent = hit.attackAction.effect.parent;

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

            type |= weaponIsMagical ? DamageType.Magical : DamageType.Nonmagical;
        }

        public override string ToString()
        {
            return $"{value} {roll.type.ToString().ToLowerInvariant()} damage";
        }
    }
}
