using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.Field
{
    public class TimeCounter : MonoBehaviour
    {
        //各画像
        [SerializeField]
        private Sprite mMorningSp = null;
        [SerializeField]
        private Sprite mNoonSp = null;
        [SerializeField]
        private Sprite mEveningSp = null;
        [SerializeField]
        private Sprite mNightSp = null;

        //コンポーネント
        private Image mcImage;

        // Use this for initialization
        void Start()
        {
            mcImage = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();

            switch(game.GameData.CurrentTime)
            {
                case 0:
                    mcImage.sprite = mMorningSp;
                    break;
                case 1:
                    mcImage.sprite = mNoonSp;
                    break;
                case 2:
                    mcImage.sprite = mEveningSp;
                    break;
                default:
                    mcImage.sprite = mNightSp;
                    break;
            }
        }
    }
}