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
        
        private Dictionary<string, Sound> soundDictionary;
        private int currentMusicIndex = 0;
        
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                // Проверяем, является ли объект корневым перед применением DontDestroyOnLoad
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
            soundDictionary = new Dictionary<string, Sound>();
            
            foreach (Sound sound in sounds)
            {
                soundDictionary[sound.name] = sound;
            }
            
            // Start playing music
            if (musicSource != null && musicTracks.Length > 0)
            {
                PlayMusicTrack(0);
            }
        }
        
        public void PlaySound(string soundName)
        {
            // Если звук не найден, просто игнорируем его без предупреждения
            if (soundSource == null || soundDictionary == null) return;
            
            if (soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                soundSource.pitch = sound.pitch;
                soundSource.PlayOneShot(sound.clip, sound.volume);
            }
            // Убираем предупреждение о ненайденных звуках
        }
        
        public void PlaySound(string soundName, float volume)
        {
            // Если звук не найден, просто игнорируем его без предупреждения
            if (soundSource == null || soundDictionary == null) return;
            
            if (soundDictionary.TryGetValue(soundName, out Sound sound))
            {
                soundSource.pitch = sound.pitch;
                soundSource.PlayOneShot(sound.clip, volume);
            }
            // Убираем предупреждение о ненайденных звуках
        }
        
        public void PlayMusicTrack(int trackIndex)
        {
            if (musicSource == null || musicTracks.Length == 0) return;
            
            if (trackIndex >= 0 && trackIndex < musicTracks.Length)
            {
                musicSource.clip = musicTracks[trackIndex];
                musicSource.loop = true;
                musicSource.Play();
                currentMusicIndex = trackIndex;
            }
        }
        
        public void PlayNextMusicTrack()
        {
            currentMusicIndex = (currentMusicIndex + 1) % musicTracks.Length;
            PlayMusicTrack(currentMusicIndex);
        }
        
        public void SetSoundVolume(float volume)
        {
            if (soundSource != null)
            {
                soundSource.volume = volume;
            }
        }
        
        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
            {
                musicSource.volume = volume;
            }
        }
        
        public void StopMusic()
        {
            if (musicSource != null)
            {
                musicSource.Stop();
            }
        }
        
        public void PauseMusic()
        {
            if (musicSource != null)
            {
                musicSource.Pause();
            }
        }
        
        public void ResumeMusic()
        {
            if (musicSource != null)
            {
                musicSource.UnPause();
            }
        }
        
        // Новый метод для проверки существования звука
        public bool HasSound(string soundName)
        {
            return soundDictionary != null && soundDictionary.ContainsKey(soundName);
        }
    }
}