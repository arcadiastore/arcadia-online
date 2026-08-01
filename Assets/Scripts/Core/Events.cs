using System;

namespace ArcadiaOnline.Core
{
    /// <summary>
    /// Event system global (Observer Pattern) untuk komunikasi antar sistem
    /// tanpa coupling langsung. Lihat docs/02_TDD/ScriptArchitecture.md.
    /// </summary>
    public static class Events
    {
        // Player
        public static event Action<int> OnPlayerLevelUp;
        public static event Action<float, float> OnHPChanged;   // current, max
        public static event Action<float, float> OnMPChanged;   // current, max
        public static event Action OnPlayerDied;

        // Quest
        public static event Action<string> OnQuestAccepted;
        public static event Action<string> OnQuestComplete;
        public static event Action<string> OnQuestFailed;

        // World
        public static event Action<string, bool> OnWorldStateChanged;

        // Combat
        public static event Action<UnityEngine.Transform> OnCombatEntered;
        public static event Action OnCombatExited;

        // Game
        public static event Action<GameState, GameState> OnGameStateChanged;

        public static void PlayerLevelUp(int level) => OnPlayerLevelUp?.Invoke(level);
        public static void HPChanged(float current, float max) => OnHPChanged?.Invoke(current, max);
        public static void MPChanged(float current, float max) => OnMPChanged?.Invoke(current, max);
        public static void PlayerDied() => OnPlayerDied?.Invoke();

        public static void QuestAccepted(string questId) => OnQuestAccepted?.Invoke(questId);
        public static void QuestComplete(string questId) => OnQuestComplete?.Invoke(questId);
        public static void QuestFailed(string questId) => OnQuestFailed?.Invoke(questId);

        public static void WorldStateChanged(string key, bool value) => OnWorldStateChanged?.Invoke(key, value);

        public static void CombatEntered(UnityEngine.Transform enemy) => OnCombatEntered?.Invoke(enemy);
        public static void CombatExited() => OnCombatExited?.Invoke();

        public static void GameStateChanged(GameState previous, GameState next) => OnGameStateChanged?.Invoke(previous, next);
    }
}
