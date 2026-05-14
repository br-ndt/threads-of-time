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

        public bool TickTurn()
        {
            _turns--;
            return _turns <= 0;
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
        [SerializeField] float _chance = 1;

        public float Chance { get { return _chance; } set { _chance = value; } }

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
        public ActiveCondition(int turns) : base(turns) { }
    }

    [Serializable]
    public class DamageOverTime : ActiveCondition
    {
        [SerializeField] int _damage;
        public int Damage => _damage;

        public DamageOverTime(int turns, int damage) : base(turns)
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