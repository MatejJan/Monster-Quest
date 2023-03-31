using System.Collections;
using MonsterQuest.Events;
using UnityEngine;

namespace MonsterQuest.Presenters
{
    public abstract class Presenter : MonoBehaviour
    {
        protected virtual IEventPresenter<AttackEvent> attackEventPresenter => null;
        protected virtual IEventPresenter<DamageEvent> damageEventPresenter => null;
        protected virtual IEventPresenter<DeathSavingThrowEvent> deathSavingThrowEventPresenter => null;
        protected virtual IEventPresenter<GainExperienceEvent> gainExperienceEventPresenter => null;
        protected virtual IEventPresenter<HealEvent> healEventPresenter => null;
        protected virtual IEventPresenter<LevelUpEvent> levelUpEventPresenter => null;
        protected virtual IEventPresenter<LifeStatusEvent> lifeStatusEventPresenter => null;
        protected virtual IEventPresenter<StabilizeCharacterEvent> stabilizeCharacterEventPresenter => null;
        protected virtual IEventPresenter<UseItemEvent> useItemEventPresenter => null;

        public IEnumerator Present(object eventData)
        {
            if (eventData is AttackEvent attackEvent) yield return attackEventPresenter?.Present(attackEvent);
            if (eventData is DamageEvent damageEvent) yield return damageEventPresenter?.Present(damageEvent);
            if (eventData is DeathSavingThrowEvent deathSavingThrowEvent) yield return deathSavingThrowEventPresenter?.Present(deathSavingThrowEvent);
            if (eventData is GainExperienceEvent gainExperienceEvent) yield return gainExperienceEventPresenter?.Present(gainExperienceEvent);
            if (eventData is HealEvent healEvent) yield return healEventPresenter?.Present(healEvent);
            if (eventData is LevelUpEvent levelUpEvent) yield return levelUpEventPresenter?.Present(levelUpEvent);
            if (eventData is LifeStatusEvent lifeStatusEvent) yield return lifeStatusEventPresenter?.Present(lifeStatusEvent);
            if (eventData is StabilizeCharacterEvent stabilizeCharacterEvent) yield return stabilizeCharacterEventPresenter?.Present(stabilizeCharacterEvent);
            if (eventData is UseItemEvent useItemEvent) yield return useItemEventPresenter?.Present(useItemEvent);
        }

        public virtual IEnumerator InitializeParty(GameState gameState)
        {
            yield return null;
        }

        public virtual IEnumerator InitializeMonsters(GameState gameState)
        {
            yield return null;
        }
    }
}
