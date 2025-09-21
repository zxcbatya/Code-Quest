using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [Header("Audio Sources")]
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioSource sfxSource;

        [Header("Sound Effects")]
        [SerializeField] private AudioClip dragStartSound;
        [SerializeField] private AudioClip dropSuccessSound;
        [SerializeField] private AudioClip dropFailSound;
        [SerializeField] private AudioClip buttonClickSound;
        [SerializeField] private AudioClip robotMoveSound;
        [SerializeField] private AudioClip robotTurnSound;
        [SerializeField] private AudioClip successSound;
        [SerializeField] private AudioClip failSound;

        private Dictionary<string, AudioClip> soundDict = new Dictionary<string, AudioClip>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
                InitializeSounds();
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void InitializeSounds()
        {
            soundDict["drag_start"] = dragStartSound;
            soundDict["drop_success"] = dropSuccessSound;
            soundDict["drop_fail"] = dropFailSound;
            soundDict["button_click"] = buttonClickSound;
            soundDict["robot_move"] = robotMoveSound;
            soundDict["robot_turn"] = robotTurnSound;
            soundDict["success"] = successSound;
            soundDict["fail"] = failSound;
        }

        public void PlaySound(string soundName)
        {
            if (soundDict.TryGetValue(soundName, out AudioClip clip) && clip != null)
            {
                sfxSource.PlayOneShot(clip);
            }
        }

        public void PlayMusic(AudioClip musicClip, bool loop = true)
        {
            if (musicSource != null && musicClip != null)
            {
                musicSource.clip = musicClip;
                musicSource.loop = loop;
                musicSource.Play();
            }
        }

        public void SetMusicVolume(float volume)
        {
            if (musicSource != null)
            {
                musicSource.volume = Mathf.Clamp01(volume);
            }
        }

        public void SetSFXVolume(float volume)
        {
            if (sfxSource != null)
            {
                sfxSource.volume = Mathf.Clamp01(volume);
            }
        }
    }
}