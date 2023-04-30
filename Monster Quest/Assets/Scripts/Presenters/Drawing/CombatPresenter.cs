using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MonsterQuest.Presenters.Drawing
{
    public class CombatPresenter : MonoBehaviour
    {
        [SerializeField] private GameObject creaturePrefab;

        private readonly Dictionary<Creature, CreaturePresenter> _creaturePresenters = new();

        private Transform _creaturesTransform;

        private void Awake()
        {
            _creaturesTransform = transform.Find("Creatures");
        }

        public CreaturePresenter GetCreaturePresenterForCreature(Creature creature)
        {
            return _creaturePresenters.ContainsKey(creature) ? _creaturePresenters[creature] : null;
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
            Vector3 facingDirection = CardinalDirectionHelper.cardinalDirectionVector2S[direction];

            foreach (Creature creature in creaturesArray)
            {
                currentX += creature.spaceInFeet;

                if (!creature.isAlive) continue;

                float spaceRadius = creature.spaceInFeet / 2;

                GameObject characterGameObject = Instantiate(creaturePrefab, _creaturesTransform);
                characterGameObject.name = creature.displayName;

                Vector3 position = new Vector3(currentX - spaceRadius, y, 0) - facingDirection * spaceRadius;
                position.z = position.y * 0.01f;
                characterGameObject.transform.position = position;

                CreaturePresenter creaturePresenter = characterGameObject.GetComponent<CreaturePresenter>();
                creaturePresenter.Initialize(this, creature);

                _creaturePresenters[creature] = creaturePresenter;
                creaturePresenter.destroyed += () => _creaturePresenters.Remove(creature);

                yield return creaturePresenter.FaceDirection(direction, true);
            }
        }
    }
}
