using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public class CombatPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject creaturePrefab;
        [SerializeField] private Material charactersMaterial;
        [SerializeField] private Material monstersMaterial;

        [SerializeField] private GameObject[] tilePrefabs;

        private readonly Dictionary<Creature, CreaturePresenter> _creaturePresenters = new();

        private Transform _creaturesTransform;
        private Transform _environmentTransform;

        private void Awake()
        {
            _creaturesTransform = transform.Find("Creatures");
            _environmentTransform = transform.Find("Environment");

            for (int x = 0; x < 8; x++)
            {
                for (int y = 0; y < 8; y++)
                {
                    Instantiate(tilePrefabs[x % 2 == y % 2 ? 1 : 0], new Vector3((x + 0.5f) * 5, 0, (y + 0.5f) * 5), Quaternion.identity, _environmentTransform);
                }
            }
        }

        public CreaturePresenter GetCreaturePresenterForCreature(Creature creature)
        {
            return _creaturePresenters.ContainsKey(creature) ? _creaturePresenters[creature] : null;
        }

        public IEnumerator InitializeParty(GameState gameState)
        {
            yield return InitializeCreatures(gameState.party.characters, 0, CardinalDirection.North, charactersMaterial);
        }

        public IEnumerator InitializeMonsters(GameState gameState)
        {
            yield return InitializeCreatures(gameState.combat.monsters, 40, CardinalDirection.South, monstersMaterial);
        }

        private IEnumerator InitializeCreatures(IEnumerable<Creature> creatures, float z, CardinalDirection direction, Material material)
        {
            Creature[] creaturesArray = creatures.ToArray();

            float totalWidth = creaturesArray.Sum(creature => creature.spaceInFeet);
            float currentX = 20 - Mathf.Floor(totalWidth / 10) * 5;
            Vector3 facingDirection = CardinalDirectionHelper.cardinalDirectionVector3S[direction];

            foreach (Creature creature in creaturesArray)
            {
                currentX += creature.spaceInFeet;

                if (!creature.isAlive) continue;

                float spaceRadius = creature.spaceInFeet / 2;

                GameObject characterGameObject = Instantiate(creaturePrefab, _creaturesTransform);
                characterGameObject.name = creature.displayName;

                Vector3 position = new Vector3(currentX - spaceRadius, 0, z) + facingDirection * spaceRadius;
                characterGameObject.transform.position = position;

                CreaturePresenter creaturePresenter = characterGameObject.GetComponent<CreaturePresenter>();
                creaturePresenter.Initialize(this, creature, material);

                _creaturePresenters[creature] = creaturePresenter;

                yield return creaturePresenter.FaceDirection(direction, true);
            }
        }
    }
}
