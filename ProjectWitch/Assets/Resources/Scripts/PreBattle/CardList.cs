using UnityEngine;
using System.Collections;

namespace ProjectWitch.PreBattle
{
    public class CardList : MonoBehaviour
    {

        //リストの親への参照
        [SerializeField]
        private GameObject mContentGroup = null;

        //カードのプレハブ
        [SerializeField]
        private GameObject mCardPrefab = null;

        //コントローラ
        [SerializeField]
        private PreBattleController mController = null;

        //カード情報ウィンドウ
        [SerializeField]
        private CardInfo mCardInfo = null;

        // Use this for initialization
        void Start()
        {
            CardSet();
        }

        // Update is called once per frame
        void Update()
        {

        }

        void CardSet()
        {
            var game = Game.GetInstance();

            var territory = game.GameData.Territory[0];
            var group = game.GameData.Group[territory.GroupList[0]];
            foreach (var cardid in group.CardList)
            {
                //コンテンツを追加
                var inst = Instantiate(mCardPrefab);
                var cCard = inst.GetComponent<Card>();
                cCard.CardID = cardid;
                cCard.Controller = mController;
                cCard.CardInfo = mCardInfo;
                inst.transform.SetParent(mContentGroup.transform, false);
            }
        }
    }
}