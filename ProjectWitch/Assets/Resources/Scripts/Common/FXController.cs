using UnityEngine;
using System;

//ＦＸ終了イベント用のイベントハンドラ
public delegate void FXEventHandler();


public class FXController : MonoBehaviour {

    //components
    private ParticleSystem[] mPartSystem;

    //inspector
    //再生速度
    [SerializeField]
    private float mPlaySpeed = 1.0f;

    //寿命
    [SerializeField]
    private float mLifeTime = 2.0f;

    //event
    public event FXEventHandler EndEvent;

	// Use this for initialization
	void Start () {
        mPartSystem = GetComponentsInChildren<ParticleSystem>();

        for (int i = 0; i < mPartSystem.Length; i++)
            mPartSystem[i].playbackSpeed = mPlaySpeed;
	}
	
	// Update is called once per frame
	void Update () {

        //寿命管理
        mLifeTime -= Time.deltaTime;

        if (mLifeTime < 0)
        {
            EndEvent();
            Destroy(this.gameObject);
        }
	}
}
