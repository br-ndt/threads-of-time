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
        public void PerformAttack(GameObject attacker, GameObject defender, AttackDefinition attackDefinition)
        {
            // 1. Create a new context for this specific calculation
            CombatCalculationContext context = new(
                attacker, defender, Instantiate(attackDefinition)
            );

            // 2. Raise the event, allowing all listeners to modify the context
            attackCalculationEvent.Raise(context);
            defendCalculationEvent.Raise(context);

            // 3. After all listeners have run, calculate the final values
            context.CalculateFinalValues();

            attackerSprite = attacker.GetComponentInChildren<SpriteCharacter2D>();
            defenderSprite = defender.GetComponentInChildren<SpriteCharacter2D>();

            // 4. Begin attack sequence
            StartCoroutine(AnimateAttackSequence(attacker, defender, context));
        }

        private IEnumerator AnimateAttackSequence(GameObject attacker, GameObject defender, CombatCalculationContext context)
        {
            Vector3 startPosition = attacker.transform.position;

            var attackerSprite = attacker.GetComponentInChildren<SpriteCharacter2D>();

            // Face the defender
            attackerSprite.isFlipped = attacker.transform.position.x > defender.transform.position.x;

            // Move toward target
            yield return StartCoroutine(MoveTowardsTarget(attacker, defender));

            // Play attack animation
            if (context.IsCriticalHit)
                attackerSprite.Play(BattleSpriteState.Critical);
            else
                attackerSprite.Play(BattleSpriteState.Attack);

            yield return new WaitForSeconds(0.45f);

            // Defender reacts
            if (context.FinalDamage <= 0)
                defenderSprite.Play(BattleSpriteState.Defend);
            else
                defenderSprite.Play(BattleSpriteState.Hurt);

            // 5. Damage gets applied here
            defender.GetComponent<Health>().TakeDamage(context.FinalDamage);

            // Flip back before moving back to start
            attackerSprite.isFlipped = !attackerSprite.isFlipped;

            // Move back
            yield return StartCoroutine(MoveBackToPosition(attacker, startPosition));

            // Restore facing to original
            attackerSprite.isFlipped = !attackerSprite.isFlipped;

            if (FindFirstObjectByType<BattleManager>().CurrentBattleState != BattleState.BattleEnd)
            {
                attackerSprite.Play(BattleSpriteState.Idle);
            }
        }

        private IEnumerator MoveTowardsTarget(GameObject attacker, GameObject defender)
        {
            attackerSprite.Play(BattleSpriteState.Run);
            Vector3 targetPosition = defender.transform.position -
                                     (defender.transform.position - attacker.transform.position).normalized * distanceBuffer;

            while (Vector3.Distance(attacker.transform.position, targetPosition) > 0.05f)
            {
                attacker.transform.position = Vector3.MoveTowards(attacker.transform.position, targetPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator MoveBackToPosition(GameObject attacker, Vector3 startPosition)
        {
            attackerSprite.Play(BattleSpriteState.Run);
            while (Vector3.Distance(attacker.transform.position, startPosition) > 0.001f)
            {
                attacker.transform.position = Vector3.MoveTowards(attacker.transform.position, startPosition, moveSpeed * Time.deltaTime);
                yield return null;
            }
        }


    }


}