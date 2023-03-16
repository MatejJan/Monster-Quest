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
        
        public MonsterType type { get; }
        public override AbilityScores abilityScores => type.abilityScores;
        public override IEnumerable<bool> deathSavingThrows => _deathSavingThrows;
        public override int armorClass => type.armorClass;

        public override IAction TakeTurn(GameState gameState)
        {
            // Attack a random character with a random weapon.
            WeaponType weaponType = type.weaponTypes.Random();

            Character target;

            if (abilityScores.intelligence > 7)
            {
                target = gameState.party.aliveCharacters.OrderBy(character => character.hitPoints).First();
            }
            else
            {
                target = gameState.party.aliveCharacters.Random();
            }

            return CreateAttack(target, weaponType);
        }
    }
}
