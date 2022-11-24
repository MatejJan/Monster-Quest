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
        [field: SerializeField] public LifeStatus lifeStatus { get; protected set; }

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

        public void GiveItem(Item item)
        {
            itemsList.Add(item);
        }

        public virtual IAction TakeTurn(GameState gameState)
        {
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
            DebugHelper.StartLog($"{definiteName.ToUpperFirst()} is making a DC {successAmount} {ability} check …");

            int roll = DiceHelper.Roll("d20");
            int abilityModifier = abilityScores[ability].modifier;
            bool result = roll + abilityModifier >= successAmount;

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

        public IEnumerator Heal(int amount)
        {
            hitPoints = Math.Min(hitPoints + amount, hitPointsMaximum);

            Console.Write($"{definiteName.ToUpperFirst()} heals {amount} HP and ");

            if (lifeStatus == LifeStatus.Alive)
            {
                Console.WriteLine($"is at {(hitPoints == hitPointsMaximum ? "full health" : $"{hitPoints} HP")}");
            }
            else
            {
                lifeStatus = LifeStatus.Alive;

                Console.WriteLine($"regains consciousness. They are at {(hitPoints == hitPointsMaximum ? "full health" : $"{hitPoints} HP")}");
            }

            if (presenter is not null) yield return presenter.Heal();
        }

        protected abstract IEnumerator TakeDamageAtZeroHP(int remainingDamageAmount, Hit hit);
    }
}
