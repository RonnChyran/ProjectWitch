using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace PreBattle
{
    public class DragUnit : DragObject
    {
        //ユニット番号
        [SerializeField]
        private int mUnitID = -1;
        public int UnitID { get { return mUnitID; } set { mUnitID = value; } }

        //コンポーネント
        private Image mcImage;
           
        //各テキストへの参照
        private Text mHP;
        private Text mSoldier;
        private Text mName;

        //既定の顔画像
        private Sprite mDefImage;

        //出撃ユニットへの参照
        private RecieveUnit[] BattleUnits = new RecieveUnit[3];

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            //コンポーネントを取得
            mcImage = GetComponent<Image>();

            //既定の画像を取得
            mDefImage = mcImage.sprite;

            //出撃ユニットを取得（BattleUnits/Box/から
            var battleUnits = GameObject.Find("BattleUnits");
            var box = battleUnits.transform.FindChild("Box").gameObject;
            for (int i = 0; i < box.transform.childCount; i++)
                BattleUnits[i] =
                    box.transform.GetChild(i).gameObject.GetComponent<RecieveUnit>();
            
            //データの初期化
            mHP = transform.FindChild("HP").gameObject.GetComponent<Text>();
            mSoldier = transform.FindChild("Soldier").gameObject.GetComponent<Text>();
            mName = transform.FindChild("Name").gameObject.GetComponent<Text>();

            DataUpdate();
        }

        protected override void Update()
        {
            //出撃ユニットに含まれているか判定
            foreach(var unit in BattleUnits)
            {
                IsDragged = false;
                if (unit.UnitID == UnitID)
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

            if (UnitID < 0)
            {
                mName.text = "";
                mSoldier.text = "";
                mHP.text = "";

                //既定の画像をセット
                mcImage.sprite = mDefImage;
            }
            else
            {
                mName.text = game.UnitData[UnitID].Name;
                mSoldier.text = "兵数 " + game.UnitData[UnitID].SoldierNum.ToString();
                mHP.text = "HP " + game.UnitData[UnitID].HP.ToString();

                //顔画像をロードしてセット
                var sprite = Resources.Load<Sprite>("Textures/Face/" + game.UnitData[mUnitID].FaceIamgePath);
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