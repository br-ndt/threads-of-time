using UnityEngine;
using System.Collections.Generic;
using Assets.Scripts.States;

namespace Assets.Scripts.Configs
{
    /// <summary>
    /// Configuration for a specific cutscene.
    /// </summary>
    [CreateAssetMenu(menuName = "Game Configs/Cutscene Config")]
    public class CutsceneConfig : GameConfig
    {
        public string cutsceneID; // Unique ID for this cutscene
        public List<string> dialogueLines = new List<string>(); // Example dialogue
        public List<Vector3> characterStartPositions = new List<Vector3>(); // Character positions
        public float duration = 5.0f; // Cutscene duration
        public GameState gameStateOnEnd = GameState.Overworld; // Optional: scene to load after cutscene
        public BattleConfig nextSceneBattle;
    }
}