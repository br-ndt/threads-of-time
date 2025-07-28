using System;
using Assets.Scripts.States;
using Assets.Scripts.Utility;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Configs
{
    [Serializable]
    public class HasDuration
    {
        [SerializeField] int _turns;

        public int Turns => _turns;

        public HasDuration(int turns)
        {
            _turns = turns;
        }

        public void TickTurn()
        {
            _turns--;
        }
        
        public void AddTurns(int turns)
        {
            _turns += turns;
        }
    }
    // TODO (tbrandt): move these classes but also include condition damage?
    [Serializable]
    public class ConditionStats : HasDuration
    {
        [SerializeField] float _chance;

        [SerializeField] public float Chance { get { return _chance; } set { _chance = value; } }

        public ConditionStats(int turns, float chance) : base(turns)
        {
            _chance = chance;
        }

        public new void TickTurn()
        { }
    }

    [Serializable]
    public class ActiveCondition : HasDuration
    {
        int _damage;
        [SerializeField] public int Damage => _damage;

        public ActiveCondition(int turns, int damage) : base(turns)
        {
            _damage = damage;
        }
    }

    [Serializable]
    public class ConditionBooleanDictionary : SerializableDictionary<Condition, bool> { }
    [Serializable]
    public class ConditionFloatDictionary : SerializableDictionary<Condition, float> { }
    [Serializable]
    public class ConditionStatsDictionary : SerializableDictionary<Condition, ConditionStats> { }
    [Serializable]
    public class ActiveConditionsDictionary : SerializableDictionary<Condition, ActiveCondition> { };
}