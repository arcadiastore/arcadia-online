using UnityEngine;

namespace ArcadiaOnline.Managers
{
    /// <summary>
    /// Battle BGM system - override BGM saat combat.
    /// Attach ke GameObject "AudioManager".
    /// </summary>
    public class BattleBGMManager : MonoBehaviour
    {
        [Header("Battle BGM")]
        [SerializeField] private AudioClip battleBGM;
        [SerializeField] [Range(0f, 1f)] private float battleVolume = 0.6f;
        [SerializeField] private float fadeDuration = 0.5f;

        [Header("Boss BGM")]
        [SerializeField] private AudioClip bossBGM;
        [SerializeField] [Range(0f, 1f)] private float bossVolume = 0.7f;

        [Header("Components")]
        [SerializeField] private AudioSource battleSource;

        private AudioSource mapSource; // Reference ke MapBGMManager source
        private bool isInBattle = false;
        private float originalMapVolume;

        public static BattleBGMManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            // Setup Battle AudioSource
            if (battleSource == null)
            {
                battleSource = gameObject.AddComponent<AudioSource>();
                battleSource.loop = true;
                battleSource.playOnAwake = false;
            }
        }

        /// <summary>
        /// Masuk battle - ganti BGM ke battle music.
        /// </summary>
        public void EnterBattle()
        {
            if (isInBattle) return;
            isInBattle = true;

            Debug.Log("[BattleBGM] Masuk battle - ganti BGM");

            // Simpan volume map BGM
            if (MapBGMManager.Instance != null)
            {
                MapBGMManager.Instance.SetVolume(0.1f); // Kecilkan map BGM
            }

            // Play battle BGM
            if (battleBGM != null)
            {
                StartCoroutine(FadeInBattleBGM(battleBGM, battleVolume));
            }
        }

        /// <summary>
        /// Masuk boss battle - ganti BGM ke boss music.
        /// </summary>
        public void EnterBossBattle()
        {
            if (isInBattle) return;
            isInBattle = true;

            Debug.Log("[BattleBGM] Masuk boss battle - ganti BGM boss");

            if (MapBGMManager.Instance != null)
            {
                MapBGMManager.Instance.SetVolume(0.05f);
            }

            if (bossBGM != null)
            {
                StartCoroutine(FadeInBattleBGM(bossBGM, bossVolume));
            }
        }

        /// <summary>
        /// Keluar battle - kembali ke map BGM.
        /// </summary>
        public void ExitBattle()
        {
            if (!isInBattle) return;
            isInBattle = false;

            Debug.Log("[BattleBGM] Keluar battle - kembali ke map BGM");

            // Fade out battle BGM
            StartCoroutine(FadeOutBattleBGM());

            // Kembalikan volume map BGM
            if (MapBGMManager.Instance != null)
            {
                MapBGMManager.Instance.SetVolume(0.5f); // Kembalikan volume normal
            }
        }

        private System.Collections.IEnumerator FadeInBattleBGM(AudioClip clip, float targetVolume)
        {
            battleSource.clip = clip;
            battleSource.volume = 0f;
            battleSource.Play();

            float elapsed = 0f;
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                battleSource.volume = Mathf.Lerp(0f, targetVolume, t);
                yield return null;
            }

            battleSource.volume = targetVolume;
        }

        private System.Collections.IEnumerator FadeOutBattleBGM()
        {
            float startVolume = battleSource.volume;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                battleSource.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            battleSource.Stop();
            battleSource.volume = 0f;
        }

        public void SetBattleVolume(float volume)
        {
            battleVolume = Mathf.Clamp01(volume);
            if (isInBattle)
            {
                battleSource.volume = battleVolume;
            }
        }
    }
}
