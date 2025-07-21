using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Combat;
using Assets.Scripts.Configs;
using Assets.Scripts.Events;
using UnityEngine;

// TODO(tbrandt): this shouldn't be around outside of save games (i.e., main menu)
// but it is annoying to assign the demo heroes to this class if it itself is instantiated
// on the global object, so for now just attach it to the global object in the editor
public class PartyManager : MonoBehaviour
{
    [Header("Event Channels")]
    [SerializeField] private float INACTIVE_EXP_COEFFICIENT = 12.0f;
    [SerializeField] private HeroConfigEvent addHeroEvent;
    [SerializeField] private SetupBattleEvent setupBattleEvent;
    [SerializeField] private HealthChangeEvent healthChangeEvent;
    [SerializeField] private IntegerGameEvent experienceGainEvent;

    [Header("Heroes")]
    public List<HeroConfig> demoHeroes;
    public Dictionary<string, Hero> AllHeroes { get; private set; } = new Dictionary<string, Hero>();

    // Placeholders
    private string[] inventory;
    private int currentCurrency;

    private void OnEnable()
    {
        addHeroEvent.OnEventRaised += HandleAddHero;
        setupBattleEvent.OnEventRaised += HandleSetupBattle;
        experienceGainEvent.OnEventRaised += HandleExperienceGain;
        healthChangeEvent.OnEventRaised += HandleHealthChange;
    }

    private void OnDisable()
    {
        addHeroEvent.OnEventRaised -= HandleAddHero;
        setupBattleEvent.OnEventRaised -= HandleSetupBattle;
        experienceGainEvent.OnEventRaised -= HandleExperienceGain;
        healthChangeEvent.OnEventRaised -= HandleHealthChange;
    }

    private void Start()
    {
        // placeholder
        foreach (HeroConfig hero in demoHeroes)
        {
            HandleAddHero(hero);
        }
    }

    private void HandleAddHero(HeroConfig config)
    {
        if (!AllHeroes.ContainsKey(config.actorID))
        {
            Debug.Log(config.actorID);
            AllHeroes.Add(config.actorID, new Hero(config));
            if (GetActiveParty().Count < 3)
            {
                AllHeroes.Values.First(hero => hero.BaseConfig == config).SetActive(true);
            }
        }
    }

    private void HandleSetupBattle(SetupBattleContext context)
    {
        foreach (Hero hero in GetActiveParty())
        {
            context.AddHero(hero);
        }
    }

    private void HandleExperienceGain(int experienceToGain)
    {
        foreach (Hero hero in AllHeroes.Values)
        {
            if (GetActiveParty().Contains(hero))
            {
                hero.GainExperience((int)Mathf.Floor(experienceToGain / GetActiveParty().Count));
            }
            else
            {
                hero.GainExperience((int)Mathf.Floor(experienceToGain / INACTIVE_EXP_COEFFICIENT));
            }
        }
    }

    private void HandleHealthChange((IBattleActor hero, float currentHealth, float maxHealth) payload)
    {
        if (payload.hero is HeroBattleActor)
        {
            AllHeroes[payload.hero.ActorID].SetHealth((int)Mathf.Floor(payload.currentHealth));
        }
    }

    public List<Hero> GetActiveParty()
    {
        return AllHeroes.Values.Where(hero => hero.IsActive).ToList();
    }

}