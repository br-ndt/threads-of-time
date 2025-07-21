using System;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Utility;


namespace Assets.Scripts.Configs
{
    [Serializable]
    public class AttackProgressionDictionary : SerializableDictionary<int, AttackList> { }
}