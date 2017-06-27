using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Field.Mana
{
    public class SoldierSpawner : MonoBehaviour
    {

        //兵士の親オブジェクト
        [SerializeField]
        private GameObject mSoldierParent = null;

        //兵士のプレハブ
        [SerializeField]
        private GameObject mSoldierPrefab = null;

        //ケ森の親オブジェクト
        [SerializeField]
        private GameObject mSmokeParent = null;

        //生成の間隔
        [SerializeField]
        private float mSpawnSpan = 0.1f;

        //スポーン位置
        [SerializeField]
        private GameObject mSpawnTarget = null;

        //開始からの時間
        private float mTime = 0.0f;

        [SerializeField]
        private bool mSpawnEnable = false;

        // Update is called once per frame
        void Update()
        {
            if (!mSpawnEnable) return;

            mTime += Time.deltaTime;

            if (mTime > mSpawnSpan)
            {
                mTime = 0.0f;

                var inst = Instantiate(mSoldierPrefab);
                inst.transform.SetParent(mSoldierParent.transform, false);
                inst.transform.position = mSpawnTarget.transform.position;
                inst.GetComponent<Soldier>().SetSmokeParent(mSmokeParent.transform);
            }
        }

        public void SpawnStart()
        {
            mSpawnEnable = true;
        }

        public void SpawnStop()
        {
            mSpawnEnable = false;
        }
    }
}