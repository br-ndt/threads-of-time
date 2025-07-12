using System;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Utility;


namespace Assets.Scripts.Combat
{
    [Serializable]
    public class DamageRangeDictionary : SerializableDictionary<DamageType, FloatRange> { }

    [Serializable]
    public class DamageFloatDictionary : SerializableDictionary<DamageType, float> { }
}