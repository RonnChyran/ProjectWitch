using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Batlle
{
    public class BattleCardEffect : MonoBehaviour
    {

        [SerializeField]
        private int mCardID = 0;
        public int CardID { get { return mCardID; } set { mCardID = value; Reset(); } }

        [SerializeField]
        private Image mImage = null;

        //パス
        [SerializeField]
        private string mCardImagePath = "Textures/Card/";

        //カードの表画像
        private Sprite mFrontSprite = null;

        //カードの裏画像
        private Sprite mBackSprite = null;

        // Use this for initialization
        void Start()
        {
            Reset();
        }

        public void Reset()
        {
            var game = Game.GetInstance();
            var card = game.GameData.Card[CardID];

            mFrontSprite = Resources.Load<Sprite>(mCardImagePath + card.ImageFront);
            mBackSprite = Resources.Load<Sprite>(mCardImagePath + card.ImageBack);
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            //表か裏かで画像を変える
            if((int)((mImage.transform.rotation.eulerAngles.y + 90) / 180) % 2 == 1)
            {
                mImage.sprite = mFrontSprite;
            }
            else
            {
                mImage.sprite = mBackSprite;
            }
        }
    }
}