using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest
{
    public class CombatPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject creaturePrefab;

        private Transform _creaturesTransform;

        private void Awake()
        {
            _creaturesTransform = transform.Find("Creatures");
        }

        public IEnumerator InitializeParty(GameState gameState)
        {
            yield return InitializeCreatures(gameState.party.characters, 0, CardinalDirection.South);
        }

        public IEnumerator InitializeMonsters(GameState gameState)
        {
            yield return InitializeCreatures(gameState.combat.monsters, 0, CardinalDirection.North);
        }

        private IEnumerator InitializeCreatures(IEnumerable<Creature> creatures, float y, CardinalDirection direction)
        {
            Creature[] creaturesArray = creatures.ToArray();

            float totalWidth = creaturesArray.Sum(creature => creature.spaceInFeet);
            float currentX = -totalWidth / 2;
            Vector3 facingDirection = CardinalDirectionHelper.cardinalDirectionVectors[direction];

            foreach (Creature creature in creaturesArray)
            {
                currentX += creature.spaceInFeet;

                if (!creature.isAlive) continue;

                float spaceRadius = creature.spaceInFeet / 2;

                GameObject characterGameObject = Instantiate(creaturePrefab, _creaturesTransform);
                characterGameObject.name = creature.displayName;
                characterGameObject.transform.position = new Vector3(currentX - spaceRadius, y, 0) - facingDirection * spaceRadius;

                CreaturePresenter creaturePresenter = characterGameObject.GetComponent<CreaturePresenter>();
                creaturePresenter.Initialize(creature);

                yield return creaturePresenter.FaceDirection(direction, true);
            }
        }
    }
}
