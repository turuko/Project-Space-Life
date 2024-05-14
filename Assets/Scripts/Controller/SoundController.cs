using System;
using UnityEngine;

namespace Controller
{
    public class SoundController : MonoBehaviour
    {
        public static SoundController Instance;

        [SerializeField] private AudioSource musicSource, sfxSource;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public void PlaySound(AudioClip clip)
        {
            sfxSource.PlayOneShot(clip);
        }

        public void ChangeMasterVolume(float value)
        {
            AudioListener.volume = value;
        }

        public void ChangeMusicVolume(float value)
        {
            musicSource.volume = value;
        }

        public void ChangeSFXVolume(float value)
        {
            sfxSource.volume = value;
        }
    }
}