using UnityEngine;
using System.Collections;

public class FXController : MonoBehaviour {

    //components
    private ParticleSystem[] mPartSystem;

    //inspector
    [SerializeField]
    private float mPlaySpeed = 1.0f;

    [SerializeField]
    private float mLifeTime = 2.0f;

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
            Destroy(this.gameObject);
        
	}
}
