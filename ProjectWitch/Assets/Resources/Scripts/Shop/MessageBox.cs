using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    public class MessageBox : MonoBehaviour {

        [SerializeField]
        private Text mName = null;

        [SerializeField]
        private Text mMessage = null;

        //すべてのテキスト
        private string mAllText = "";

        //表示開始からの時間
        private float mTime = 0.0f;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            mTime += Time.deltaTime;

            //テキストスピードを取得
            var game = Game.GetInstance();
            var speed = ConfigDataFormat.TextSpeedValues[game.SystemData.Config.TextSpeed];

            //表示する文字数
            int numLetter = (int)Mathf.Min(speed * mTime, mAllText.Length);

            //テキストセット
            mMessage.text = mAllText.Substring(0, numLetter);
        }

        //テキストをセットしなおす
        public void SetText(string name, string mes)
        {
            mName.text = name;
            mAllText = mes;
            mTime = 0.0f;
        }
    }
}