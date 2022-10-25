using UnityEngine;

namespace MonsterQuest
{
    public class CombatPresenter : MonoBehaviour
    {
        private Transform _creaturesTransform;

        private void Awake()
        {
            _creaturesTransform = transform.Find("Creatures");
        }

        public void InitializeParty(GameState gameState)
        {
            // Create the character views.
            for (int i = 0; i < gameState.party.characters.Count; i++)
            {
                Creature character = gameState.party.characters[i];

                GameObject characterGameObject = Instantiate(GameManager.database.creaturePrefab, _creaturesTransform);
                characterGameObject.name = character.displayName;
                characterGameObject.transform.position = new Vector3(((gameState.party.characters.Count - 1) * -0.5f + i) * 5, character.spaceTaken / 2, 0);

                CreaturePresenter creaturePresenter = characterGameObject.GetComponent<CreaturePresenter>();
                creaturePresenter.Initialize(character);
                creaturePresenter.FaceDirection(CardinalDirection.South);
            }
        }

        public void InitializeMonster(GameState gameState)
        {
            Combat combat = gameState.combat;

            // Create the monster view.
            GameObject monsterGameObject = Instantiate(GameManager.database.creaturePrefab, _creaturesTransform);
            monsterGameObject.name = combat.monster.displayName;
            monsterGameObject.transform.position = new Vector3(0, -combat.monster.spaceTaken / 2, 0);

            CreaturePresenter creaturePresenter = monsterGameObject.GetComponent<CreaturePresenter>();
            creaturePresenter.Initialize(combat.monster);
            creaturePresenter.FaceDirection(CardinalDirection.North);
        }
    }
}
