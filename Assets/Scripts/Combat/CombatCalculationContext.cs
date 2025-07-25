using System;
using System.Linq;
using Assets.Scripts.Utility;
using UnityEngine; // Just for Debug.Log for clarity, can be removed

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Represents the mutable context for a combat calculation (e.g., damage).
    /// Listeners will modify properties of this object.
    /// </summary>
    public class CombatCalculationContext
    {
        public static DamageType[] damageTypes = new DamageType[]{
            DamageType.PHYSICAL,
            DamageType.ERG,
            DamageType.ADDO,
            DamageType.SOLLU,
            DamageType.SQUQ,
            DamageType.AIY,
            DamageType.MAR,
            DamageType.VEX,
            DamageType.STRUE
        };
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

            // Damage Calculation Order: (damage range + base damage) * % mod
            foreach (DamageType damageType in damageTypes)
            {
                // the base damage is 0 if we do not have it in the mutable definition
                float baseDamage = Definition.baseDamageModifierByType.Keys.Contains(damageType) ? Definition.baseDamageModifierByType[damageType] : 0;
                // the damage range is null if we do not have it in the mutable definition
                FloatRange? damageRange = Definition.baseDamageRangeByType.Keys.Contains(damageType) ? Definition.baseDamageRangeByType[damageType] : null;
                // the multiplier is 1 if we do not have it in the definition or the damage type is STRUE
                float multi = damageType == DamageType.STRUE || !Definition.damageMultiplierByType.Keys.Contains(damageType) ? 1 : Definition.damageMultiplierByType[damageType];

                // exit early if we aren't dealing damage
                if (damageRange == null)
                {
                    if (baseDamage == 0)
                    {
                        continue;
                    }
                }
                else
                {
                    baseDamage += UnityEngine.Random.Range(damageRange.Value.min, damageRange.Value.max);
                }

                Debug.Log($"Base {damageType} Damage: {baseDamage:F2}");
                Debug.Log($"{damageType} Damage Multi: {multi * 100}%");
                Debug.Log($"Pre-Resistance {damageType} Damage: {baseDamage * multi}");
                FinalDamage += baseDamage * multi;
            }
            if (IsCriticalHit)
            {
                // Example: Critical hits do 1.5x damage
                FinalDamage *= 1.5f;
                Debug.Log("CRITICAL HIT!");
            }
            Debug.Log($"Flat Bonus (Damage - Resistance): {Definition.overallDamageModifier - Definition.overallResistanceModifier:F2}");
            Debug.Log($"Overall Multi: {(Definition.overallDamageMultiplier - Definition.overallResistanceMultiplier) * 100}%");
            FinalDamage += Definition.overallDamageModifier - Definition.overallResistanceModifier;
            FinalDamage *= Math.Max(Definition.overallDamageMultiplier - Definition.overallResistanceMultiplier, 0);
            FinalDamage = (float)Math.Floor(FinalDamage);
            Debug.Log($"Final Damage: {FinalDamage}");
        }
    }
}