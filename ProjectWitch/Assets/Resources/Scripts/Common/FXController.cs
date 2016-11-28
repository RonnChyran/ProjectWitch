using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System;


namespace ProjectWitch
{
    //ＦＸ終了イベント用のイベントハンドラ
    public delegate void FXEventHandler();

    public class FXController : MonoBehaviour
    {

        //components
        private ParticleSystem[] mPartSystem;
        private Animator[] mAnimators;

        //inspector
        //再生速度
        [SerializeField]
        private float mPlaySpeed = 1.0f;
        public float PlaySpeed { get { return mPlaySpeed; } set { mPlaySpeed = value; } }

        //寿命
        [SerializeField]
        private float mLifeTime = 2.0f;
        public float LifeTime { get { return mLifeTime; } set { mLifeTime = value; } }

        //event
        [SerializeField]
        public UnityEvent EndEvent = null;

        //コルーチンが走っているかどうかのフラグ
        private bool mCoIsRunning = false;

        // Use this for initialization
        void Start()
        {
            mPartSystem = GetComponentsInChildren<ParticleSystem>();
            mAnimators = GetComponentsInChildren<Animator>();

            foreach (var part in mPartSystem)
                part.playbackSpeed = mPlaySpeed;

            foreach (var anim in mAnimators)
                anim.speed = mPlaySpeed;
        }

        // Update is called once per frame
        void Update()
        {

            //寿命管理
            mLifeTime -= Time.deltaTime;

            if (mLifeTime < 0 && mCoIsRunning == false)
            {
                StartCoroutine(Delete());
            }
        }

        private IEnumerator Delete()
        {
            mCoIsRunning = true;

            if (EndEvent != null) EndEvent.Invoke();
            for (float i = 0; i < 1.0f; i += 0.1f)
            {
                foreach (var partsys in mPartSystem)
                {
                    var color = partsys.startColor;
                    color.a = Math.Max(0.0f, color.a - i);
                    partsys.startColor = color;
                }
                yield return null;
            }


            Destroy(this.gameObject);
            yield return null;
        }
    }
}