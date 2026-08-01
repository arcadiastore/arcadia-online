using UnityEngine;

namespace ArcadiaOnline.Managers
{
    /// <summary>
    /// Sistem SFX berdasarkan Gender saja.
    /// Semua job pakai suara yang sama, beda male/female.
    /// </summary>
    public class JobSFXManager : MonoBehaviour
    {
        [System.Serializable]
        public class GenderClips
        {
            public AudioClip run;
            public AudioClip hit;
            public AudioClip death;
            public AudioClip skill;
            public AudioClip hurt;
            public AudioClip levelUp;
        }

        [Header("Male SFX")]
        [SerializeField] private GenderClips maleClips;

        [Header("Female SFX")]
        [SerializeField] private GenderClips femaleClips;

        [Header("Default (Jika gender tidak diketahui)")]
        [SerializeField] private GenderClips defaultClips;

        [Header("Settings")]
        [SerializeField] [Range(0f, 1f)] private float sfxVolume = 0.7f;

        [Header("Components")]
        [SerializeField] private AudioSource sfxSource;

        public static JobSFXManager Instance { get; private set; }

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

            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.playOnAwake = false;
            }
        }

        /// <summary>
        /// Play SFX berdasarkan gender dan action.
        /// Contoh: PlaySFX("male", "hit")
        /// </summary>
        public void PlaySFX(string gender, string action)
        {
            AudioClip clip = GetClip(gender, action);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
            else
            {
                Debug.LogWarning($"[JobSFX] Clip tidak ditemukan: {gender}_{action}");
            }
        }

        /// <summary>
        /// Play SFX di posisi tertentu (3D sound).
        /// </summary>
        public void PlaySFXAtPosition(string gender, string action, Vector3 position)
        {
            AudioClip clip = GetClip(gender, action);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
            }
        }

        /// <summary>
        /// Shortcut: Play run SFX.
        /// </summary>
        public void PlayRun(string gender)
        {
            PlaySFX(gender, "run");
        }

        /// <summary>
        /// Shortcut: Play hit SFX.
        /// </summary>
        public void PlayHit(string gender)
        {
            PlaySFX(gender, "hit");
        }

        /// <summary>
        /// Shortcut: Play death SFX.
        /// </summary>
        public void PlayDeath(string gender)
        {
            PlaySFX(gender, "death");
        }

        /// <summary>
        /// Shortcut: Play skill SFX.
        /// </summary>
        public void PlaySkill(string gender)
        {
            PlaySFX(gender, "skill");
        }

        /// <summary>
        /// Shortcut: Play hurt SFX (saat terkena damage).
        /// </summary>
        public void PlayHurt(string gender)
        {
            PlaySFX(gender, "hurt");
        }

        /// <summary>
        /// Shortcut: Play level up SFX.
        /// </summary>
        public void PlayLevelUp(string gender)
        {
            PlaySFX(gender, "levelup");
        }

        private AudioClip GetClip(string gender, string action)
        {
            // Pilih gender clips
            GenderClips clips;
            switch (gender.ToLower())
            {
                case "male":
                    clips = maleClips;
                    break;
                case "female":
                    clips = femaleClips;
                    break;
                default:
                    Debug.LogWarning($"[JobSFX] Gender '{gender}' tidak dikenali, pakai default");
                    clips = defaultClips;
                    break;
            }

            if (clips == null)
            {
                clips = defaultClips;
            }

            if (clips == null)
            {
                return null;
            }

            // Ambil clip berdasarkan action
            switch (action.ToLower())
            {
                case "run":
                case "walk":
                case "footstep":
                    return clips.run;
                case "hit":
                case "attack":
                    return clips.hit;
                case "death":
                case "die":
                    return clips.death;
                case "skill":
                case "skill1":
                case "skill2":
                case "skill3":
                    return clips.skill;
                case "hurt":
                case "damaged":
                    return clips.hurt;
                case "levelup":
                case "level":
                    return clips.levelUp;
                default:
                    Debug.LogWarning($"[JobSFX] Action '{action}' tidak dikenali!");
                    return null;
            }
        }

        public void SetVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
    }
}
