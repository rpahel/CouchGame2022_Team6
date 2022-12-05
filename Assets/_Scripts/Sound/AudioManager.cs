using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
        private static AudioManager Instance { get; set; }
        public Sound[] sounds;
        private void Awake()
        {
                if (Instance != null)
                {
                        Debug.Log("More than one instance of AudioManager");
                }
                else
                {
                        Instance = this;
                        DontDestroyOnLoad(Instance);
                }
                
                foreach (var s in sounds) 
                {
                        s.source = gameObject.AddComponent<AudioSource>();
                        s.source.clip = s.clip;
                        s.source.outputAudioMixerGroup = s.audiogroup;
                        s.source.volume = s.volume;
                        s.source.pitch = s.pitch;
                        s.source.loop = s.loop;
                }
        }

        public void Play(string name)
        {
                var s = Array.Find(sounds, sound => sound.name == name);
                
                if (s == null)
                {
                        Debug.LogWarning("Sound :" + name + "not found!");
                        return;
                }
                
                s.source.Play();
        }
}
