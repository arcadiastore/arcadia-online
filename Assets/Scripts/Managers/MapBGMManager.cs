using UnityEngine;
using System.Collections.Generic;

namespace ArcadiaOnline.Managers
{
    /// <summary>
    /// Sistem BGM yang berubah sesuai map/area.
    /// Attach ke GameObject "AudioManager".
    /// </summary>
    public class MapBGMManager : MonoBehaviour
    {
        [System.Serializable]
        public class MapBGM
        {
            public string mapName;
            public AudioClip bgmClip;
            [Range(0f, 1f)]
            public float volume = 0.5f;
        }

        [Header("BGM List")]
        [SerializeField] private List<MapBGM> mapBGMs = new List<MapBGM>();

        [Header("Default")]
        [SerializeField] private string defaultMap = "BeginnerVillage";
        [SerializeField] private float fadeDuration = 1f;

        [Header("Components")]
        [SerializeField] private AudioSource audioSourceA;
        [SerializeField] private AudioSource audioSourceB;

        private string currentMap;
        private AudioSource activeSource;
        private AudioSource inactiveSource;
        private bool isFading = false;

        public static MapBGMManager Instance { get; private set; }

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

            // Setup dual audio source untuk crossfade
            if (audioSourceA == null || audioSourceB == null)
            {
                SetupAudioSources();
            }

            activeSource = audioSourceA;
            inactiveSource = audioSourceB;

            // Play default BGM
            PlayBGM(defaultMap);
        }

        private void SetupAudioSources()
        {
            // Buat 2 AudioSource jika belum ada
            if (audioSourceA == null)
            {
                audioSourceA = gameObject.AddComponent<AudioSource>();
                audioSourceA.loop = true;
                audioSourceA.playOnAwake = false;
            }

            if (audioSourceB == null)
            {
                audioSourceB = gameObject.AddComponent<AudioSource>();
                audioSourceB.loop = true;
                audioSourceB.playOnAwake = false;
            }
        }

        /// <summary>
        /// Ganti BGM berdasarkan nama map.
        /// Panggil saat player masuk area baru.
        /// </summary>
        public void PlayBGM(string mapName)
        {
            if (currentMap == mapName) return; // Sudah playing

            MapBGM mapBGM = mapBGMs.Find(m => m.mapName == mapName);
            if (mapBGM == null)
            {
                Debug.LogWarning($"[MapBGM] BGM untuk map '{mapName}' tidak ditemukan!");
                return;
            }

            currentMap = mapName;
            StartCoroutine(CrossfadeBGM(mapBGM.bgmClip, mapBGM.volume));
        }

        /// <summary>
        /// Stop semua BGM.
        /// </summary>
        public void StopBGM()
        {
            if (activeSource.isPlaying)
            {
                StartCoroutine(FadeOut(activeSource));
            }
            currentMap = null;
        }

        /// <summary>
        /// Set volume BGM.
        /// </summary>
        public void SetVolume(float volume)
        {
            if (activeSource != null)
            {
                activeSource.volume = volume;
            }
        }

        private System.Collections.IEnumerator CrossfadeBGM(AudioClip newClip, float targetVolume)
        {
            if (isFading) yield break;
            isFading = true;

            // Fade out active source
            float startVolumeA = activeSource.volume;
            float elapsed = 0f;

            // Setup inactive source dengan clip baru
            inactiveSource.clip = newClip;
            inactiveSource.volume = 0f;
            inactiveSource.Play();

            // Crossfade
            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;

                // Fade out old
                activeSource.volume = Mathf.Lerp(startVolumeA, 0f, t);

                // Fade in new
                inactiveSource.volume = Mathf.Lerp(0f, targetVolume, t);

                yield return null;
            }

            // Stop old source
            activeSource.Stop();
            activeSource.volume = 0f;

            // Swap sources
            AudioSource temp = activeSource;
            activeSource = inactiveSource;
            inactiveSource = temp;

            isFading = false;
        }

        private System.Collections.IEnumerator FadeOut(AudioSource source)
        {
            float startVolume = source.volume;
            float elapsed = 0f;

            while (elapsed < fadeDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / fadeDuration;
                source.volume = Mathf.Lerp(startVolume, 0f, t);
                yield return null;
            }

            source.Stop();
            source.volume = 0f;
        }
    }
}
