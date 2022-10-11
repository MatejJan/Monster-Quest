using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Actions;
using MonsterQuest.Controllers;
using MonsterQuest.Effects;
using UnityEngine;
using Attack = MonsterQuest.Effects.Attack;

namespace MonsterQuest
{
    [Serializable]
    public abstract partial class Creature : IRulesHandler, IRulesProvider
    {
        public enum LifeStatus
        {
            Alive,
            StableUnconscious,
            UnstableUnconscious,
            Dead
        }

        public enum SizeCategory
        {
            None,
            Tiny,
            Small,
            Medium,
            Large,
            Huge,
            Gargantuan
        }

        public static Dictionary<SizeCategory, float> spaceTakenPerSize;

        protected readonly List<Effect> effectsList = new();
        protected readonly List<Item> itemsList = new();

        static Creature()
        {
            // Define how much space each size category takes up.
            spaceTakenPerSize = new Dictionary<SizeCategory, float>
            {
                { SizeCategory.Tiny, 2.5f },
                { SizeCategory.Small, 5f },
                { SizeCategory.Medium, 5f },
                { SizeCategory.Large, 10f },
                { SizeCategory.Huge, 15f },
                { SizeCategory.Gargantuan, 20f }
            };
        }

        protected Creature()
        {
            abilityScores = new AbilityScores();
        }

        // State properties.
        public AbilityScores abilityScores { get; }
        [field: SerializeField] public int hitPointsMaximum { get; protected set; }
        [field: SerializeField] public string name { get; protected set; }

        [field: SerializeField] public int hitPoints { get; protected set; }
        [field: SerializeField] public LifeStatus lifeStatus { get; protected set; }

        // Derived properties.
        public abstract SizeCategory size { get; }
        public float spaceTaken => spaceTakenPerSize[size];

        public IEnumerable<Effect> effects => effectsList;
        public IEnumerable<Item> items => itemsList;

        public string definiteName => EnglishHelpers.GetDefiniteNounForm(name);
        public string indefiniteName => EnglishHelpers.GetIndefiniteNounForm(name);

        public abstract Sprite bodySprite { get; }

        public int proficiencyBonus => 2 + Math.Max(0, (proficiencyBonusBase - 1) / 4);
        protected abstract int proficiencyBonusBase { get; }

        public CreatureController controller { get; private set; }

        public bool isUnconscious => lifeStatus == LifeStatus.StableUnconscious || lifeStatus == LifeStatus.UnstableUnconscious;

        public IEnumerable<object> rules => new object[] { this }.Concat(effects).Concat(items.SelectMany(item => item.rules));
        public string rulesProviderName => indefiniteName;

        public void Initialize(CreatureController controller)
        {
            this.controller = controller;
        }

        public void GiveItem(Item item)
        {
            itemsList.Add(item);
        }

        public virtual IAction TakeTurn(Battle battle, Creature target)
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

            Attack attackEffect = RandomHelpers.Element(attackEffects);
            Item attackItem = null;
            Ability? attackAbility = null;

            // See if the effect comes from an item.
            if (attackEffect.parent is Item parentItem)
            {
                attackItem = parentItem;

                // If the weapon has a finesse property, use the higher of the two ability scores.
                if (parentItem.GetEffect<Finesse>() != null)
                {
                    attackAbility = abilityScores.strength > abilityScores.dexterity ? Ability.Strength : Ability.Dexterity;
                }
            }

            return new Actions.Attack(battle, this, target, attackEffect, attackItem, attackAbility);
        }

        public bool MakeAbilityCheck(Ability ability, int successAmount)
        {
            DebugHelper.StartLog($"{definiteName.ToUpperFirst()} is making a DC {successAmount} {ability} check …");

            int roll = Dice.Roll("d20");
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

            yield return controller.Die();
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

            yield break;
        }

        protected abstract IEnumerator TakeDamageAtZeroHP(int remainingDamageAmount, Hit hit);
    }
}
