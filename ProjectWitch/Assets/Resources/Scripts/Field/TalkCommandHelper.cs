using UnityEngine;
using System.Collections;

namespace ProjectWitch.Field
{
    public class TalkCommandHelper : MonoBehaviour
    {
        //フィールドコントローラ
        [SerializeField]
        private FieldController mFieldController = null;

        //エリアウィンドウのプレハブ
        [SerializeField]
        private GameObject mAreaWindow = null;
        [SerializeField]
        private GameObject mAreaNameWindow = null;

        //強調カーソルのプレハブ
        [SerializeField]
        private GameObject mCursor = null;

        //ある処理が終了したときに呼び出されるメソッド
        public delegate void EndCallBack();

        //エリアウィンドウのインスタンス
        GameObject mInstAreaWindow = null;
        GameObject mInstAreaName = null;

        //強調カーソルのインスタンス
        GameObject mInstCursor = null;

        //指定エリアの位置にカメラを移動させてハイライトする
        public void HilightArea(int area, EndCallBack callback)
        {
            StartCoroutine(_HilightArea(area, callback));
        }

        //エリアウィンドウを開く
        public void OpenAreaWindow(int area)
        {
            CloseAreaWindow();

            //描画先のキャンバス
            var canvas = mFieldController.FieldUIController.CameraCanvas;

            //エリア名リボンを生成
            mInstAreaName = Instantiate(mAreaNameWindow);
            mInstAreaName.transform.SetParent(canvas.transform, false);

            var cAreaName = mInstAreaName.GetComponent<AreaName>();
            cAreaName.AreaID = area;
            cAreaName.Init();

            //メニュープレハブを生成
            mInstAreaWindow = Instantiate(mAreaWindow);
            mInstAreaWindow.transform.SetParent(canvas.transform, false);

            var cAreaWindow = mInstAreaWindow.GetComponent<AreaWindow>();
            cAreaWindow.AreaID = area;
            cAreaWindow.FieldController = mFieldController;
            cAreaWindow.FieldUIController = mFieldController.FieldUIController;
            cAreaWindow.NameWindow = mInstAreaName;
            cAreaWindow.Init();

            //seの再生
            Game.GetInstance().SoundManager.PlaySE(SE.Click);
        }

        //エリアウィンドウを閉じる
        public void CloseAreaWindow()
        {
            if(mInstAreaName)
                Destroy(mInstAreaName);
            if (mInstAreaWindow)
            {
                Destroy(mInstAreaWindow);
                Game.GetInstance().SoundManager.PlaySE(SE.Cancel);
            }
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
            //描画先のキャンバス
            var canvas = mFieldController.FieldUIController.CameraCanvas;
            
            HideAccentCursor();
            mInstCursor = Instantiate(mCursor);
            mInstCursor.transform.SetParent(canvas.transform, false);

            //位置の調整
            var rect = canvas.GetComponent<RectTransform>().rect;
            var adjustPos = new Vector3(0.0f,0.0f);
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

        //コルーチン
        private IEnumerator _HilightArea(int areaID, EndCallBack callback)
        {
            var game = Game.GetInstance();
            var area = game.GameData.Area[areaID];
            var pos = area.Position;

            //カメラコントローラの取得
            var cameraCtrl = mFieldController.CameraController;

            //移動
            yield return StartCoroutine(cameraCtrl.MoveTo(pos));

            //終了通知
            if(callback != null)
                callback();

            yield return null;
        }

    }
}