using System;
using System.Collections.Generic;
using System.Linq;

namespace MonsterQuest.Effects
{
    public abstract class AttackType : EffectType, IRulesProvider, IRuleDescriptionsProvider
    {
        public TargetType targetType = TargetType.All;
        public DamageRoll[] damageRolls;

        public Ability attackAbility;

        public bool customAttackModifiers;
        public int attackRollModifier;
        public int damageRollModifier;

        public string displayName;
        public string descriptionVerb;
        public string descriptionObject;

        public virtual string typeName => "attack";

        public ArrayValue<RuleDescription> GetRuleDescriptions(object context = null)
        {
            int? attackerAttackRollModifier = null;
            int? attackerDamageRollModifier = null;

            if (context is InformativeMonsterAttackAction attackAction)
            {
                attackerAttackRollModifier = attackAction.GetRuleValues((IInformativeMonsterAttackAttackRollModifierRule rule) => rule.GetAttackRollModifier(attackAction)).Resolve();
                attackerDamageRollModifier = attackAction.GetRuleValues((IInformativeMonsterAttackDamageRollModifierRule rule) => rule.GetDamageRollModifier(attackAction)).Resolve();
            }

            return new ArrayValue<RuleDescription>(this, new[]
            {
                GetRuleDescription(attackerAttackRollModifier, attackerDamageRollModifier)
            });
        }

        public string rulesProviderName => displayName;

        public RuleDescription GetRuleDescription(int? attackerAttackRollModifier = null, int? attackerDamageRollModifier = null)
        {
            List<string> descriptionParts = new();

            // Generate to hit description.
            if (customAttackModifiers || attackerAttackRollModifier.HasValue)
            {
                int attackModifier = customAttackModifiers ? attackRollModifier : attackerAttackRollModifier.Value;
                descriptionParts.Add($"{attackModifier:+#;-#;+0} to hit");
            }

            // Generate reach description.
            string distanceDescription = GetDistanceDescription();

            if (distanceDescription != null)
            {
                descriptionParts.Add(distanceDescription);
            }

            // Generate target description.
            descriptionParts.Add(GetTargetDescription());

            // Combine description.
            string description = $"{string.Join(", ", descriptionParts).ToUpperFirst()}.";

            // Add hit description if possible.
            if (damageRolls is not null)
            {
                List<string> hitDescriptionParts = new();

                IEnumerable<DamageRoll> simpleDamageRolls = damageRolls.Where(damageRoll => damageRoll.savingThrowAbility == Ability.None);
                string[] damageRollDescriptions = simpleDamageRolls.Select(damageRoll => GetDamageRollDescription(damageRoll, attackerDamageRollModifier)).ToArray();

                if (damageRollDescriptions.Length > 0)
                {
                    string hitDescription = damageRollDescriptions[0];

                    if (damageRollDescriptions.Length > 1)
                    {
                        hitDescription += $", plus {EnglishHelper.JoinWithAnd(damageRollDescriptions[1..])}";
                    }

                    hitDescription += ".";

                    hitDescriptionParts.Add(hitDescription);
                }

                IEnumerable<DamageRoll> savingThrowDamageRolls = damageRolls.Where(damageRoll => damageRoll.savingThrowAbility != Ability.None);

                foreach (DamageRoll damageRoll in savingThrowDamageRolls)
                {
                    hitDescriptionParts.Add($"The target must make a DC {damageRoll.savingThrowDC} {damageRoll.savingThrowAbility} " + $"saving throw, taking {GetDamageRollDescription(damageRoll)} on a failed save, " + "or half as much damage on a successful one.");
                }

                if (hitDescriptionParts.Count > 0)
                {
                    description += $" Hit: {string.Join(", ", hitDescriptionParts)}";
                }
            }

            return new RuleDescription(RuleCategory.Action, displayName, typeName, description);
        }

        protected virtual string GetDistanceDescription()
        {
            return null;
        }

        protected virtual string GetTargetDescription()
        {
            return $"one {targetType.GetDescription()}";
        }

        protected virtual string GetDamageRollDescription(DamageRoll damageRoll, int? damageModifier = null)
        {
            return $"{damageRoll.roll}{GetDamageModifierDescription(damageModifier, damageRoll.isExtraDamage)} {damageRoll.type.ToString().ToLower()} damage";
        }

        protected string GetDamageModifierDescription(int? damageModifier, bool isExtraDamage)
        {
            return damageModifier.HasValue && damageModifier != 0 && !isExtraDamage ? $"{damageModifier:+#;-#}" : "";
        }
    }

    [Serializable]
    public abstract class Attack : Effect, IAttackAbilityRule, IAttackRollModifierRule, IDamageRollModifierRule, IDamageRollRule
    {
        protected Attack(EffectType type, object parent) : base(type, parent) { }
        public AttackType attackType => (AttackType)type;

        public SingleValue<Ability> GetAttackAbility(AttackAction attackAction)
        {
            // Only provide information for the current attack.
            if (!IsOwnAttack(attackAction)) return null;

            // The attack type can specify its own attack ability.
            if (attackType.attackAbility != Ability.None) return new SingleValue<Ability>(this, attackType.attackAbility);

            return null;
        }

        public IntegerValue GetAttackRollModifier(AttackAction attackAction)
        {
            // Only provide information for the current attack.
            if (!IsOwnAttack(attackAction)) return null;

            // The attack type can override the attack roll modifier.
            if (attackType.customAttackModifiers) return new IntegerValue(this, overrideValue: attackType.attackRollModifier);

            return null;
        }

        public IntegerValue GetDamageRollModifier(AttackAction attackAction)
        {
            // Only provide information for the current attack.
            if (!IsOwnAttack(attackAction)) return null;

            // The attack type can override the damage roll modifier.
            if (attackType.customAttackModifiers) return new IntegerValue(this, overrideValue: attackType.damageRollModifier);

            return null;
        }

        public virtual ArrayValue<DamageRoll> GetDamageRolls(Hit hit)
        {
            // Only provide information to attacks with this weapon.
            if (!IsOwnAttack(hit.attackAction)) return null;

            return new ArrayValue<DamageRoll>(this, attackType.damageRolls);
        }

        protected bool IsOwnAttack(AttackAction attackAction)
        {
            return attackAction.effect == this && (attackAction.weapon == parent || attackAction.attacker == parent);
        }
    }
}
