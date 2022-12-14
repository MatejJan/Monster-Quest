using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Effects;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public abstract partial class Creature : IRulesHandler, IRulesProvider
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

            abilityScores = new AbilityScores();
        }

        // State properties

        [field: SerializeField] public AbilityScores abilityScores { get; protected set; }
        [field: SerializeField] public int hitPointsMaximum { get; protected set; }
        [field: SerializeField] public string displayName { get; protected set; }

        [field: SerializeField] public int hitPoints { get; protected set; }

        public LifeStatus lifeStatus
        {
            get => _lifeStatus;
            protected set
            {
                _lifeStatus = value;
                if (presenter is not null) presenter.UpdateStableStatus();
            }
        }

        public CreaturePresenter presenter { get; private set; }

        // Derived properties

        public abstract SizeCategory sizeCategory { get; }
        public float spaceInFeet => SizeHelper.spaceInFeetPerSizeCategory[sizeCategory];

        public IEnumerable<Effect> effects => effectsList;
        public IEnumerable<Item> items => itemsList;

        public string definiteName => EnglishHelper.GetDefiniteNounForm(displayName);
        public string indefiniteName => EnglishHelper.GetIndefiniteNounForm(displayName);

        public abstract Sprite bodySprite { get; }
        public abstract float flyHeight { get; }

        public int proficiencyBonus => 2 + Math.Max(0, (proficiencyBonusBase - 1) / 4);
        protected abstract int proficiencyBonusBase { get; }

        public bool isUnconscious => lifeStatus is LifeStatus.StableUnconscious or LifeStatus.UnstableUnconscious;
        public abstract bool[] deathSavingThrows { get; }
        public int deathSavingThrowSuccesses => deathSavingThrows.Count(result => result);
        public int deathSavingThrowFailures => deathSavingThrows.Count(result => !result);

        public IEnumerable<object> rules =>
            new object[]
            {
                this
            }.Concat(effects).Concat(items.SelectMany(item => item.rules));

        public string rulesProviderName => indefiniteName;

        // Methods

        protected void Initialize()
        {
            hitPoints = hitPointsMaximum;
        }

        public void InitializePresenter(CreaturePresenter creaturePresenter)
        {
            presenter = creaturePresenter;
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

            Attack attackEffect = RandomHelper.Element(attackEffects);
            Item attackItem = null;
            Ability? attackAbility = null;

            // See if the effect comes from an item.
            if (attackEffect.parent is Item parentItem)
            {
                attackItem = parentItem;

                // If the weapon has a finesse property, use the higher of the two ability scores.
                if (parentItem.GetEffect<Finesse>() is not null)
                {
                    attackAbility = abilityScores.strength > abilityScores.dexterity ? Ability.Strength : Ability.Dexterity;
                }
            }

            // Choose a random target.
            IEnumerable<Creature> hostileCreatures = gameState.combat.GetCreatures().Where(creature => gameState.combat.AreHostile(this, creature));
            Creature target = RandomHelper.Element(hostileCreatures);

            return new AttackAction(gameState, this, target, attackEffect, attackItem, attackAbility);
        }

        public bool MakeAbilityCheck(Ability ability, int successAmount)
        {
            return MakeAbilityCheck(ability, successAmount, out _);
        }

        public bool MakeAbilityCheck(Ability ability, int successAmount, out int rollResult)
        {
            DebugHelper.StartLog($"{definiteName.ToUpperFirst()} is making a DC {successAmount} {ability} check ???");

            rollResult = DiceHelper.Roll("d20");
            int abilityModifier = abilityScores[ability].modifier;
            bool result = rollResult + abilityModifier >= successAmount;

            DebugHelper.EndLog($"The check {(result ? "succeeds" : "fails")}.");

            return result;
        }

        public bool MakeSavingThrow(Ability ability, int successAmount)
        {
            return MakeAbilityCheck(ability, successAmount);
        }

        public IEnumerator Die()
        {
            hitPoints = 0;
            lifeStatus = LifeStatus.Dead;

            Console.WriteLine($"{definiteName.ToUpperFirst()} dies.");

            if (presenter is not null) yield return presenter.Die();
        }

        public virtual IEnumerator Heal(int amount)
        {
            hitPoints = Math.Min(hitPoints + amount, hitPointsMaximum);

            Console.Write($"{definiteName.ToUpperFirst()} heals {amount} HP and ");

            if (lifeStatus == LifeStatus.Alive)
            {
                Console.WriteLine($"is at {(hitPoints == hitPointsMaximum ? "full health" : $"{hitPoints} HP")}.");
            }
            else
            {
                lifeStatus = LifeStatus.Alive;

                Console.WriteLine($"regains consciousness. They are at {(hitPoints == hitPointsMaximum ? "full health" : $"{hitPoints} HP")}.");

                if (presenter is not null) yield return presenter.RegainConsciousness();
            }

            if (presenter is not null) yield return presenter.Heal();
        }

        protected abstract IEnumerator TakeDamageAtZeroHP(int remainingDamageAmount, Hit hit);
    }
}
