using UnityEngine;

namespace ArcadiaOnline.Core
{
    /// <summary>
    /// Game state global. Lihat docs/02_TDD/GameManagers.md.
    /// Urutan inisialisasi manager mengikuti dokumen tersebut:
    /// GameManager -> SaveManager -> WorldStateManager -> AudioManager -> UIManager (Awake)
    /// lalu QuestManager -> CombatManager -> NPCManager -> ItemManager -> MapManager (Start).
    /// </summary>
    public class GameManager : Singleton<GameManager>
    {
        public GameState CurrentState { get; private set; } = GameState.MainMenu;
        public float PlayTime { get; private set; }

        private bool _isPaused;

        protected override void Awake()
        {
            base.Awake();
            // Manager lain (SaveManager, AudioManager, dst) sebaiknya
            // diletakkan sebagai sibling GameObject di scene Bootstrap
            // dan masing-masing menginisialisasi dirinya sendiri di Awake().
        }

        private void Update()
        {
            if (CurrentState == GameState.Playing)
            {
                PlayTime += Time.deltaTime;
            }
        }

        public void ChangeState(GameState newState)
        {
            if (CurrentState == newState) return;

            GameState previous = CurrentState;
            CurrentState = newState;
            Events.GameStateChanged(previous, newState);
        }

        public void PauseGame()
        {
            if (_isPaused) return;
            _isPaused = true;
            Time.timeScale = 0f;
            ChangeState(GameState.Paused);
        }

        public void ResumeGame()
        {
            if (!_isPaused) return;
            _isPaused = false;
            Time.timeScale = 1f;
            ChangeState(GameState.Playing);
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        Battle,
        Cutscene,
        GameOver
    }
}
