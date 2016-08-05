using UnityEngine;
using System.Collections;

namespace PreBattle
{
    public class CardList : MonoBehaviour
    {
        //カード単体のベース
        [SerializeField]
        private GameObject mCard;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            var playerCards = game.TerritoryData[0].CardList;
            foreach(var card in playerCards)
            {
                //カードの生成
                var inst = Instantiate(mCard);
                inst.transform.SetParent(transform);
                inst.transform.localScale = Vector3.one;

                //データのセット
                var dragCard = inst.GetComponent<DragCard>();
                dragCard.CardID = card;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
