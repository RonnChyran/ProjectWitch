using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Smoke : MonoBehaviour {
    
    [SerializeField]
    private Sprite[] mSprites = null;

    //速度
    [SerializeField]
    private float mSpeedY = 50.0f;

    //ライフタイム
    [SerializeField]
    private float mLifeTime = 1.0f;

    private Image mcImage = null;

    private Vector3 Position { get { return transform.position; } set { transform.position = value; } }

    private float mTransparent = 1.0f;
    private float mTime = 0.0f;

	// Use this for initialization
	void Start () {
        mcImage = GetComponent<Image>();
        mcImage.sprite = mSprites[UnityEngine.Random.Range(0, mSprites.Length)];
	}
	
	// Update is called once per frame
	void Update () {
        //時間の更新
        mTime += Time.deltaTime;
        if (mTime > mLifeTime) Destroy(this.gameObject);

        //透過度の更新
        mTransparent = (mTime / mLifeTime) * 0.5f + 0.5f;
        mcImage.color = new Color(1.0f, 1.0f, 1.0f, mTransparent);

        //位置の更新
        Position = new Vector3(Position.x, Position.y + mSpeedY * Time.deltaTime, Position.z);
        
	}
}
