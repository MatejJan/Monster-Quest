using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MonsterQuest
{
    public class AttackAction : IAction
    {
        private Creature _attacker;
        private Creature _target;
        private WeaponType _weaponType;

        public AttackAction(Creature attacker, Creature target, WeaponType weaponType)
        {
            _attacker = attacker;
            _target = target;
            _weaponType = weaponType;
        }
        
        public IEnumerator Execute()
        {
            // Face the target.
            yield return _attacker.presenter.FaceCreature(_target);
            yield return _attacker.presenter.Attack();

            // Determine whether the attack is a hit or a miss.
            bool wasHit = false;
            bool wasCritical = false;

            // Attacks on unconscious targets is always a critical hit.
            if (_target.isUnconscious)
            {
                wasHit = true;
                wasCritical = true;
            }
            else
            {
                // Perform an attack roll.
                int attackRoll = DiceHelper.Roll("d20");

                // The attack always misses on a critical miss.
                if (attackRoll == 1)
                {
                    wasCritical = true;
                }
                // The attack always hits on a critical hit.
                else if (attackRoll == 20)
                {
                    wasHit = true;
                    wasCritical = true;
                }
                // Otherwise the attack value must be greater than or equal to the target's armor class.
                else
                {
                    // Determine the target's armor class.
                    int armorClass = _target.armorClass;

                    // Determine result.
                    wasHit = attackRoll >= armorClass;
                }
            }
            
            // End the attack if it was a miss.
            if (!wasHit)
            {
                // Describe the outcome of the attack.
                Console.WriteLine($"{_attacker.displayName} attacks {_target.displayName} with {_weaponType.displayName} but misses.");

                yield break;
            }
           
            // Roll the damage.
            int damageAmount = DiceHelper.Roll(_weaponType.damageRoll);

            // Roll twice for critical hits.
            if (wasCritical)
            {
                damageAmount += DiceHelper.Roll(_weaponType.damageRoll);
            }
            
            // Describe the outcome of the attack.
            Console.WriteLine($"The {_attacker.displayName} hits {_target.displayName} with {_weaponType.displayName} for {damageAmount} damage.");
            
            // Apply the damage.
            yield return _target.ReactToDamage(damageAmount);
        }
    }
}
