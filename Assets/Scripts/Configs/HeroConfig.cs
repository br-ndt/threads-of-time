using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.States;
using Assets.Scripts.Utility;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Configuration for a specific hero.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Hero Config")]
    public class HeroConfig : ActorConfig
    {
        public SerializableDictionary<HeroProgression, AnimationCurve> progressions = new();
        public AttackProgressionDictionary attacksByLevel = new();
        public AnimationCurve expToLevel;

        public List<AttackDefinition> AttacksForLevel(int level)
        {
            return attacksByLevel.Where(kvp => kvp.Key <= level).Select(kvp => kvp.Value.Attacks).SelectMany(list => list).ToList();
        }

        private void OnEnable()
        {
            // This check runs whenever the asset is loaded in the editor.
            // It ensures that even if the saved data for the dictionary is null,
            // we create a new instance so the editor doesn't crash.
            if (attacksByLevel == null)
            {
                attacksByLevel = new();
            }

            // It's good practice to do this for all serializable class fields.
            if (progressions == null)
            {
                progressions = new();
            }
        }
    }
}