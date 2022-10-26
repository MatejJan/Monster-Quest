using UnityEngine;

namespace MonsterQuest
{
    public class Character : Creature
    {
        public Character(string displayName, Sprite bodySprite, int hitPointsMaximum, SizeCategory size) : base(displayName, bodySprite, hitPointsMaximum, size)
        {
        }
    }
}
