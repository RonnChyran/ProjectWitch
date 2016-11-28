using UnityEngine;
using System.Collections;

namespace ProjectWitch
{
    public class QuitWindow : MonoBehaviour
    {

        //component
        private Canvas mcCanvas = null;

        void Start()
        {
            mcCanvas = GetComponent<Canvas>();
        }

        //終了時処理
        void OnApplicationQuit()
        {
            if (mcCanvas.enabled == false)
            {
                Application.CancelQuit();
            }

            mcCanvas.enabled = true;
        }

        //終了ボタン
        public void OnExitClicked()
        {
            Application.Quit();
        }

        //キャンセルボタン
        public void OnCancelClicked()
        {
            GetComponent<Canvas>().enabled = false;
        }
    }
}