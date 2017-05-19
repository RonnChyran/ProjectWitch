using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch
{
    public class SEPlayer : MonoBehaviour
    {
        [SerializeField]
        private string mCueName = "";

        [SerializeField]
        private bool mPlayOnStart = true;

        // Use this for initialization
        void Start()
        {
            if (mPlayOnStart)
                Play();
        }

        public void Play()
        {
            var game = Game.GetInstance();
            game.SoundManager.Play(mCueName, SoundType.SE);
        }
    }
}