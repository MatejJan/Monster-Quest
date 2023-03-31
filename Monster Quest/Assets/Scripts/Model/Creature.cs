using System;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;
using MonsterQuest.Events;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public abstract partial class Creature : IRulesHandler, IRulesProvider, IStateEventProvider
    {
        // Fields

        [SerializeReference] protected List<Effect> effectsList;
        [SerializeReference] protected List<Item> itemsList;
        [SerializeField] private LifeStatus _lifeStatus;

        // Constructor

        protected Creature()
        {
            effectsList = new List<Effect>();
            itemsList = new List<Item>();
        }

        // State properties

        [field: SerializeField] public string displayName { get; protected set; }

        [field: SerializeField] public int hitPoints { get; protected set; }
        [field: SerializeField] public int hitPointsMaximum { get; protected set; }

        public LifeStatus lifeStatus
        {
            get => _lifeStatus;
            protected set
            {
                LifeStatusEvent lifeStatusEvent = new()
                {
                    creature = this,
                    previousLifeStatus = _lifeStatus,
                    newLifeStatus = value
                };

                _lifeStatus = value;

                ReportStateEvent(lifeStatusEvent);
            }
        }

        // Derived properties

        public abstract AbilityScores abilityScores { get; }
        public abstract SizeCategory sizeCategory { get; }
        public float spaceInFeet => SizeHelper.spaceInFeetPerSizeCategory[sizeCategory];

        public IEnumerable<Effect> effects => effectsList;
        public IEnumerable<Item> items => itemsList;

        public string definiteName => EnglishHelper.GetDefiniteNounForm(displayName);
        public string indefiniteName => EnglishHelper.GetIndefiniteNounForm(displayName);

        public abstract Sprite bodySprite { get; }
        public abstract float flyHeight { get; }

        public int proficiencyBonus => CreatureRules.GetProficiencyBonus(proficiencyBonusBase);
        protected abstract int proficiencyBonusBase { get; }

        public bool isAlive => lifeStatus is not LifeStatus.Dead;
        public bool isUnconscious => lifeStatus is LifeStatus.UnconsciousStable or LifeStatus.UnconsciousUnstable;

        public abstract IEnumerable<bool> deathSavingThrows { get; }
        public int deathSavingThrowSuccesses => deathSavingThrows.Count(result => result);
        public int deathSavingThrowFailures => deathSavingThrows.Count(result => !result);

        public IEnumerable<object> rules =>
            new object[]
            {
                this
            }.Concat(effects).Concat(items.SelectMany(item => item.rules));

        public string rulesProviderName => indefiniteName;

        // Events 

        [field: NonSerialized] public event Action<object> stateEvent;

        // Methods

        protected void ReportStateEvent(object eventData)
        {
            stateEvent?.Invoke(eventData);
        }

        protected void Initialize()
        {
            hitPoints = hitPointsMaximum;
        }

        public void GiveItem(GameState gameState, Item item)
        {
            gameState.MutateRules(() => itemsList.Add(item));
        }

        public void RemoveItem(GameState gameState, Item item)
        {
            gameState.MutateRules(() => itemsList.Remove(item));
        }

        public virtual IAction TakeTurn(GameState gameState)
        {
            // If you have less than half health, use any available healing items.
            if (hitPoints < hitPointsMaximum / 2)
            {
                Item healingItem = items.FirstOrDefault(item => item.HasEffect<HealingItem>());

                if (healingItem is not null)
                {
                    return new UseItemAction(gameState, this, healingItem);
                }
            }

            // Perform an attack with a random attack.
            List<Attack> attackEffects = new();

            foreach (Item item in items)
            {
                attackEffects.AddRange(item.GetEffects<Attack>());
            }

            attackEffects.AddRange(effects.OfType<Attack>());

            // See if we have any attacks, otherwise we can't do anything.
            if (attackEffects.Count == 0) return null;

            Attack attackEffect = attackEffects.Random();
            Item attackItem = null;
            Ability? attackAbility = null;

            // See if the effect comes from an item.
            if (attackEffect.parent is Item parentItem)
            {
                attackItem = parentItem;

                // If the weapon has a finesse property, use the higher of the two ability scores.
                if (parentItem.GetEffect<Finesse>() is not null)
                {
                    attackAbility = abilityScores.strength.score > abilityScores.dexterity.score ? Ability.Strength : Ability.Dexterity;
                }
            }

            // Attack a target.
            Creature target;
            IEnumerable<Creature> hostileCreatures = gameState.combat.creatures.Where(creature => creature.isAlive && gameState.combat.AreHostile(this, creature));

            if (abilityScores.intelligence >= 8)
            {
                // Smart creatures attack the hostile with the lowest hit points.
                target = hostileCreatures.OrderBy(creature => creature.hitPoints).First();
            }
            else
            {
                // Others attack randomly.
                target = hostileCreatures.Random();
            }

            return new AttackAction(gameState, this, target, attackEffect, attackItem, attackAbility);
        }

        public bool MakeAbilityCheck(Ability ability, int successAmount)
        {
            return MakeAbilityCheck(ability, successAmount, out _);
        }

        public bool MakeAbilityCheck(Ability ability, int successAmount, out RollResult rollResult)
        {
            DebugHelper.StartLog($"{definiteName.ToUpperFirst()} is making a DC {successAmount} {ability} check …");

            rollResult = new RollResult($"d20{abilityScores[ability].modifier:+#;-#}");
            bool result = rollResult.result >= successAmount;

            DebugHelper.EndLog($"The check {(result ? "succeeds" : "fails")}.");

            return result;
        }

        public RollResult MakeAbilityRoll(Ability ability)
        {
            DebugHelper.StartLog($"{definiteName.ToUpperFirst()} is making a {ability} roll …");

            RollResult rollResult = new($"d20{abilityScores[ability].modifier:+#;-#}");

            DebugHelper.EndLog($"They roll a {rollResult.rolls[0]} for a total of {rollResult.result}.");

            return rollResult;
        }

        public bool MakeSavingThrow(Ability ability, int successAmount)
        {
            return MakeAbilityCheck(ability, successAmount);
        }

        public bool MakeSavingThrow(Ability ability, int successAmount, out RollResult rollResult)
        {
            return MakeAbilityCheck(ability, successAmount, out rollResult);
        }

        public void Die()
        {
            hitPoints = 0;
            lifeStatus = LifeStatus.Dead;
        }

        public virtual void Heal(int amount)
        {
            // Increase hit points.
            HealEvent healEvent = new()
            {
                creature = this,
                hitPointsStart = hitPoints,
                hitPointsMaximum = hitPointsMaximum,
                amount = amount
            };

            hitPoints = Math.Min(hitPoints + amount, hitPointsMaximum);

            healEvent.hitPointsEnd = hitPoints;

            ReportStateEvent(healEvent);

            // Regain consciousness if needed.
            if (lifeStatus != LifeStatus.Conscious)
            {
                lifeStatus = LifeStatus.Conscious;
            }
        }

        protected abstract void TakeDamageAtZeroHitPoints(int remainingDamageAmount, Hit hit);
    }
}
