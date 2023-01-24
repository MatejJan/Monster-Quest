using System;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class AbilityScores
    {
        public bool justChecking;
        public AbilityScore wonderful;

        public AbilityScores()
        {
            strength = new AbilityScore();
            dexterity = new AbilityScore();
            constitution = new AbilityScore();
            intelligence = new AbilityScore();
            wisdom = new AbilityScore();
            charisma = new AbilityScore();
        }

        // State properties
        [field: SerializeField] public AbilityScore strength { get; private set; }
        [field: SerializeField] public AbilityScore dexterity { get; private set; }
        [field: SerializeField] public AbilityScore constitution { get; private set; }
        [field: SerializeField] public AbilityScore intelligence { get; private set; }
        [field: SerializeField] public AbilityScore wisdom { get; private set; }
        [field: SerializeField] public AbilityScore charisma { get; private set; }

        // Allow access with the enum.
        public AbilityScore this[Ability ability]
        {
            get
            {
                return ability switch
                {
                    Ability.Strength => strength,
                    Ability.Dexterity => dexterity,
                    Ability.Constitution => constitution,
                    Ability.Intelligence => intelligence,
                    Ability.Wisdom => wisdom,
                    Ability.Charisma => charisma,
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}
