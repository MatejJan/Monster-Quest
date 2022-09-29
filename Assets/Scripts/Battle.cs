using System;
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
            globalRules.Add(new DamageTypeDamageAlteration());
        }

        public IEnumerable<object> rules => globalRules.Concat(heroes.rules).Concat(monster.rules);

        public void QueryRules<T>(Action<T> callback) where T : class
        {
            foreach (object rule in rules)
            {
                if (rule is T)
                {
                    callback(rule as T);
                }
            }
        }

        public IEnumerable<V> GetRuleValues<T, V>(Func<T, V> callback) where T : class
        {
            List<V> values = new();

            QueryRules((T rule) =>
            {
                V value = callback(rule);

                if (value != null)
                {
                    values.Add(value);
                }
            });

            return values;
        }

        public void Simulate()
        {
            Console.WriteLine($"Watch out, {monster.indefiniteName} with {monster.hitPoints} HP appears!");

            int round = 1;

            do
            {
                // Heroes' turn.
                foreach (Character hero in heroes)
                {
                    IAction action = hero.TakeTurn(this, monster);
                    action?.Execute();

                    if (monster.hitPoints == 0) break;
                }

                if (monster.hitPoints > 0)
                {
                    // Monster's turn.
                    int randomHeroIndex = Random.Range(0, heroes.Count);
                    Character attackedHero = heroes[randomHeroIndex];

                    IAction action = monster.TakeTurn(this, attackedHero);
                    action?.Execute();

                    if (attackedHero.hitPoints == 0)
                    {
                        heroes.Remove(attackedHero);
                    }
                }

                round++;
            } while (monster.hitPoints > 0 && heroes.Count > 0 && round < 100);

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
