// File: AttackList.cs
using System;
using System.Collections.Generic;
using Assets.Scripts.Combat; // Or wherever AttackDefinition is

namespace Assets.Scripts.Configs
{
    [Serializable]
    public class AttackList
    {
        public List<AttackDefinition> Attacks = new();
    }

}