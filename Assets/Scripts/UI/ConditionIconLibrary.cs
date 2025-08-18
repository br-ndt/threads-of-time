using System.Collections.Generic;
using Assets.Scripts.States;
using UnityEngine;

[CreateAssetMenu(fileName = "ConditionIconLibrary", menuName = "Game/Condition Icon Library")]
public class ConditionIconLibrary : ScriptableObject
{
    [System.Serializable]
    public class ConditionIconMapping
    {
        public Condition condition;
        public Sprite icon;
    }

    [SerializeField]
    private List<ConditionIconMapping> iconMappings;

    private Dictionary<Condition, Sprite> _iconDictionary;

    private void OnEnable()
    {
        _iconDictionary = new Dictionary<Condition, Sprite>();
        foreach (var mapping in iconMappings)
        {
            if (!_iconDictionary.ContainsKey(mapping.condition))
            {
                _iconDictionary.Add(mapping.condition, mapping.icon);
            }
        }
    }

    public Sprite GetIcon(Condition condition)
    {
        if (_iconDictionary.TryGetValue(condition, out Sprite icon))
        {
            return icon;
        }

        Debug.LogWarning($"Icon for condition '{condition}' not found in the library.");
        return null; 
    }
}