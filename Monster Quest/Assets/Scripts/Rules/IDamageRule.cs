using System.Collections;

namespace MonsterQuest
{
    public interface IDamageRule
    {
        public IEnumerator ReactToDamage(Damage damage)
        {
            yield break;
        }

        public IEnumerator ReactToDamageDealt(Damage damage)
        {
            yield break;
        }
    }
}
