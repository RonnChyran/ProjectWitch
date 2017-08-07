using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Field.Mana
{
    [RequireComponent(typeof(Animator))]
    public class Message : MonoBehaviour
    {
        [SerializeField]
        private Text mcText = null;
        public string Text { set { mcText.text = value; } }

        [SerializeField]
        private float mLifeTime = 1.0f;

        //animatorコンポーネント
        private Animator mcAnimator = null;

        private float mTime = 0.0f;

        private void Start()
        {
            mcAnimator = GetComponent<Animator>();  
        }

        // Update is called once per frame
        void Update()
        {
            mTime += Time.deltaTime;
            if (mTime > mLifeTime) Close();
            

        }

        void Close()
        {
            mcAnimator.SetBool("IsShow", false);
        }

        //アニメーションによるイベントの呼び出しで削除
        public void AnimationEnd()
        {
            Destroy(this.gameObject);
        }
    }
}