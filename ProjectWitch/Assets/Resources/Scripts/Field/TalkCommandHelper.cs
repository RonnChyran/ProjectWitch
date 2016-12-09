using UnityEngine;
using System.Collections;

namespace ProjectWitch.Field
{
    public class TalkCommandHelper : MonoBehaviour
    {

        //ある処理が終了したときに呼び出されるメソッド
        public delegate void EndCallBack();

        //指定エリアの位置にカメラを移動させてハイライトする
        public void HilightArea(int area, EndCallBack callback)
        {

        }

        //コルーチン

        private void _HilightArea(int area, EndCallBack callback)
        {

            //終了通知
            callback();
        }
    }
}