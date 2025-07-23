using System;
using Assets.Scripts.States;
using Assets.Scripts.Utility;

namespace Assets.Scripts.Configs
{
    [Serializable]
    public class ConditionBooleanDictionary : SerializableDictionary<Condition, bool> { }
}