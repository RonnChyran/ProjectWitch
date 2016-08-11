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
                    mcText.text = "morning";
                    break;
                case 1:
                    mcText.text = "noon";
                    break;
                case 2:
                    mcText.text = "evening";
                    break;
                default:
                    mcText.text = "night";
                    break;
            }
        }
    }
}