using UnityEngine;
using Assets.Scripts.Events; // For CombatCalculationEvent
using Assets.Scripts.States;
using Unity.VisualScripting;
using System.Collections; // For GameState, to ensure we're in Battle state

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Orchestrates combat actions, including damage calculation.
    /// </summary>
    public class CombatManager : MonoBehaviour
    {
        [Header("Event Channel")]
        [SerializeField] private CombatCalculationEvent attackCalculationEvent;
        [SerializeField] private CombatCalculationEvent defendCalculationEvent;
        [SerializeField] private ConditionApplyEvent conditionApplyEvent;

        [Header("Debug References")]
        [SerializeField] private GameObject characterA; // Assign Character A in Inspector
        [SerializeField] private GameObject monsterZ;   // Assign Monster Z in Inspector

        private SpriteCharacter2D attackerSprite;
        private SpriteCharacter2D defenderSprite;

        [Header("Attack Animation")]
        [SerializeField] private readonly float moveSpeed = 20f;
        [SerializeField] private readonly float distanceBuffer = 1.5f;

        /// <summary>
        /// Initiates an attack calculation and applies its effects.
        /// </summary>
        public void PerformAttack(IBattleActor attacker, IBattleActor defender, AttackDefinition attackDefinition, bool skipAnimate = false)
        {
            CombatCalculationContext context = new(
                attacker, defender, Instantiate(attackDefinition)
            );

            attackCalculationEvent.Raise(context);
            defendCalculationEvent.Raise(context);

            context.CalculateFinalValues();


            // TODO(tbrandt): make this more robust, handle per-actor etc
            if (skipAnimate)
            {
                defender.GameObject.GetComponent<Health>().TakeDamage(context.FinalDamage);
                if (context.ConditionsToApply.Count > 0)
                {
                    conditionApplyEvent.Raise((defender, context.ConditionsToApply));
                }
            }
            else
            {
                StartCoroutine(AnimateAttackSequence(attacker, defender, context));
            }
        }

        private IEnumerator AnimateAttackSequence(IBattleActor attacker, IBattleActor defender, CombatCalculationContext context)
        {
            attackerSprite = attacker.GameObject.GetComponentInChildren<SpriteCharacter2D>();
            defenderSprite = defender.GameObject.GetComponentInChildren<SpriteCharacter2D>();
            Vector3 startPosition = attacker.GameObject.transform.position;

            attackerSprite.isFlipped = attacker.GameObject.transform.position.x > defender.GameObject.transform.position.x;

            yield return StartCoroutine(MoveTowardsTarget(attacker, defender));

            if (context.IsCriticalHit)
            {
                attackerSprite.Play(BattleSpriteState.Critical);
            }
            else
            {
                attackerSprite.Play(BattleSpriteState.Attack);
            }

            yield return new WaitForSeconds(0.45f);

            if (context.FinalDamage <= 0)
            {
                defenderSprite.Play(BattleSpriteState.Defend);
            }
            else
            {
                defenderSprite.Play(BattleSpriteState.Hurt);
            }

            defender.GameObject.GetComponent<Health>().TakeDamage(context.FinalDamage);
            if (context.ConditionsToApply.Count > 0)
            {
                conditionApplyEvent.Raise((defender, context.ConditionsToApply));
            }

            attackerSprite.isFlipped = !attackerSprite.isFlipped;

            yield return StartCoroutine(MoveBackToPosition(attacker, startPosition));

            attackerSprite.isFlipped = !attackerSprite.isFlipped;

            if (FindFirstObjectByType<BattleManager>().CurrentBattleState != BattleState.BattleEnd)
            {
                attackerSprite.Play(BattleSpriteState.Idle);
            }
        }

        private IEnumerator MoveTowardsTarget(IBattleActor attacker, IBattleActor defender)
        {
            attackerSprite.Play(BattleSpriteState.Run);
            Vector3 targetPosition = defender.GameObject.transform.position -
                                     (defender.GameObject.transform.position - attacker.GameObject.transform.position).normalized * distanceBuffer;

            while (Vector3.Distance(attacker.GameObject.transform.position, targetPosition) > 0.05f)
            {
                attacker.GameObject.transform.position = Vector3.MoveTowards(attacker.GameObject.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator MoveBackToPosition(IBattleActor attacker, Vector3 startPosition)
        {
            attackerSprite.Play(BattleSpriteState.Run);
            while (Vector3.Distance(attacker.GameObject.transform.position, startPosition) > 0.001f)
            {
                attacker.GameObject.transform.position = Vector3.MoveTowards(attacker.GameObject.transform.position, startPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }


    }


}