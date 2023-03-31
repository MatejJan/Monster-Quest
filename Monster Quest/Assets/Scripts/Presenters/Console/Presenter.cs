using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MonsterQuest.Events;

namespace MonsterQuest.Presenters.Console
{
    public class Presenter : Presenters.Presenter
    {
        protected override IEventPresenter<AttackEvent> attackEventPresenter { get; } = new AttackEventPresenter();
        protected override IEventPresenter<DamageEvent> damageEventPresenter { get; } = new DamageEventPresenter();
        protected override IEventPresenter<DeathSavingThrowEvent> deathSavingThrowEventPresenter { get; } = new DeathSavingThrowEventPresenter();
        protected override IEventPresenter<GainExperienceEvent> gainExperienceEventPresenter { get; } = new GainExperienceEventPresenter();
        protected override IEventPresenter<HealEvent> healEventPresenter { get; } = new HealEventPresenter();
        protected override IEventPresenter<LevelUpEvent> levelUpEventPresenter { get; } = new LevelUpEventPresenter();
        protected override IEventPresenter<LifeStatusEvent> lifeStatusEventPresenter { get; } = new LifeStatusEventPresenter();
        protected override IEventPresenter<StabilizeCharacterEvent> stabilizeCharacterEventPresenter { get; } = new StabilizeCharacterEventPresenter();
        protected override IEventPresenter<UseItemEvent> useItemEventPresenter { get; } = new UseItemEventPresenter();

        public override IEnumerator InitializeMonsters(GameState gameState)
        {
            Monster[] monsters = gameState.combat.monsters.ToArray();

            if (monsters.Length == 1)
            {
                MonsterQuest.Console.WriteLine($"Watch out, {monsters[0].indefiniteName} with {monsters[0].hitPoints} HP appears!");
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

                MonsterQuest.Console.WriteLine($"Watch out, {monstersDescription} with {totalHitPoints} total HP appear!");
            }

            yield return null;
        }
    }
}
