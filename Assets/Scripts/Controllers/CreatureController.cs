using System.Collections;
using UnityEngine;

namespace MonsterQuest.Controllers
{
    public class CreatureController : MonoBehaviour
    {
        private static readonly int _attackHash = Animator.StringToHash("Attack");
        private static readonly int _takeDamageHash = Animator.StringToHash("Take damage");
        private static readonly int _dieHash = Animator.StringToHash("Die");
        private static readonly int _liveHash = Animator.StringToHash("Live");

        private Animator _bodySpriteAnimator;
        private Creature _creature;
        private Game _game;

        private void Start()
        {
            _creature.Initialize(this);

            Transform bodySpriteTransform = transform.Find("Body").Find("Sprite");
            SpriteRenderer bodySpriteRenderer = bodySpriteTransform.GetComponent<SpriteRenderer>();
            bodySpriteRenderer.sprite = _creature.bodySprite;

            _bodySpriteAnimator = bodySpriteTransform.GetComponent<Animator>();

            SpriteRenderer standSpriteRenderer = transform.Find("Stand").GetComponent<SpriteRenderer>();
            standSpriteRenderer.sprite = Game.database.standSprites[(int)_creature.size];
        }

        public void Initialize(Game game, Creature creature)
        {
            _game = game;
            _creature = creature;
        }

        public IEnumerator Attack()
        {
            // Trigger the attack animation.
            _bodySpriteAnimator.SetTrigger(_attackHash);

            yield return new WaitForSeconds(15f / 60f);
        }

        public IEnumerator TakeDamage()
        {
            // Trigger the take damage animation.
            _bodySpriteAnimator.SetTrigger(_takeDamageHash);

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator FallUnconscious()
        {
            // Trigger the die animation.
            _bodySpriteAnimator.SetTrigger(_dieHash);

            yield return new WaitForSeconds(1.9f);
        }

        public IEnumerator RegainConsciousness()
        {
            // Trigger the live animation.
            _bodySpriteAnimator.SetTrigger(_liveHash);

            yield return new WaitForSeconds(1f);
        }

        public IEnumerator Die()
        {
            // Trigger the die animation.
            _bodySpriteAnimator.SetTrigger(_dieHash);

            yield return new WaitForSeconds(1.9f);

            Destroy(gameObject);

            yield return new WaitForSeconds(1f);
        }
    }
}
