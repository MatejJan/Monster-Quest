using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Actions;
using MonsterQuest.Effects;
using UnityEngine;

namespace MonsterQuest
{
    public partial class Character : Creature, IArmorClassRule
    {
        public Character(string name, Race race, ClassType classType, Sprite bodySprite)
        {
            this.name = name;
            DebugHelper.StartLog($"Creating {definiteName}.");

            this.race = race;

            Class characterClass = classType.Create(this) as Class;
            effectsList.Add(characterClass);

            this.bodySprite = bodySprite;

            // Generate possible ability scores.
            DebugHelper.StartLog("Rolling ability scores …");
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
                DebugHelper.Log($"{(Ability)(i + 1)}: {possibleAbilityScores[i]}");
            }

            DebugHelper.EndLog();

            // Calculate hit points at first level.
            hitPointsMaximum = classType.hitPointsBase + abilityScores.constitution.modifier;
            hitPoints = hitPointsMaximum;

            DebugHelper.EndLog($"Created {definiteName} with {hitPointsMaximum} HP.");
        }

        public Race race { get; }

        public override SizeCategory size => race.size;

        public override Sprite bodySprite { get; }

        [field: SerializeField] public int level { get; private set; }

        protected override int proficiencyBonusBase => level;

        public IntegerValue GetArmorClass(Creature creature)
        {
            // Only provide information for the current character.
            if (creature != this) return null;

            // Characters have a base armor class of 10.
            return new IntegerValue(this, 10);
        }

        public override IAction TakeTurn(Battle battle, Creature target)
        {
            // An unconscious character must perform a death saving throw action.
            if (lifeStatus != LifeStatus.Alive)
            {
                return new BeUnconscious(battle, this);
            }

            return base.TakeTurn(battle, target);
        }
    }
}
