using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace PreBattle
{
    public class DragCard : DragObject
    {
        //カード番号
        [SerializeField]
        private int mCardID = -1;
        public int CardID { get { return mCardID; } set { mCardID = value; } }

        //コンポーネント
        private Image mcImage; 

        //既定の画像
        private Sprite mDefImage;

        //使用カードへの参照
        private RecieveCard[] BattleCards = new RecieveCard[3];

        //use this for initialization
        protected override void Start()
        {
            base.Start();

            //コンポーネントを取得
            mcImage = GetComponent<Image>();

            //既定の画像を取得
            mDefImage = mcImage.sprite;

            //使用カードを取得
            var battleCards = GameObject.Find("BattleCards");
            var box = battleCards.transform.FindChild("Box").gameObject;
            for(int i=0; i<box.transform.childCount;i++)
                BattleCards[i] = 
                    box.transform.GetChild(i).gameObject.GetComponent<RecieveCard>();

            //データの初期化
            DataUpdate();
        }

        protected override void Update()
        {
            //使用カードに含まれているか判定
            foreach(var card in BattleCards)
            {
                IsDragged = false;
                if(card.CardID == CardID)
                {
                    IsDragged = true;
                    break;
                }
            }

            base.Update();
        }

        //データの更新
        public void DataUpdate()
        {
            var game = Game.GetInstance();

            if(CardID < 0)
            {
                mcImage.sprite = mDefImage;
            }
            else
            {
                var sprite = Resources.Load<Sprite>("Textures/Card/" + game.CardData[CardID].ImageBack);
                if(sprite)
                {
                    mcImage.sprite = sprite;
                }
                else
                {
                    mcImage.sprite = mDefImage;
                }
            }
        }
    }
}