using Assets.Scripts.Configs;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Overworld
{
    public class OverworldEnemy : MonoBehaviour
    {
        [SerializeField] private BattleConfig _battleConfig;

        public BattleConfig BattleConfig => _battleConfig;
    }
}