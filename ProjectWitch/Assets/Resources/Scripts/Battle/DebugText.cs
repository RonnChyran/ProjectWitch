using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace Battle
{
    public class DebugText : MonoBehaviour
    {
        //最大表示行数
        [SerializeField]
        private int mMaxRows = 10;

        //テキストコンポーネント
        private Text mcText = null;

        //表示するテキストの配列
        private Queue<string> text = new Queue<string>();

        // Use this for initialization
        void Start()
        {
            mcText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //テキストを一行追加する
        public void Push(string str)
        {
            text.Enqueue(str);
            if (text.Count > mMaxRows) text.Dequeue();

            var outText = "";
            foreach(var part in text)
            {
                outText += part + "\r\n";
            }
            mcText.text = outText;
        }

        //キャプション付きのPush
        public void Push(string caption, object data)
        {
            var str = caption;
            str += " : ";
            str += data.ToString();

            Push(str);
        }
    }
}