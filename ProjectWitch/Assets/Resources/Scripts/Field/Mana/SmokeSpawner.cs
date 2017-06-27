using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Field.Mana
{
    public class SmokeSpawner : MonoBehaviour
    {
        [SerializeField]
        private GameObject mPrefab = null;

        //生成間隔
        [SerializeField]
        private float mCreateSpan = 0.3f;

        public Transform Parent { get; set; }

        //前回生成からの時間
        private float mTime = 0.0f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            mTime += Time.deltaTime;

            if (mTime > mCreateSpan)
            {
                mTime = 0.0f;

                var inst = Instantiate(mPrefab);
                inst.transform.SetParent(Parent);
            }
        }
    }
}