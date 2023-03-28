using System.Collections;

namespace MonsterQuest
{
    public partial class Character
    {
        public override IEnumerator Heal(int amount)
        {
            // Characters reset saving throws when healing.
            ResetDeathSavingThrows();

            yield return base.Heal(amount);
        }

        public void Stabilize()
        {
            lifeStatus = LifeStatus.UnconsciousStable;

            ResetDeathSavingThrows();
        }

        public IEnumerator HandleUnconsciousState()
        {
            // Unstable unconscious characters must make a death saving throw.
            if (lifeStatus != LifeStatus.UnconsciousUnstable) yield break;

            int deathSavingThrowRollResult = DiceHelper.Roll("d20");

            switch (deathSavingThrowRollResult)
            {
                case 1:
                    ReportStateEvent($"{definiteName.ToUpperFirst()} critically fails a death saving throw.");

                    // Critical fails add 2 saving throw failures.
                    yield return ApplyDeathSavingThrows(2, false, deathSavingThrowRollResult);

                    break;

                case 20:
                    // Critical successes regain consciousness with 1 HP.
                    ReportStateEvent($"{definiteName.ToUpperFirst()} critically succeeds a death saving throw.");

                    yield return ApplyDeathSavingThrows(1, true, deathSavingThrowRollResult);

                    ResetDeathSavingThrows();

                    yield return Heal(1);

                    break;

                case < 10:
                    ReportStateEvent($"{definiteName.ToUpperFirst()} fails a death saving throw.");

                    yield return ApplyDeathSavingThrows(1, false, deathSavingThrowRollResult);

                    break;

                default:
                    ReportStateEvent($"{definiteName.ToUpperFirst()} succeeds a death saving throw.");

                    yield return ApplyDeathSavingThrows(1, true, deathSavingThrowRollResult);

                    break;
            }

            yield return HandleDeathSavingThrows();
        }

        protected override IEnumerator TakeDamageAtZeroHitPoints(int remainingDamageAmount, Hit hit)
        {
            // Characters instantly die if the remaining damage they would have taken is greater or equal to their maximum HP.
            if (remainingDamageAmount >= hitPointsMaximum)
            {
                lifeStatus = LifeStatus.Dead;
                ReportStateEvent($"{definiteName.ToUpperFirst()} instantly dies.");

                if (presenter is not null) yield return presenter.GetAttacked(true);

                if (presenter is not null) yield return presenter.Die();

                yield break;
            }

            // Alive characters fall unconscious.
            if (lifeStatus == LifeStatus.Conscious)
            {
                lifeStatus = LifeStatus.UnconsciousUnstable;
                ReportStateEvent($"{definiteName.ToUpperFirst()} falls unconscious.");

                if (presenter is not null) yield return presenter.GetAttacked();

                yield break;
            }

            // The creature is unconscious. See if damage was dealt.
            if (remainingDamageAmount > 0)
            {
                if (presenter is not null) yield return presenter.GetAttacked();

                // When damage was dealt when unconscious, they receive a death saving throw failure (2 on a critical hit).
                if (hit.wasCritical)
                {
                    ReportStateEvent($"{definiteName.ToUpperFirst()} suffers two death saving throw failures.");

                    yield return ApplyDeathSavingThrows(2, false);
                }
                else
                {
                    ReportStateEvent($"{definiteName.ToUpperFirst()} suffers a death saving throw failure.");

                    yield return ApplyDeathSavingThrows(1, false);
                }

                // Destabilize if necessary.
                if (lifeStatus == LifeStatus.UnconsciousStable)
                {
                    lifeStatus = LifeStatus.UnconsciousUnstable;
                }
            }

            yield return HandleDeathSavingThrows();
        }

        private IEnumerator HandleDeathSavingThrows()
        {
            // If the character fails 3 death saving throws, they die.
            if (deathSavingThrowFailures >= 3)
            {
                lifeStatus = LifeStatus.Dead;

                yield return Die();

                yield break;
            }

            // If the character succeeds 3 death saving throws, they stabilize.
            if (deathSavingThrowSuccesses >= 3)
            {
                ReportStateEvent($"{definiteName.ToUpperFirst()} succeeded 3 times and they stabilize.");

                Stabilize();
            }
        }

        private IEnumerator ApplyDeathSavingThrows(int amount, bool success, int? rollResult = null)
        {
            for (int i = 0; i < amount; i++)
            {
                _deathSavingThrows.Add(success);

                if (presenter is not null) yield return presenter.PerformDeathSavingThrow(success, i == 0 ? rollResult : null);
            }
        }

        private void ResetDeathSavingThrows()
        {
            _deathSavingThrows.Clear();
            if (presenter is not null) presenter.ResetDeathSavingThrows();
        }
    }
}
