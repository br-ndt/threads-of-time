using Assets.Scripts.Events;
using Assets.Scripts.States;
using UnityEngine;

namespace Assets.Scripts.Overworld
{
    public class OverworldPlayer : MonoBehaviour
    {
        [SerializeField] private GameStateChangeEvent hitTriggerEvent;
        void OnTriggerEnter(Collider other)
        {
            OverworldEnemy enemy = other.GetComponent<OverworldEnemy>();
            if (hitTriggerEvent != null && enemy != null)
            {
                Debug.Log("Player entered enemy trigger!");
                hitTriggerEvent.Raise((GameState.Battle, enemy.BattleConfig));
            }
        }
    }
}