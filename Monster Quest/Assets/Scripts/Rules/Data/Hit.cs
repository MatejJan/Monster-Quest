namespace MonsterQuest
{
    public class Hit
    {
        public Hit(AttackAction attackAction, bool wasCritical)
        {
            this.attackAction = attackAction;
            this.wasCritical = wasCritical;
        }

        public AttackAction attackAction { get; }

        public bool wasCritical { get; }

        // TODO: public Spell spell {get; }

        public Creature target => attackAction.target;
    }
}
