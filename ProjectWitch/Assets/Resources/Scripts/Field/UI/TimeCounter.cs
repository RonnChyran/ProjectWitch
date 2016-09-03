using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Field
{
    public class TimeCounter : MonoBehaviour
    {
        //コンポーネント
        private Text mcText;

        // Use this for initialization
        void Start()
        {
            mcText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();

            switch(game.CurrentTime)
            {
                case 0:
                    mcText.text = "朝";
                    break;
                case 1:
                    mcText.text = "昼";
                    break;
                case 2:
                    mcText.text = "夕";
                    break;
                default:
                    mcText.text = "晩";
                    break;
            }
        }
    }
}