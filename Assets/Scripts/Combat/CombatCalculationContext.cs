using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine; // Just for Debug.Log for clarity, can be removed

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Represents the mutable context for a combat calculation (e.g., damage).
    /// Listeners will modify properties of this object.
    /// </summary>
    public class CombatCalculationContext
    {
        // General Combat Info
        public GameObject Attacker { get; private set; }
        public GameObject Defender { get; private set; }
        public AttackDefinition Definition { get; private set; }

        // Final calculated values (can be read after all modifications)
        public float FinalDamage { get; private set; }
        public bool IsCriticalHit { get; private set; }
        public bool ApplyPoison { get; private set; }

        // TODO(tbrandt): make attacker and defender be custom monobehaviors
        public CombatCalculationContext(GameObject attacker, GameObject defender, AttackDefinition attackDefinition)
        {
            Attacker = attacker;
            Defender = defender;
            Definition = attackDefinition;
        }

        /// <summary>
        /// Calculates the final outcome based on accumulated modifiers.
        /// Call this AFTER all listeners have had a chance to modify the context.
        /// </summary>
        public void CalculateFinalValues()
        {
            // Critical Hit Check
            // A simple example: flat base crit chance + bonuses.
            // In a real game, this might involve defender's crit evasion, etc.
            float toHit = UnityEngine.Random.value;
            Debug.Log($"To Hit: {toHit:F2}");

            if (toHit < Definition.dodgeChance)
            {
                // We missed;
                Debug.Log("MISS!");
                return;
            }
            IsCriticalHit = toHit >= 1 - (Definition.baseCritChance + Definition.critChanceBonus);

            // Damage Calculation Order: base damage * % mod
            foreach (KeyValuePair<DamageType, float> damageEntry in Definition.baseDamageByType)
            {
                float damage = Definition.damageRangeByType.Keys.Contains(damageEntry.Key) ? UnityEngine.Random.Range(damageEntry.Value, Definition.damageRangeByType[damageEntry.Key]) : damageEntry.Value;
                float multi = damageEntry.Key != DamageType.STRUE ? Definition.damageModifierByType[damageEntry.Key] : 1;

                Debug.Log($"Base {damageEntry.Key} Damage: {damageEntry.Value:F2}");
                Debug.Log($"{damageEntry.Key} Damage Multi: {multi * 100}%");
                Debug.Log($"Pre-Resistance {damageEntry.Key} Damage: {damage * multi}");
                FinalDamage += damage * multi;
            }
            if (IsCriticalHit)
            {
                // Example: Critical hits do 1.5x damage
                FinalDamage *= 1.5f;
                Debug.Log("CRITICAL HIT!");
            }
            Debug.Log($"Flat Resistance: {Definition.overallResistanceValue:F2}");
            Debug.Log($"Resistance Multi: {Definition.overallResistanceMultiplier * 100}%");
            FinalDamage -= Definition.overallResistanceValue;
            FinalDamage *= Math.Clamp(1 - Definition.overallResistanceMultiplier, 0, FinalDamage);
            FinalDamage = (float)Math.Floor(FinalDamage);
            Debug.Log($"Final Damage: {FinalDamage}");
        }
    }
}