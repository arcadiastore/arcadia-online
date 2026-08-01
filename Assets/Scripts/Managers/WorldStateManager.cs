using System.Collections.Generic;
using UnityEngine;
using ArcadiaOnline.Core;

namespace ArcadiaOnline.Managers
{
    /// <summary>
    /// Sistem WorldState - fondasi pilar "Setiap Pilihan Berarti".
    /// Lihat docs/ProjectCharter.md (contoh: VillageDestroyed, KingAlive,
    /// DemonKingAwaken) dan docs/02_TDD/GameManagers.md.
    /// </summary>
    public class WorldStateManager : Singleton<WorldStateManager>
    {
        private readonly Dictionary<string, bool> _states = new Dictionary<string, bool>();

        public void RegisterState(string key, bool defaultValue)
        {
            if (!_states.ContainsKey(key))
            {
                _states[key] = defaultValue;
            }
        }

        public void SetState(string key, bool value)
        {
            _states[key] = value;
            Events.WorldStateChanged(key, value);
        }

        public bool GetState(string key)
        {
            return _states.TryGetValue(key, out bool value) && value;
        }

        public IReadOnlyDictionary<string, bool> GetAllStates() => _states;
    }
}
