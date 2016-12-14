using UnityEngine;
using System.Collections;

namespace ProjectWitch.Talk
{
    public class GameObjectManager : MonoBehaviour
    {
        //ゲームオブジェクトのパス
        [SerializeField]
        private string mGameObjectPath = "Prefabs/Talk/GameObject/";
        //ゲームオブジェクトを生成する親
        [SerializeField]
        private GameObject mGameObjectParent = null;

        //内部変数
        private GameObject[] mGameObjects = new GameObject[64];

        //ゲームオブジェクトの生成
        public void CreateGameObject(int id, string path, Vector2 pos, out string error)
        {
            error = null;
            if (!CheckIdUsage(id))
            {
                error = "idが無効です。0~63であることを確認してください。";
                return;
            }

            DeleteGameObject(id, out error);
            var obj = Resources.Load(mGameObjectPath + path) as GameObject;

            if(!obj)
            {
                error = path + "を見つけられません。" + mGameObjectPath + path + "を確認してください。";
                return;
            }

            var inst = Instantiate(obj);
            inst.transform.SetParent(mGameObjectParent.transform, false);
            inst.transform.localPosition = pos;
            mGameObjects[id] = inst;
        }

        //ゲームオブジェクトを削除
        public void DeleteGameObject(int id, out string error)
        {
            error = null;
            if(!CheckIdUsage(id))
            {
                error = "idが無効です。0~63の範囲であることを確認してください。";
                return;
            }
            if (mGameObjects[id])
                Destroy(mGameObjects[id]);
        }

        //idの有効性をチェック
        private bool CheckIdUsage(int id)
        {
            if (id < 0) return false;
            if (id >= mGameObjects.Length) return false;
            return true;
        }
    }
}