using UnityEngine;

namespace MonsterQuest
{
    public class Character : Creature
    {
        public Character(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory sizeCategory) : base(displayName, bodySprite, hitPointsMaximum, sizeCategory)
        {
        }
    }
}
