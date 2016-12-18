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

        //ある処理が終了したときに呼び出されるメソッド
        public delegate void EndCallBack();

        //エリアウィンドウのインスタンス
        GameObject mInstAreaWindow = null;
        GameObject mInstAreaName = null;

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

        //エリアウィンドウから戦闘を呼び出す
        public void CallBattleFromAreaWindow()
        {
            if(mInstAreaWindow)
            {
                var game = Game.GetInstance();
                var cAreaWindow = mInstAreaWindow.GetComponent<AreaWindow>();

                //味方の領地かどうかチェック
                if (game.GameData.Area[cAreaWindow.AreaID].Owner == 0)
                {
                    Debug.LogWarning(cAreaWindow.AreaID + "番の領地は味方の領地です。攻め込むことはできません。");
                    return;
                }

                //バトルの呼び出し（侵攻戦）
                cAreaWindow.CallBattle();
            }
            else
            {
                Debug.LogWarning("CallBattleFromAreaWindowを呼び出せません。先にOpenAreaWindowを実行してください。");
            }
        }

        //メニューを開く
        public void OpenMenu()
        {
            StartCoroutine(Game.GetInstance().CallMenu());
            mFieldController.FieldUIController.HideUI();
            mFieldController.MenuClickable = false;
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