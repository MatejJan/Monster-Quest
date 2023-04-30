using System.Collections;
using UnityEngine;

namespace MonsterQuest.Presenters.Miniatures
{
    public class Presenter : Presenters.Presenter
    {
        // private AttackEventPresenter _attackEventPresenter;
        private CombatPresenter _combatPresenter;
        // private DamageEventPresenter _damageEventPresenter;
        // private DeathSavingThrowEventPresenter _deathSavingThrowEventPresenter;
        // private GainExperienceEventPresenter _gainExperienceEventPresenter;
        // private HealEventPresenter _healEventPresenter;
        // private LevelUpEventPresenter _levelUpEventPresenter;
        // private LifeStatusEventPresenter _lifeStatusEventPresenter;
        // private StabilizeCharacterEventPresenter _stabilizeCharacterEventPresenter;
        // private UseItemEventPresenter _useItemEventPresenter;
        //
        // protected override IEventPresenter<AttackEvent> attackEventPresenter => _attackEventPresenter;
        // protected override IEventPresenter<DamageEvent> damageEventPresenter => _damageEventPresenter;
        // protected override IEventPresenter<DeathSavingThrowEvent> deathSavingThrowEventPresenter => _deathSavingThrowEventPresenter;
        // protected override IEventPresenter<GainExperienceEvent> gainExperienceEventPresenter => _gainExperienceEventPresenter;
        // protected override IEventPresenter<HealEvent> healEventPresenter => _healEventPresenter;
        // protected override IEventPresenter<LevelUpEvent> levelUpEventPresenter => _levelUpEventPresenter;
        // protected override IEventPresenter<LifeStatusEvent> lifeStatusEventPresenter => _lifeStatusEventPresenter;
        // protected override IEventPresenter<StabilizeCharacterEvent> stabilizeCharacterEventPresenter => _stabilizeCharacterEventPresenter;
        // protected override IEventPresenter<UseItemEvent> useItemEventPresenter => _useItemEventPresenter;

        private void Awake()
        {
            Transform combatTransform = transform.Find("Combat");
            _combatPresenter = combatTransform.GetComponent<CombatPresenter>();
            //
            // _attackEventPresenter = new AttackEventPresenter(_combatPresenter);
            // _damageEventPresenter = new DamageEventPresenter(_combatPresenter);
            // _deathSavingThrowEventPresenter = new DeathSavingThrowEventPresenter(_combatPresenter);
            // _gainExperienceEventPresenter = new GainExperienceEventPresenter(_combatPresenter);
            // _healEventPresenter = new HealEventPresenter(_combatPresenter);
            // _levelUpEventPresenter = new LevelUpEventPresenter(_combatPresenter);
            // _lifeStatusEventPresenter = new LifeStatusEventPresenter(_combatPresenter);
            // _stabilizeCharacterEventPresenter = new StabilizeCharacterEventPresenter(_combatPresenter);
            // _useItemEventPresenter = new UseItemEventPresenter(_combatPresenter);
        }

        public override IEnumerator InitializeParty(GameState gameState)
        {
            yield return _combatPresenter.InitializeParty(gameState);
        }

        public override IEnumerator InitializeMonsters(GameState gameState)
        {
            yield return _combatPresenter.InitializeMonsters(gameState);
        }
    }
}
