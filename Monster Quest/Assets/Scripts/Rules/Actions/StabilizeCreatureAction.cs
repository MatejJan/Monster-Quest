using System.Collections;

namespace MonsterQuest
{
    public class StabilizeCreatureAction : IAction
    {
        public StabilizeCreatureAction(Character character, Character target)
        {
            this.character = character;
            this.target = target;
        }

        private Character character { get; }
        private Character target { get; }

        public IEnumerator Execute()
        {
            DebugHelper.StartLog($"Performing stabilize creature action by {character.definiteName} targeting {target.definiteName} … ");

            // Face the target.
            if (character.presenter is not null) yield return character.presenter.FaceCreature(target);

            // Perform a DC 10 Wisdom (Medicine) check.
            bool success = character.MakeAbilityCheck(Ability.Wisdom, 10, out int rollResult);

            if (character.presenter is not null) yield return character.presenter.PerformAbilityCheck(success, rollResult);

            Console.WriteLine($"{character.definiteName.ToUpperFirst()} administer first aid to {target.definiteName} {(success ? "and manages" : "but fails")} to stabilize them.");

            if (success) target.Stabilize();

            DebugHelper.EndLog();
        }
    }
}
