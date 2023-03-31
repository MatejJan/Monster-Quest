using System.Linq;
using MonsterQuest.Events;

namespace MonsterQuest
{
    public partial class Character
    {
        public override void Heal(int amount)
        {
            // Characters reset saving throws when healing.
            if (_deathSavingThrows.Count > 0)
            {
                ResetDeathSavingThrows();

                ReportStateEvent(new DeathSavingThrowEvent
                {
                    creature = this,
                    deathSavingThrows = deathSavingThrows.ToArray()
                });
            }

            base.Heal(amount);
        }

        public void Stabilize()
        {
            // Reset saving throws if needed.
            if (_deathSavingThrows.Count > 0)
            {
                ResetDeathSavingThrows();

                ReportStateEvent(new DeathSavingThrowEvent
                {
                    creature = this,
                    deathSavingThrows = deathSavingThrows.ToArray()
                });
            }

            // Update life status.
            lifeStatus = LifeStatus.UnconsciousStable;
        }

        public void HandleUnconsciousState()
        {
            // Unstable unconscious characters must make a death saving throw.
            if (lifeStatus != LifeStatus.UnconsciousUnstable) return;

            DeathSavingThrowEvent deathSavingThrowEvent = new()
            {
                creature = this,
                rollResult = new RollResult("d20"),
                amount = 1
            };

            switch (deathSavingThrowEvent.rollResult)
            {
                case 1:
                    // Critical fails add 2 saving throw failures.
                    deathSavingThrowEvent.amount = 2;
                    deathSavingThrowEvent.succeeded = false;

                    ApplyDeathSavingThrows(deathSavingThrowEvent);

                    break;

                case 20:
                    // Critical successes regain consciousness with 1 HP.
                    deathSavingThrowEvent.succeeded = true;

                    ApplyDeathSavingThrows(deathSavingThrowEvent);
                    Heal(1);

                    break;

                case < 10:
                    deathSavingThrowEvent.succeeded = false;

                    ApplyDeathSavingThrows(deathSavingThrowEvent);

                    break;

                default:
                    deathSavingThrowEvent.succeeded = true;

                    ApplyDeathSavingThrows(deathSavingThrowEvent);

                    break;
            }

            HandleDeathSavingThrows();
        }

        protected override void TakeDamageAtZeroHitPoints(int remainingDamageAmount, Hit hit)
        {
            // Characters instantly die if the remaining damage they would have taken is greater or equal to their maximum HP.
            if (remainingDamageAmount >= hitPointsMaximum)
            {
                Die();

                return;
            }

            // Alive characters fall unconscious.
            if (lifeStatus == LifeStatus.Conscious)
            {
                lifeStatus = LifeStatus.UnconsciousUnstable;

                return;
            }

            // The creature is unconscious. See if damage was dealt.
            if (remainingDamageAmount > 0)
            {
                // When damage was dealt when unconscious, they receive a death saving throw failure (2 on a critical hit).
                DeathSavingThrowEvent deathSavingThrowEvent = new()
                {
                    creature = this,
                    succeeded = false,
                    amount = hit.wasCritical ? 2 : 1
                };

                ApplyDeathSavingThrows(deathSavingThrowEvent);

                // Destabilize if necessary.
                if (lifeStatus == LifeStatus.UnconsciousStable)
                {
                    lifeStatus = LifeStatus.UnconsciousUnstable;
                }
            }

            HandleDeathSavingThrows();
        }

        private void HandleDeathSavingThrows()
        {
            // If the character fails 3 death saving throws, they die.
            if (deathSavingThrowFailures >= 3)
            {
                Die();

                return;
            }

            // If the character succeeds 3 death saving throws, they stabilize.
            if (deathSavingThrowSuccesses >= 3)
            {
                Stabilize();
            }
        }

        private void ApplyDeathSavingThrows(DeathSavingThrowEvent deathSavingThrowEvent)
        {
            for (int i = 0; i < deathSavingThrowEvent.amount; i++)
            {
                _deathSavingThrows.Add(deathSavingThrowEvent.succeeded);
            }

            deathSavingThrowEvent.deathSavingThrows = deathSavingThrows.ToArray();

            ReportStateEvent(deathSavingThrowEvent);
        }

        private void ResetDeathSavingThrows()
        {
            _deathSavingThrows.Clear();
        }
    }
}
