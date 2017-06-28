using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Field.Mana
{
    public class MessageSpawner : MonoBehaviour
    {
        //生成感覚
        [SerializeField]
        private float mSpawnSpan = 1.0f;

        //メッセージオブジェクトの親
        [SerializeField]
        private Transform mMessageParent = null;

        //生成するプレハブたち
        [SerializeField]
        private GameObject[] mPrefabs = null;

        //位置のオフセット
        [SerializeField]
        private Vector2 mPositionOffset = new Vector2(50.0f, 200.0f);

        //ランダムで選ぶメッセージ
        [SerializeField]
        private string[] mMessages = null;

        //生成できるかのフラグ
        [SerializeField]
        private bool mSpawnEnable = false;

        private float mTime = 0.0f;

        //前回選んだプレハブの番号
        private int mLastPrefabIndex = 0;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if (!mSpawnEnable) return;

            mTime += Time.deltaTime;
            if(mTime > mSpawnSpan)
            {
                mTime = 0.0f;
                var index = UnityEngine.Random.Range(0, mPrefabs.Length);
                if (index == mLastPrefabIndex) index++;
                if (index >= mPrefabs.Length) index = 0;
                mLastPrefabIndex = index;

                var inst = Instantiate(mPrefabs[index]);
                inst.GetComponent<Message>().Text = mMessages[UnityEngine.Random.Range(0, mMessages.Length)];
                inst.transform.SetParent(mMessageParent, false);
                inst.transform.position += new Vector3(UnityEngine.Random.Range(-mPositionOffset.x, mPositionOffset.x),
                                                        UnityEngine.Random.Range(-mPositionOffset.y, mPositionOffset.y), 0.0f);
            }
        }

        public void StartSpawn()
        {
            mSpawnEnable = true;
        }

        public void StopSpawn()
        {
            mSpawnEnable = false;
        }
    }
}