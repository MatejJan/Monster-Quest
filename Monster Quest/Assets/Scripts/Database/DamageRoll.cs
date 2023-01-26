using System;
using UnityEngine;

namespace MonsterQuest
{
    [Serializable]
    public class DamageRoll
    {
        public DamageRoll(string roll, DamageType type, bool isExtraDamage = false, Ability savingThrowAbility = Ability.None, int savingThrowDC = 0)
        {
            this.roll = roll;
            this.type = type;
            this.isExtraDamage = isExtraDamage;
            this.savingThrowAbility = savingThrowAbility;
            this.savingThrowDC = savingThrowDC;
        }

        [field: SerializeField] public string roll { get; private set; }
        [field: SerializeField] public DamageType type { get; private set; }
        [field: SerializeField] public bool isExtraDamage { get; private set; }
        [field: SerializeField] public Ability savingThrowAbility { get; private set; }
        [field: SerializeField] public int savingThrowDC { get; private set; }

        public override string ToString()
        {
            return $"{roll} {(isExtraDamage ? "extra " : "")}{type.ToString().ToLower()} damage";
        }
    }
}
