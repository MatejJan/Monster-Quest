using System;
using UnityEngine;

namespace MonsterQuest.Effects
{
    [CreateAssetMenu(fileName = "New Missile Attraction Curse", menuName = "Effects/Magical/Missile Attraction Curse")]
    public class MissileAttractionCurseType : EffectType
    {
        public int range;

        public override Effect Create(object parent)
        {
            return new MissileAttractionCurse(this, parent);
        }
    }

    [Serializable]
    public class MissileAttractionCurse : Effect, ITargetRedirectionRule
    {
        public MissileAttractionCurse(EffectType type, object parent) : base(type, parent) { }
        public MissileAttractionCurseType missileAttractionCurseType => (MissileAttractionCurseType)type;

        public SingleValue<Creature> RedirectTarget(Actions.Attack attack)
        {
            // Only redirect ranged attacks.
            if (attack.effect is not RangedAttack) return null;

            // Only redirect attacks within the range.
            if (Game.state.combat.GetDistance(parent as Creature, attack.target) > missileAttractionCurseType.range) return null;

            // The owner of this curse is within the range and becomes the new target.
            return new SingleValue<Creature>(this, parent as Creature);
        }
    }
}
