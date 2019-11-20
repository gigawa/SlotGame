using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts
{
    public class SoundController : MonoBehaviour
    {
        public StateMachine stateMachine;

        public AudioSource audioSource;

        public AudioClip reelSpinSound;
        public AudioClip reelStopSound;
        public AudioClip coinInSound;
        public AudioClip winSound;

        void Start()
        {
            foreach(var reel in stateMachine.reels)
            {
                reel.startReels += StartReelSpinSound;
                reel.stopReels += StopReelSpinSound;
            }

            audioSource = GetComponent<AudioSource>();
        }

        void StartReelSpinSound(Reel reel)
        {
            var audioSource = reel.gameObject.GetComponent<AudioSource>();
            audioSource.clip = reelSpinSound;
            audioSource.loop = true;
            audioSource.Play();
        }

        void StopReelSpinSound(Reel reel)
        {
            var audioSource = reel.gameObject.GetComponent<AudioSource>();
            audioSource.clip = reelStopSound;
            audioSource.loop = false;
            audioSource.Play();
        }

        public void PlaySmallWinSound()
        {
            audioSource.clip = winSound;
            audioSource.loop = false;
            audioSource.Play();
        }
    }
}
