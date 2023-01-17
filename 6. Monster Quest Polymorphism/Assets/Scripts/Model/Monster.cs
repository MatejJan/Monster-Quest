using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace MonsterQuest
{
    public class Monster : Creature
    {
        private static readonly bool[] _deathSavingThrows = Array.Empty<bool>();
        
        public Monster(MonsterType type) : base(type.displayName, type.bodySprite, type.sizeCategory)
        {
            this.type = type;

            hitPointsMaximum = DiceHelper.Roll(type.hitPointsRoll);
            
            Initialize();
        }
        
        public MonsterType type { get; private set; }
        public override IEnumerable<bool> deathSavingThrows => _deathSavingThrows;

        public override int armorClass => type.armorClass;

        public override IAction TakeTurn(GameState gameState)
        {
            // Attack a random character with a random weapon.
            WeaponType weaponType = type.weaponTypes[Random.Range(0, type.weaponTypes.Length)];
            
            Character target = gameState.party.aliveCharacters.ToArray()[Random.Range(0, gameState.party.aliveCount)];

            return new AttackAction(this, target, weaponType);
        }
    }
}
