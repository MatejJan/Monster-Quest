using System;
using System.Linq;

namespace MonsterQuest
{
    public class CombatManager : IStateEventProvider
    {
        private readonly GameState _gameState;

        public CombatManager(GameState gameState)
        {
            _gameState = gameState;
        }

        public bool combatActive => areHostileGroupsPresent;

        private bool areHostileGroupsPresent => _gameState.combat.GetHostileGroups().Count() > 1;

        // Events 

        public event Action<object> stateEvent;

        // Methods 

        public void SimulateTurn()
        {
            // Simulate a combat turn.
            Creature creature = _gameState.combat.StartNextCreatureTurn();

            if (creature.lifeStatus == LifeStatus.Dead) return;

            // In case a character became conscious after the start of the combat, mark them as participating.
            if (creature.lifeStatus == LifeStatus.Conscious && creature is Character character)
            {
                _gameState.combat.AddParticipatingCharacter(character);
            }

            IAction action = creature.TakeTurn(_gameState);

            if (action is IStateEventProvider stateEventProvider)
            {
                stateEventProvider.stateEvent += ReportStateEvent;
                stateEventProvider.StartProvidingStateEvents();
            }

            action?.Execute();

            if (areHostileGroupsPresent) return;

            if (_gameState.party.aliveCount > 0)
            {
                Console.WriteLine("The heroes celebrate their victory!");
            }
            else
            {
                Console.WriteLine("The party has failed and the monsters continue to attack unsuspecting adventurers.");
            }

            _gameState.combat.End();
        }

        private void ReportStateEvent(object eventData)
        {
            stateEvent?.Invoke(eventData);
        }
    }
}
