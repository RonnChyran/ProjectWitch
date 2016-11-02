//=====================================
//author	:shotta
//summary	:立ち絵の位置関係とかを把握
//=====================================

using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System; //Exception
using System.Text.RegularExpressions;
using UnityEngine.SceneManagement;

namespace Scenario
{
    class StandCGController : MonoBehaviour
    {
        //	立ち絵管理用の配列
        private GameObject[] mCGArray = new GameObject[16];

        //立ち絵表示用のプレハブ
        [SerializeField]
        private GameObject mCGPrefab = null;

        public GameObject GetStandCG(int id, out string error)
        {
            error = null;

            if (!(0 <= id && id < mCGArray.Length))
            {
                error = "IDが許容範囲を超えています(" + id + ")";
                return null;
            }

            return mCGArray[id];
        }

        //立ち絵のデータを追加(まだ表示はしない)
        public void AddStandCG(int id, string path, out string error)
        {
            error = null;

            if (!(0 <= id && id < mCGArray.Length))
            {
                error = "IDが許容範囲を超えています(" + id + ")";
                return;
            }
            GameObject obj = mCGArray[id];

            if (obj == null)
                Destroy(obj);

            obj = Instantiate(mCGPrefab) as GameObject;
            obj.SetActive(false);
            obj.GetComponent<RawImage>().texture = Resources.Load(path) as Texture2D;
            obj.GetComponent<RawImage>().SetNativeSize();
            obj.transform.SetParent(this.transform);

            mCGArray[id] = obj;
        }

        //空きのある立ち絵の位置を取得
        public Vector3 GetUnduplicatePosition(Vector3 position)
        {
            Vector3 cgPosition = position;
            for (int i = 0; ; i++)
            {
                float distance = 120.0f * i;
                float bias = 1.0f;
                float offsetX = bias * distance;
                cgPosition = position + new Vector3(offsetX, 0.0f, 0.0f);
                if (!IsDuplicatePosition(cgPosition))
                    break;
            }
            return cgPosition;
        }

        //立ち絵を表示
        public void ShowStandCG(int id, bool isShowFront, out string error)
        {
            error = null;
            if (!(0 <= id && id < mCGArray.Length))
            {
                error = "IDが許容範囲を超えています(" + id + ")";
                return;
            }
            GameObject obj = mCGArray[id];
            if (obj == null)
            {
                error = "ID=" + id + "の立ち絵が存在しません";
                return;
            }

            if (!isShowFront)
                obj.transform.SetAsFirstSibling();
            else
                obj.transform.SetAsLastSibling();
            obj.SetActive(true);
        }

        //立ち絵を非表示
        public void HideStandCG(int id, out string error)
        {
            error = null;
            if (!(0 <= id && id < mCGArray.Length))
            {
                error = "IDが許容範囲を超えています(" + id + ")";
                return;
            }
            GameObject obj = mCGArray[id];
            if (obj == null)
            {
                error = "ID=" + id + "の立ち絵が存在しません";
                return;
            }
            obj.SetActive(false);
        }

        //位置の重複がないか検索
        private bool IsDuplicatePosition(Vector3 position)
        {
            bool isDuplicate = false;
            for (int i = 0; i < mCGArray.Length; i++)
            {
                GameObject obj = mCGArray[i];
                if (obj == null)
                    continue;
                if (obj.activeSelf == false)
                    continue;
                if (Vector3.Distance(obj.transform.localPosition, position) <= 10)
                {
                    isDuplicate = true;
                    break;
                }
            }
            return isDuplicate;
        }
    }
}