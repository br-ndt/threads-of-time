using UnityEngine;
using Assets.Scripts.States;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(fileName = "SpriteBook", menuName = "SpriteAnimation/SpriteBook")]
    public class SpriteBookConfig : ScriptableObject
    {
        public BattleSpriteState animationType;
        public AnimPlayback playbackType;
        public Texture2D spriteSheet;
        public int columns = 1;
        public int rows = 1;
        public float framesPerSecond = 6f;
    }
}
