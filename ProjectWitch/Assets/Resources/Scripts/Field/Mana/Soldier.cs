using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Field.Mana
{
    public class Soldier : MonoBehaviour
    {
        //ライフタイム
        [SerializeField]
        private float mLifeTime = 3.0f;

        //開始からの時間
        private float mTime = 0.0f;

        //x速度
        [SerializeField]
        private float mSpeedX = 3.0f;

        //煙発生器
        [SerializeField]
        private SmokeSpawner mSmokeSpawner = null;
        
        //煙の親オブジェクトをセット
        public void SetSmokeParent(Transform t)
        {
            mSmokeSpawner.Parent = t;
        }

        // Use this for initialization
        void Start()
        {
            mSpeedX = UnityEngine.Random.Range(mSpeedX, mSpeedX + 50f);
        }

        // Update is called once per frame
        void Update()
        {
            mTime += Time.deltaTime;
            if (mTime > mLifeTime)
                Destroy(this.gameObject);

            //移動
            transform.position = new Vector3(transform.position.x + mSpeedX * Time.deltaTime,
                                                transform.position.y, transform.position.z);
        }
    }
}