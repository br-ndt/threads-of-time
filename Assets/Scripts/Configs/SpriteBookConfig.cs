using UnityEngine;
using Assets.Scripts.States;

namespace Assets.Scripts.Configs
{
    [CreateAssetMenu(fileName = "SpriteBook", menuName = "Sprite Animation/Sprite Book")]
    public class SpriteBookConfig : GameConfig
    {
        public BattleSpriteState animationType;
        public AnimPlayback playbackType => GetPlaybackTypeFromAnimation(animationType);
        public Texture2D spriteSheet;
        public int columns = 1;
        public int rows = 1;
        public float framesPerSecond = 6f;

        private static AnimPlayback GetPlaybackTypeFromAnimation(BattleSpriteState anim)
        {
            switch (anim)
            {
                case BattleSpriteState.None:
                case BattleSpriteState.Idle:
                case BattleSpriteState.Walk:
                case BattleSpriteState.Run:
                    return AnimPlayback.Loop;
                case BattleSpriteState.Defend:
                case BattleSpriteState.Jump:
                case BattleSpriteState.Attack:
                case BattleSpriteState.Critical:
                case BattleSpriteState.Hurt:
                    return AnimPlayback.PlayOnce;
                case BattleSpriteState.Die:
                    return AnimPlayback.PlayThenStop;
            }

            return AnimPlayback.Loop;
        }
    }
}
