using System;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    [Serializable]
    [Tooltip("The state and config to pass to a GameStateChangeEvent at the conclusion of this scene. Provide GameState.None to remain in the same scene.")]
    public struct SceneChange
    {
        public GameState newState;
        public GameConfig newStateConfig;
    }
}