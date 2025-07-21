using UnityEngine;
using System;
using System.Collections.Generic;
using Assets.Scripts.Combat;
using Assets.Scripts.Configs;
using Assets.Scripts.States;

[Serializable]
public class Hero
{
    HeroConfig _baseConfig;
    bool _isActive;
    int _currentLevel;
    int _currentHealth;
    int _maxHealth;
    int _currentExperience;
    List<AttackDefinition> _availableAttacks = new();
    DamageFloatDictionary _flatDamageModifiers = new();
    DamageFloatDictionary _damageMultipliers = new();
    DamageFloatDictionary _flatResistances = new();
    DamageFloatDictionary _resistanceMultipliers = new();

    public HeroConfig BaseConfig => _baseConfig;
    public bool IsActive => _isActive;
    public int CurrentLevel => _currentLevel;
    public int CurrentHealth => _currentHealth;
    public int MaxHealth => _maxHealth;
    public int CurrentExperience => _currentExperience;
    public List<AttackDefinition> AvailableAttacks => _availableAttacks;
    public DamageFloatDictionary FlatDamageModifiers => _flatDamageModifiers;
    public DamageFloatDictionary DamageMultipliers => _damageMultipliers;
    public DamageFloatDictionary FlatResistances => _flatResistances;
    public DamageFloatDictionary ResistanceMultipliers => _resistanceMultipliers;

    public Hero(HeroConfig config, int level = 1)
    {
        _baseConfig = config;
        _isActive = false;
        _currentLevel = level;
        _currentHealth = _maxHealth = _baseConfig.baseHealth;
        _availableAttacks = new List<AttackDefinition>(_baseConfig.attacks);
        for (int i = 0; i < level; ++i)
        {
            int healthToGain = (int)Math.Floor(_baseConfig.progressions[HeroProgression.HealthBonus].Evaluate(_currentLevel));
            _currentHealth += healthToGain;
            _maxHealth += healthToGain;
            _availableAttacks.AddRange(_baseConfig.AttacksForLevel(_currentLevel));
            // _flatDamageModifiers = _baseConfig.flatDamageModifiers + _baseConfig.progressions.flatDamageModifiers.Evaluate(_currentLevel);
            // _damageMultipliers = _baseConfig.damageMultipliers + _baseConfig.progressions.damageMultipliers.Evaluate(_currentLevel);
            // _flatResistances = _baseConfig.flatResistances + _baseConfig.progressions.flatResistances.Evaluate(_currentLevel);
            // _resistanceMultipliers = _baseConfig.resistanceMultipliers + _baseConfig.progressions.resistanceMultipliers.Evaluate(_currentLevel);
        }
    }

    public void SetActive(bool isActive)
    {
        _isActive = isActive;
    }

    public void GainExperience(int expToGain)
    {
        Debug.Log($"{BaseConfig.actorName} gained {expToGain} experience...");
        _currentExperience += expToGain;
        if (_currentExperience >= _baseConfig.expToLevel.Evaluate(_currentLevel))
        {
            LevelUp();
        }
    }

    public void SetHealth(int newHealth)
    {
        _currentHealth = Math.Max(0, Math.Min(newHealth, MaxHealth));
    }

    private void LevelUp()
    {
        Debug.Log($"{BaseConfig.actorName} leveled up!");
        _currentLevel++;
        _currentExperience = 0;
        int healthToGain = (int)Math.Floor(_baseConfig.progressions[HeroProgression.HealthBonus].Evaluate(_currentLevel));
        _currentHealth += healthToGain;
        _maxHealth += healthToGain;
        _availableAttacks.AddRange(_baseConfig.AttacksForLevel(_currentLevel));
    }
}