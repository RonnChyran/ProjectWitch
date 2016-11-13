using UnityEngine;
using System.Collections;

namespace PreBattle
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

            var territory = game.TerritoryData[0];
            var group = game.GroupData[territory.GroupList[0]];
            foreach (var cardid in group.CardList)
            {
                //コンテンツを追加
                var inst = Instantiate(mCardPrefab);
                var cCard = inst.GetComponent<Card>();
                cCard.CardID = cardid;
                cCard.Controller = mController;
                inst.transform.SetParent(mContentGroup.transform, false);
            }
        }
    }
}