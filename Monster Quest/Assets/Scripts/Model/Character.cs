using System;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;
using MonsterQuest.Events;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public partial class Character : Creature, IArmorClassRule
    {
        [SerializeField] private Sprite _bodySprite;
        [SerializeField] private List<bool> _deathSavingThrows;
        [SerializeField] private AbilityScores _abilityScores;

        public Character(string displayName, RaceType raceType, ClassType classType, Sprite bodySprite, int startingLevel = 1)
        {
            this.displayName = displayName;
            DebugHelper.StartLog($"Creating {definiteName}.");

            race = raceType.Create(this) as Race;
            effectsList.Add(race);

            _bodySprite = bodySprite;

            // Generate ability scores.
            DebugHelper.StartLog("Rolling ability scores …");
            _abilityScores = new AbilityScores();

            for (int i = 1; i <= 6; i++)
            {
                // Roll 4d6 and remove the lowest dice.
                List<int> rolls = new();

                for (int j = 0; j < 4; j++)
                {
                    rolls.Add(DiceHelper.Roll("d6"));
                }

                rolls.Sort();
                rolls.RemoveAt(0);
                int score = rolls.Sum();

                Ability ability = (Ability)i;
                _abilityScores[ability].score = score;
                DebugHelper.Log($"{ability}: {score}");
            }

            DebugHelper.EndLog();

            // Calculate hit points at first level.
            hitPointsMaximum = classType.hitPointsBase + _abilityScores.constitution.modifier;

            // Create character class at starting level.
            characterClass = classType.Create(this, startingLevel, out int hitPointsMaximumIncrease) as Class;
            effectsList.Add(characterClass);

            hitPointsMaximum += hitPointsMaximumIncrease;

            experiencePoints = CharacterRules.GetExperiencePointsForLevel(startingLevel);

            // Prepare death saving throws.
            _deathSavingThrows = new List<bool>();

            // Finalize creature initialization.
            Initialize();

            DebugHelper.EndLog($"Created {definiteName} with {hitPointsMaximum} HP.");
        }

        // State properties
        [field: SerializeReference] public Class characterClass { get; private set; }
        [field: SerializeReference] public Race race { get; private set; }
        [field: SerializeField] public int experiencePoints { get; private set; }

        // Derived properties
        public override AbilityScores abilityScores => _abilityScores;
        public override SizeCategory sizeCategory => race.raceType.sizeCategory;

        public override Sprite bodySprite => _bodySprite;
        public override float flyHeight => 0;

        protected override int proficiencyBonusBase => characterClass.level;

        public override IEnumerable<bool> deathSavingThrows => _deathSavingThrows;

        // Public methods

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
            if (lifeStatus != LifeStatus.Conscious)
            {
                return new BeUnconsciousAction(this);
            }

            // See if there are any unconscious party members and take the one with the most death saving throw failures.
            Character unconsciousCharacter = gameState.party.characters.Where(character => character.isUnconscious).OrderByDescending(character => character.deathSavingThrowFailures).FirstOrDefault();

            // If you have any available healing items, administer them.
            if (unconsciousCharacter is not null)
            {
                Item healingItem = items.FirstOrDefault(item => item.HasEffect<HealingItem>());

                if (healingItem is not null)
                {
                    return new UseItemAction(gameState, this, healingItem, unconsciousCharacter);
                }
            }

            // See if there are any unstable party members and take the one with the most death saving throw failures.
            Character unstableCharacter = gameState.party.characters.Where(character => character.lifeStatus == LifeStatus.UnconsciousUnstable).OrderByDescending(character => character.deathSavingThrowFailures).FirstOrDefault();

            // Attempt to stabilize them if our wisdom is at least average.
            if (unstableCharacter?.deathSavingThrowFailures > 0 && abilityScores.wisdom >= 10)
            {
                return new StabilizeCharacterAction(this, unstableCharacter);
            }

            return base.TakeTurn(gameState);
        }

        public void TakeShortRest()
        {
            while (hitPoints <= hitPointsMaximum * 0.75f && characterClass.availableHitDice > 0)
            {
                characterClass.SpendHitDice();
            }
        }

        public void GainExperiencePoints(int amount)
        {
            experiencePoints += amount;

            ReportStateEvent(new GainExperienceEvent
            {
                character = this,
                amount = amount
            });

            // Determine new level based on new experience points.
            int newLevel = CharacterRules.GetLevelForExperiencePoints(experiencePoints);

            while (characterClass.level < newLevel)
            {
                LevelUp();
            }
        }

        private void LevelUp()
        {
            characterClass.LevelUp(out int hitPointsMaximumIncrease);
            hitPointsMaximum += hitPointsMaximumIncrease;

            ReportStateEvent(new LevelUpEvent
            {
                character = this
            });
        }
    }
}
