using MonsterQuest.Actions;

namespace MonsterQuest
{
    public class Hit
    {
        public Hit(Attack attack, bool wasCritical)
        {
            this.attack = attack;
            this.wasCritical = wasCritical;
        }

        public Attack attack { get; }

        public bool wasCritical { get; }

        // TODO: public Spell spell {get; }

        public Creature target => attack.target;
        public Combat combat => Game.state.combat;
    }
}
