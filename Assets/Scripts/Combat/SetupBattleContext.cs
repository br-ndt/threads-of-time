using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.Configs;

namespace Assets.Scripts.Combat
{
    /// <summary>
    /// Represents the mutable context for setting up a battle.
    /// Listeners will modify properties of this object to assign
    /// the heroes and monsters.
    /// </summary>
    public class SetupBattleContext
    {
        // General Combat Info
        private List<Hero> allHeroes = new();
        private List<EnemyConfig> allEnemies = new();

        public int MaxPerSide { get; private set; } = 3;
        public List<Hero> Heroes => allHeroes.Take(MaxPerSide).ToList();
        public List<EnemyConfig> Enemies => allEnemies.Take(MaxPerSide).ToList();
        public SetupBattleContext(List<HeroConfig> staticHeroes = null, List<EnemyConfig> staticEnemies = null, int maxPerSide = 3) // conditions, timers, etc.
        {
            allHeroes = staticHeroes != null ? staticHeroes.Select(config => new Hero(config)).ToList() : new List<Hero>();
            allEnemies = staticEnemies ?? new List<EnemyConfig>();
            MaxPerSide = maxPerSide;
        }

        public void AddHero(Hero hero)
        {
            allHeroes.Add(hero);
        }
    }
}