using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace ProjectWitch.PreBattle
{
    public class BattleCard : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        //こんとろーら
        [SerializeField]
        private PreBattleController mController = null;

        //ID
        [SerializeField]
        private int mID = 0;

        //パス
        [SerializeField]
        private string mCardImagePath = "Textures/Card/";

        //パネル
        [SerializeField]
        private GameObject mPanel = null;

        //カード画像
        [SerializeField]
        private Image mCardImg = null;

        //外すボタン
        [SerializeField]
        private GameObject mDetouchButton = null;

        //内部変数
        private int mCardIDInGroup = -1;

        //プロパティ
        private int CardID
        {
            get
            {
                var game = Game.GetInstance();
                var territory = game.GameData.Territory[0];
                var group = game.GameData.Group[territory.GroupList[0]];
                var cardList = group.CardList;
                if (mCardIDInGroup == -1) return -1;
                return cardList[mCardIDInGroup];
            }
        }

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            //unitIDが変化したら更新
            if (mController.CardList[mID] != mCardIDInGroup)
            {
                mCardIDInGroup = mController.CardList[mID];
                Reset();
            }

        }

        //表示を更新
        void Reset()
        {
            if (mController.CardList[mID] != -1)
            {
                mPanel.SetActive(true);

                var game = Game.GetInstance();
                var card = game.GameData.Card[CardID];

                //表示の更新
                var sprite = Resources.Load<Sprite>(mCardImagePath + card.ImageBack);
                mCardImg.sprite = sprite;

                mDetouchButton.SetActive(false);
            }
            else
            {
                mPanel.SetActive(false);
            }
        }


        public void OnPointerEnter(PointerEventData e)
        {
            if (mController.CardList[mID] != -1)
            {
                mDetouchButton.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData e)
        {
            if (mController.CardList[mID] != -1)
            {
                mDetouchButton.SetActive(false);
            }
        }

        public void OnClickedDetouchButton()
        {
            mController.CardList[mID] = -1;
            mController.CardSetHistory.HistoryRemove(mID);
        }

    }
}