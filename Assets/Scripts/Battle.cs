using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Actions;
using Random = UnityEngine.Random;

namespace MonsterQuest
{
    [Serializable]
    public class Battle : IRulesHandler
    {
        public List<object> globalRules = new();
        public Party heroes;
        public Monster monster;

        public Battle()
        {
            // Create global providers.
            globalRules.Add(new AttackAbilityModifier());
            globalRules.Add(new CoverBonus());
            globalRules.Add(new DamageAmountTypeDamageAmountAmountAlteration());
        }

        public IEnumerable<object> rules => globalRules.Concat(heroes.rules).Concat(monster.rules);

        public IEnumerator CallRules<T>(Func<T, IEnumerator> callback) where T : class
        {
            foreach (object rule in rules)
            {
                if (rule is T)
                {
                    IEnumerator result = callback(rule as T);

                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        public IEnumerable<V> GetRuleValues<T, V>(Func<T, V> callback) where T : class
        {
            List<V> values = new();

            foreach (object rule in rules)
            {
                if (rule is T t)
                {
                    V value = callback(t);

                    if (value != null)
                    {
                        values.Add(value);
                    }
                }
            }

            return values;
        }

        public IEnumerator Simulate()
        {
            Console.WriteLine($"Watch out, {monster.indefiniteName} with {monster.hitPoints} HP appears!");

            do
            {
                // Heroes' turn.
                foreach (Character hero in heroes)
                {
                    IAction action = hero.TakeTurn(this, monster);
                    yield return action?.Execute();

                    if (monster.hitPoints == 0) break;
                }

                heroes.RemoveAll(hero => hero.lifeStatus == Creature.LifeStatus.Dead);

                if (monster.lifeStatus != Creature.LifeStatus.Dead && heroes.Count > 0)
                {
                    // Monster's turn.
                    int randomHeroIndex = Random.Range(0, heroes.Count);
                    Character attackedHero = heroes[randomHeroIndex];

                    IAction action = monster.TakeTurn(this, attackedHero);
                    yield return action?.Execute();

                    if (attackedHero.lifeStatus == Creature.LifeStatus.Dead)
                    {
                        heroes.Remove(attackedHero);
                    }
                }
            } while (monster.hitPoints > 0 && heroes.Count > 0);

            if (monster.hitPoints == 0)
            {
                Console.WriteLine($"{monster.definiteName.ToUpperFirst()} collapses and the heroes celebrate their victory!");
            }
            else
            {
                Console.WriteLine($"The party has failed and {monster.definiteName} continues to attack unsuspecting adventurers.");
            }
        }

        public int GetDistance(Creature a, Creature b)
        {
            return 5;
        }

        public IEnumerable<Creature> GetCreatures()
        {
            List<Creature> creatures = new();

            creatures.AddRange(heroes);
            creatures.Add(monster);

            return creatures;
        }

        public bool AreHostile(Creature a, Creature b)
        {
            // Creatures are hostile if one is a character and the other is a monster.
            return (a is Character && b is Monster) || (a is Monster && b is Character);
        }
    }
}