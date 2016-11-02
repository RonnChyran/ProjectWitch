using UnityEngine;
using System.Collections;

namespace PreBattle
{
    public class CardList : MonoBehaviour
    {
        //カード単体のベース
        [SerializeField]
        private GameObject mCard=null;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();
            var groupID = game.TerritoryData[0].GroupList[0];
            var group = game.GroupData[groupID];

            foreach (var card in group.CardList)
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
