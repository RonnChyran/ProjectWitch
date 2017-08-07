using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Sound
{
    public class SoundPlayer : MonoBehaviour
    {
        //サウンドの種類
        [SerializeField]
        private SoundType mType = SoundType.BGM;

        //サウンド名
        [SerializeField]
        private string mName = "";

        //生成時に再生するか
        [SerializeField]
        private bool mPlayOnAwake = false;

        // Use this for initialization
        void Start()
        {
            if (mPlayOnAwake)
                Game.GetInstance().SoundManager.Play(mName, mType);
        }

        // Update is called once per frame
        public void Play()
        {
            Game.GetInstance().SoundManager.Play(mName, mType);
        }
    }
}
