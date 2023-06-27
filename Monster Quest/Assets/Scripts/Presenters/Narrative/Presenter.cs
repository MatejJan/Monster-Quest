using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Narrative
{
    public class Presenter : Presenters.Presenter
    {
        private AttackEventPresenter _attackEventPresenter;
        private DamageEventPresenter _damageEventPresenter;
        private DeathSavingThrowEventPresenter _deathSavingThrowEventPresenter;
        private GainExperienceEventPresenter _gainExperienceEventPresenter;
        private HealEventPresenter _healEventPresenter;
        private LevelUpEventPresenter _levelUpEventPresenter;
        private LifeStatusEventPresenter _lifeStatusEventPresenter;
        private StabilizeCharacterEventPresenter _stabilizeCharacterEventPresenter;
        private UseItemEventPresenter _useItemEventPresenter;

        protected override IEventPresenter<AttackEvent> attackEventPresenter => _attackEventPresenter;
        protected override IEventPresenter<DamageEvent> damageEventPresenter => _damageEventPresenter;
        protected override IEventPresenter<DeathSavingThrowEvent> deathSavingThrowEventPresenter => _deathSavingThrowEventPresenter;
        protected override IEventPresenter<GainExperienceEvent> gainExperienceEventPresenter => _gainExperienceEventPresenter;
        protected override IEventPresenter<HealEvent> healEventPresenter => _healEventPresenter;
        protected override IEventPresenter<LevelUpEvent> levelUpEventPresenter => _levelUpEventPresenter;
        protected override IEventPresenter<LifeStatusEvent> lifeStatusEventPresenter => _lifeStatusEventPresenter;
        protected override IEventPresenter<StabilizeCharacterEvent> stabilizeCharacterEventPresenter => _stabilizeCharacterEventPresenter;
        protected override IEventPresenter<UseItemEvent> useItemEventPresenter => _useItemEventPresenter;

        private void Awake()
        {
            IOutput output = GetComponent<IOutput>();

            _attackEventPresenter = new AttackEventPresenter(output);
            _damageEventPresenter = new DamageEventPresenter(output);
            _deathSavingThrowEventPresenter = new DeathSavingThrowEventPresenter(output);
            _gainExperienceEventPresenter = new GainExperienceEventPresenter(output);
            _healEventPresenter = new HealEventPresenter(output);
            _levelUpEventPresenter = new LevelUpEventPresenter(output);
            _lifeStatusEventPresenter = new LifeStatusEventPresenter(output);
            _stabilizeCharacterEventPresenter = new StabilizeCharacterEventPresenter(output);
            _useItemEventPresenter = new UseItemEventPresenter(output);
        }

        public override IEnumerator InitializeMonsters(GameState gameState)
        {
            Monster[] monsters = gameState.combat.monsters.ToArray();

            if (monsters.Length == 1)
            {
                Console.WriteLine($"Watch out, {monsters[0].indefiniteName} with {monsters[0].hitPoints} HP appears!");
            }
            else
            {
                Dictionary<MonsterType, IEnumerable<Monster>> monsterGroupsByType = gameState.combat.GetMonsterGroupsByType();
                List<string> monsterGroupDescriptions = new();
                int totalHitPoints = 0;

                foreach (KeyValuePair<MonsterType, IEnumerable<Monster>> monsterGroupEntry in monsterGroupsByType)
                {
                    totalHitPoints += monsterGroupEntry.Value.Sum(monster => monster.hitPoints);

                    string displayName = monsterGroupEntry.Key.displayName;
                    int count = monsterGroupEntry.Value.Count();
                    string description;

                    if (count == 1)
                    {
                        description = EnglishHelper.GetIndefiniteNounForm(displayName);
                    }
                    else
                    {
                        description = $"{count} {EnglishHelper.GetPluralNounForm(displayName)}";
                    }

                    monsterGroupDescriptions.Add(description);
                }

                string monstersDescription = EnglishHelper.JoinWithAnd(monsterGroupDescriptions);

                Console.WriteLine($"Watch out, {monstersDescription} with {totalHitPoints} total HP appear!");
            }

            yield return null;
        }
    }
}
