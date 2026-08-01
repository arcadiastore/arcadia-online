using System.Collections.Generic;
using UnityEngine;
using ArcadiaOnline.Core;

namespace ArcadiaOnline.Managers
{
    /// <summary>Lihat docs/02_TDD/GameManagers.md dan docs/01_GDD/27_Audio.md.</summary>
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] private AudioSource _bgmSource;
        [SerializeField] private AudioSource _sfxSource;
        [SerializeField] private AudioSource _voiceSource;

        [SerializeField] private List<NamedClip> _bgmClips = new List<NamedClip>();
        [SerializeField] private List<NamedClip> _sfxClips = new List<NamedClip>();
        [SerializeField] private List<NamedClip> _voiceClips = new List<NamedClip>();

        public void PlayBGM(string bgmName)
        {
            AudioClip clip = FindClip(_bgmClips, bgmName);
            if (clip == null) return;
            _bgmSource.clip = clip;
            _bgmSource.loop = true;
            _bgmSource.Play();
        }

        public void PlaySFX(string sfxName)
        {
            AudioClip clip = FindClip(_sfxClips, sfxName);
            if (clip == null) return;
            _sfxSource.PlayOneShot(clip);
        }

        public void PlayVoice(string voiceName)
        {
            AudioClip clip = FindClip(_voiceClips, voiceName);
            if (clip == null) return;
            _voiceSource.PlayOneShot(clip);
        }

        public void StopBGM() => _bgmSource.Stop();

        public void SetVolume(AudioType type, float volume)
        {
            volume = Mathf.Clamp01(volume);
            switch (type)
            {
                case AudioType.BGM: _bgmSource.volume = volume; break;
                case AudioType.SFX: _sfxSource.volume = volume; break;
                case AudioType.Voice: _voiceSource.volume = volume; break;
            }
        }

        private AudioClip FindClip(List<NamedClip> clips, string name)
        {
            var found = clips.Find(c => c.name == name);
            if (found == null)
            {
                Debug.LogWarning($"Audio clip '{name}' tidak ditemukan.");
                return null;
            }
            return found.clip;
        }
    }

    public enum AudioType { BGM, SFX, Voice }

    [System.Serializable]
    public class NamedClip
    {
        public string name;
        public AudioClip clip;
    }
}
