using UnityEngine;
using UnityEngine.Events;
using System;

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
    public UnityEvent EndEvent=null;

    // Use this for initialization
    void Start()
    {
        mPartSystem = GetComponentsInChildren<ParticleSystem>();
        mAnimators = GetComponentsInChildren<Animator>();

        foreach(var part in mPartSystem)
            part.playbackSpeed = mPlaySpeed;

        foreach(var anim in mAnimators)
            anim.speed = mPlaySpeed;
    }

    // Update is called once per frame
    void Update()
    {

        //寿命管理
        mLifeTime -= Time.deltaTime;

        if (mLifeTime < 0)
        {
            if(EndEvent != null)EndEvent.Invoke();
            Destroy(this.gameObject);
        }
    }
}