using UnityEngine;
using System.Collections;

namespace ProjectWitch
{
    //Gameクラスから直接呼び出すようのTalkコマンド
    public class TalkCommandHelper : MonoBehaviour
    {
        //カーソル用のキャンバス
        [SerializeField]
        private Canvas mCanvas = null;

        //強調カーソルのプレハブ
        [SerializeField]
        private GameObject mCursor = null;

        //強調カーソルのインスタンス
        GameObject mInstCursor = null;

        //バトルのチュートリアルモードをオンにする
        public void EnableBattleTutorial()
        {
            Game.GetInstance().BattleIn.IsTutorial = true;
        }

        //カーソルの非表示
        public void HideCursor()
        {
            Cursor.visible = false;
        }

        //カーソルの表示
        public void ShowCursor()
        {
            Cursor.visible = true;
        }

        //強調カーソルの表示
        //pos:0~100% 左上を0とした画面に対する相対位置
        public void ShowAccentCursor(Vector2 pos)
        {
            //前のカーソルを消す
            HideAccentCursor();

            mInstCursor = Instantiate(mCursor);
            mInstCursor.transform.SetParent(mCanvas.transform, false);

            //なぜか再アクティブしないと描画されないことがある（Unityのバグか？)
            mInstCursor.SetActive(false);
            mInstCursor.SetActive(true);

            //位置の調整
            var rect = mCanvas.GetComponent<RectTransform>().rect;
            var adjustPos = new Vector3(0.0f, 0.0f);
            adjustPos.x = pos.x / 100.0f * rect.width;
            adjustPos.y = pos.y / 100.0f * rect.height;
            adjustPos = adjustPos + new Vector3(rect.x, rect.y);

            mInstCursor.transform.localPosition = adjustPos;
        }

        //強調カーソルの非表示
        public void HideAccentCursor()
        {
            if (mInstCursor) Destroy(mInstCursor);
        }
    }
}