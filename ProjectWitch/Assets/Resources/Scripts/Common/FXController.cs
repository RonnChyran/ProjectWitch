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
        private ParticleSystemRenderer[] mPartRenderer;
        private Animator[] mAnimators;
        private MeshRenderer[] mMeshRenderer;

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
            mPartRenderer = GetComponentsInChildren<ParticleSystemRenderer>();
            mAnimators = GetComponentsInChildren<Animator>();
            mMeshRenderer = GetComponentsInChildren<MeshRenderer>();

            foreach (var part in mPartSystem)
            {
                var ma = part.main;
                ma.simulationSpeed = mPlaySpeed;
            }

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

            //終了イベント発行
            if (EndEvent != null) EndEvent.Invoke();

            if (mPartSystem.Length > 0 || mMeshRenderer.Length > 0)
            {
                for (float i = 0; i < 1.0f; i += 0.1f)
                {
                    //パーティクルのフェードアウト処理
                    foreach (var partsys in mPartRenderer)
                    {
                        var ma = partsys.material;
                        if (!ma.HasProperty("_TintColor")) continue; //tint colorプロパティのないパーティクルは無視

                        var color = ma.GetColor("_TintColor");
                        color.a = Math.Max(0.0f, color.a - i);
                        ma.SetColor("_TintColor",color);
                    }

                    //メッシュのフェードアウト処理
                    foreach(var mesh in mMeshRenderer)
                    {
                        var mat = mesh.material;
                        if (!mat.HasProperty("_Color")) continue;

                        var color = mat.GetColor("_Color");
                        color.a = Math.Max(0.0f, color.a - i);
                        mat.SetColor("_Color", color);
                    }
                    yield return null;
                }
            }


            Destroy(this.gameObject);
            yield return null;
        }
    }
}