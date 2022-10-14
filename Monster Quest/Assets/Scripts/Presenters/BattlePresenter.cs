using UnityEngine;

namespace MonsterQuest.Presenters
{
    public class BattlePresenter : MonoBehaviour
    {
        private Transform _creaturesTransform;

        private void Awake()
        {
            _creaturesTransform = transform.Find("Creatures");
        }

        public void InitializeParty()
        {
            // Create the character views.
            for (int i = 0; i < Game.state.party.characters.Count; i++)
            {
                Creature character = Game.state.party.characters[i];

                GameObject characterGameObject = Instantiate(Game.database.creaturePrefab, _creaturesTransform);
                characterGameObject.name = character.displayName;
                characterGameObject.transform.position = new Vector3(((Game.state.party.characters.Count - 1) * -0.5f + i) * 5, character.spaceTaken / 2, 0);

                CreaturePresenter creaturePresenter = characterGameObject.GetComponent<CreaturePresenter>();
                creaturePresenter.Initialize(character);
                creaturePresenter.FaceDirection(CardinalDirection.South);
            }
        }

        public void InitializeMonster()
        {
            Combat combat = Game.state.combat;

            // Create the monster view.
            GameObject monsterGameObject = Instantiate(Game.database.creaturePrefab, _creaturesTransform);
            monsterGameObject.name = combat.monster.displayName;
            monsterGameObject.transform.position = new Vector3(0, -combat.monster.spaceTaken / 2, 0);

            CreaturePresenter creaturePresenter = monsterGameObject.GetComponent<CreaturePresenter>();
            creaturePresenter.Initialize(combat.monster);
            creaturePresenter.FaceDirection(CardinalDirection.North);
        }
    }
}
