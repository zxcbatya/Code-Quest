using UnityEngine;
using System.Collections.Generic;

namespace Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }
        
        [System.Serializable]
        public class Sound
        {
            public string name;
            public AudioClip clip;
            [Range(0f, 1f)] public float volume = 1f;
            [Range(0.1f, 3f)] public float pitch = 1f;
        }
        
        [Header("Audio Settings")]
        [SerializeField] private AudioSource soundSource;
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private Sound[] sounds;
        [SerializeField] private AudioClip[] musicTracks;
        
        private Dictionary<string, Sound> _soundDictionary;
        private int _currentMusicIndex = 0;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                if (transform.parent == null)
                {
                    DontDestroyOnLoad(gameObject);
                }
                InitializeSounds();
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        private void InitializeSounds()
        {
            _soundDictionary = new Dictionary<string, Sound>();
            
            foreach (Sound sound in sounds)
            {
                _soundDictionary[sound.name] = sound;
            }
            
            if (musicSource != null && musicTracks.Length > 0)
            {
                PlayMusicTrack(0);
            }
        }
        
        public void PlaySound(string soundName)
        {
            if (soundSource == null || _soundDictionary == null) return;
            
            if (_soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                soundSource.pitch = sound.pitch;
                soundSource.PlayOneShot(sound.clip, sound.volume);
            }
        }
        
        public void PlayMusicTrack(int trackIndex)
        {
            if (musicSource == null || musicTracks.Length == 0) return;
            
            if (trackIndex >= 0 && trackIndex < musicTracks.Length)
            {
                musicSource.clip = musicTracks[trackIndex];
                musicSource.loop = true;
                musicSource.Play();
                _currentMusicIndex = trackIndex;
            }
        }
        public bool HasSound(string soundName)
        {
            return _soundDictionary != null && _soundDictionary.ContainsKey(soundName);
        }
    }
}