using UnityEngine;

namespace ArcadiaOnline.Managers
{
    /// <summary>
    /// Manager untuk sound effects (hit, mati, langkah, dll).
    /// Attach ke GameObject "AudioManager" (sama dengan MapBGMManager).
    /// </summary>
    public class SFXManager : MonoBehaviour
    {
        [Header("SFX Clips")]
        [SerializeField] private AudioClip hitSound;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private AudioClip footstepSound;
        [SerializeField] private AudioClip levelUpSound;
        [SerializeField] private AudioClip pickupSound;
        [SerializeField] private AudioClip buttonClickSound;

        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.7f;
        [SerializeField] private int maxSimultaneousSFX = 5;

        [Header("Components")]
        [SerializeField] private AudioSource sfxSource;

        public static SFXManager Instance { get; private set; }

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

            // Setup SFX AudioSource
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
        }

        /// <summary>
        /// Play SFX satu kali.
        /// </summary>
        public void PlaySFX(AudioClip clip, float volumeMultiplier = 1f)
        {
            if (clip == null) return;
            sfxSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
        }

        // ========== Shortcut Methods ==========

        public void PlayHit()
        {
            PlaySFX(hitSound);
        }

        public void PlayDeath()
        {
            PlaySFX(deathSound);
        }

        public void PlayFootstep()
        {
            PlaySFX(footstepSound, 0.3f); // Lebih pelan
        }

        public void PlayLevelUp()
        {
            PlaySFX(levelUpSound);
        }

        public void PlayPickup()
        {
            PlaySFX(pickupSound);
        }

        public void PlayButtonClick()
        {
            PlaySFX(buttonClickSound, 0.5f);
        }

        /// <summary>
        /// Play SFX di posisi tertentu (3D sound).
        /// </summary>
        public void PlaySFXAtPosition(AudioClip clip, Vector3 position, float volume = 1f)
        {
            if (clip == null) return;
            AudioSource.PlayClipAtPoint(clip, position, sfxVolume * volume);
        }

        public void SetVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
    }
}
