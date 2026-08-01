using UnityEngine;
using System.Collections.Generic;

namespace ArcadiaOnline.Managers
{
    /// <summary>
    /// Sistem SFX berdasarkan Job, Gender, dan Action.
    /// Contoh: Warrior_Male_Run, Mage_Female_Hit, dll.
    /// </summary>
    public class JobSFXManager : MonoBehaviour
    {
        [System.Serializable]
        public class JobSFX
        {
            public string jobId; // warrior, mage, archer, dll
            public GenderClips maleClips;
            public GenderClips femaleClips;
        }

        [System.Serializable]
        public class GenderClips
        {
            public AudioClip run;
            public AudioClip hit;
            public AudioClip death;
            public AudioClip skill1;
            public AudioClip skill2;
            public AudioClip skill3;
            public AudioClip levelUp;
            public AudioClip hurt; // Saat terkena damage
        }

        [Header("Job SFX List")]
        [SerializeField] private List<JobSFX> jobSFXList = new List<JobSFX>();

        [Header("Default SFX (Jika job tidak punya SFX)")]
        [SerializeField] private AudioClip defaultRun;
        [SerializeField] private AudioClip defaultHit;
        [SerializeField] private AudioClip defaultDeath;
        [SerializeField] private AudioClip defaultSkill;
        [SerializeField] private AudioClip defaultHurt;

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
        /// Play SFX berdasarkan job, gender, dan action.
        /// </summary>
        public void PlayJobSFX(string jobId, string gender, string action)
        {
            AudioClip clip = GetClip(jobId, gender, action);
            if (clip != null)
            {
                sfxSource.PlayOneShot(clip, sfxVolume);
            }
            else
            {
                Debug.LogWarning($"[JobSFX] Clip tidak ditemukan: {jobId}_{gender}_{action}");
            }
        }

        /// <summary>
        /// Play SFX di posisi tertentu (3D sound).
        /// </summary>
        public void PlayJobSFXAtPosition(string jobId, string gender, string action, Vector3 position)
        {
            AudioClip clip = GetClip(jobId, gender, action);
            if (clip != null)
            {
                AudioSource.PlayClipAtPoint(clip, position, sfxVolume);
            }
        }

        private AudioClip GetClip(string jobId, string gender, string action)
        {
            // Cari job SFX
            JobSFX jobSFX = jobSFXList.Find(j => j.jobId.ToLower() == jobId.ToLower());
            if (jobSFX == null)
            {
                Debug.LogWarning($"[JobSFX] Job '{jobId}' tidak ditemukan!");
                return GetDefaultClip(action);
            }

            // Pilih gender
            GenderClips genderClips = gender.ToLower() == "male" ? jobSFX.maleClips : jobSFX.femaleClips;
            if (genderClips == null)
            {
                Debug.LogWarning($"[JobSFX] Gender '{gender}' tidak ditemukan untuk job '{jobId}'!");
                return GetDefaultClip(action);
            }

            // Ambil clip berdasarkan action
            switch (action.ToLower())
            {
                case "run":
                case "walk":
                case "footstep":
                    return genderClips.run;
                case "hit":
                case "attack":
                    return genderClips.hit;
                case "death":
                case "die":
                    return genderClips.death;
                case "skill1":
                    return genderClips.skill1;
                case "skill2":
                    return genderClips.skill2;
                case "skill3":
                    return genderClips.skill3;
                case "levelup":
                    return genderClips.levelUp;
                case "hurt":
                case "damaged":
                    return genderClips.hurt;
                default:
                    Debug.LogWarning($"[JobSFX] Action '{action}' tidak dikenali!");
                    return GetDefaultClip(action);
            }
        }

        private AudioClip GetDefaultClip(string action)
        {
            switch (action.ToLower())
            {
                case "run":
                case "walk":
                case "footstep":
                    return defaultRun;
                case "hit":
                case "attack":
                    return defaultHit;
                case "death":
                case "die":
                    return defaultDeath;
                case "skill1":
                case "skill2":
                case "skill3":
                    return defaultSkill;
                case "hurt":
                case "damaged":
                    return defaultHurt;
                default:
                    return null;
            }
        }

        public void SetVolume(float volume)
        {
            sfxVolume = Mathf.Clamp01(volume);
        }
    }
}
