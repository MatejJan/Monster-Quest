using System;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Actions;
using MonsterQuest.Effects;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public partial class Character : Creature, IArmorClassRule
    {
        [SerializeField] private Sprite _bodySprite;

        public Character(string displayName, Race race, ClassType classType, Sprite bodySprite)
        {
            this.displayName = displayName;
            DebugHelpers.StartLog($"Creating {definiteName}.");

            this.race = race;

            Class characterClass = classType.Create(this) as Class;
            effectsList.Add(characterClass);

            _bodySprite = bodySprite;

            // Generate possible ability scores.
            DebugHelpers.StartLog("Rolling ability scores …");
            List<int> possibleAbilityScores = new();

            for (int i = 0; i < 6; i++)
            {
                // Roll 4d6 and remove the lowest dice.
                List<int> rolls = new();

                for (int j = 0; j < 4; j++)
                {
                    rolls.Add(Dice.Roll("d6"));
                }

                rolls.Sort();
                rolls.RemoveAt(0);
                possibleAbilityScores.Add(rolls.Sum());
            }

            // Randomly assign ability scores.
            possibleAbilityScores.Shuffle();

            for (int i = 0; i < 6; i++)
            {
                abilityScores[(Ability)(i + 1)].score = possibleAbilityScores[i];
                DebugHelpers.Log($"{(Ability)(i + 1)}: {possibleAbilityScores[i]}");
            }

            DebugHelpers.EndLog();

            // Calculate hit points at first level.
            hitPointsMaximum = classType.hitPointsBase + abilityScores.constitution.modifier;
            hitPoints = hitPointsMaximum;

            DebugHelpers.EndLog($"Created {definiteName} with {hitPointsMaximum} HP.");
        }

        // State properties
        [field: SerializeField] public Race race { get; private set; }
        [field: SerializeField] public int level { get; private set; }

        // Derived properties
        public override SizeCategory size => race.size;

        public override Sprite bodySprite => _bodySprite;
        public override float flyHeight => 0;

        protected override int proficiencyBonusBase => level;

        public IntegerValue GetArmorClass(Creature creature)
        {
            // Only provide information for the current character.
            if (creature != this) return null;

            // Characters have a base armor class of 10.
            return new IntegerValue(this, 10);
        }

        public override IAction TakeTurn(GameState gameState)
        {
            // An unconscious character must perform a death saving throw action.
            if (lifeStatus != LifeStatus.Alive)
            {
                return new BeUnconscious(this);
            }

            return base.TakeTurn(gameState);
        }
    }
}
