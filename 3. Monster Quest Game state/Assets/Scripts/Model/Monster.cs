using System;

namespace MonsterQuest
{
    public class Monster
    {
        public Monster(string displayName, int hitPoints, int savingThrowDC)
        {
            this.displayName = displayName;
            this.hitPoints = hitPoints;
            this.savingThrowDC = savingThrowDC;
        }
        
        public string displayName { get; private set; }
        public int hitPoints { get; private set; }
        public int savingThrowDC { get; private set; }

        public void ReactToDamage(int damageAmount)
        {
            hitPoints -= damageAmount;
            if (hitPoints < 0) hitPoints = 0;
        }
    }
}
