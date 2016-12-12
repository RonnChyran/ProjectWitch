using UnityEngine;
using System.Collections;

namespace ProjectWitch.Talk
{
    public class TextWindowManager : MonoBehaviour {

        //デフォルトのスキン
        [SerializeField]
        private TextWindow mDefSkin = null;

        //内部変数
        private TextWindow mCurrentWindow = null;

        //プロパティ
        public TextWindow Window { get { return mCurrentWindow; } private set { } }

        private void Start()
        {
            mCurrentWindow = mDefSkin;
        }

        //ウィンドウスキンを変更する
        public void ChangeSkin(string path)
        {
            if (mCurrentWindow)
                Destroy(mCurrentWindow.gameObject);

            var res = Resources.Load(path) as GameObject;
            if (res == null)
            {
                Debug.LogError("スキンが見つかりませんでした。" + path + "が存在することを確認してください");
                return;
            }

            var inst = Instantiate(res);
            inst.transform.SetParent(transform, false);

            mCurrentWindow = inst.GetComponent<TextWindow>();
        }
    }
}