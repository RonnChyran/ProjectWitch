using UnityEngine;
using UnityEngine.UI;

using System.Collections;

namespace ProjectWitch.Menu
{
    public class SoldierEditWindow : MonoBehaviour
    {

        [SerializeField]
        private GameObject mPanel = null;
        [SerializeField]
        private ArmyMenu mArmyMenu = null;

        [Header("スライダー")]
        [SerializeField]
        private Slider mSliderHP = null;
        [SerializeField]
        private Slider mSliderSoldier = null;
        [SerializeField]
        private Slider mSliderMaxSoldier = null;

        [Header("ステータス表示テキスト")]
        [SerializeField]
        private Text mHPText = null;
        [SerializeField]
        private Text mSoldierText = null;
        [SerializeField]
        private Text mMaxSoldierText = null;

        [Header("コスト表示テキスト")]
        [SerializeField]
        private Text mHPCostText = null;
        [SerializeField]
        private Text mSoldierCostText = null;
        [SerializeField]
        private Text mSoldierMaxCostText = null;

        [Header("マナ表示テキスト")]
        [SerializeField]
        private Text mRequiredManaText = null;
        [SerializeField]
        private Text mCurrentManaText = null;

        [Header("ボタン")]
        [SerializeField]
        private Button mSubmitButton = null;

        //内部変数
        private int mUpHP = 0;
        private int mUpSoldierNum = 0;
        private int mUpSoldierLimit = 0;
        private int mSumCost = 0;

        //ID
        public int UnitID { get; set; }
        public StatusWindow StatusWindow { get; set; }

        // Use this for initialization
        void Start()
        {
            UnitID = -1;
        }

        // Update is called once per frame
        void Update()
        {
            if (UnitID != -1)
                Reset();
        }

        void Reset()
        {
            var game = Game.GetInstance();
            var unit = game.UnitData[UnitID];

            //スライダーの範囲を設定
            var hpmax = unit.MaxHP - unit.HP;
            if (hpmax == 0) mSliderHP.interactable = false;
            else mSliderHP.interactable = true;
            mSliderHP.maxValue = hpmax;

            var solMax = unit.MaxSoldierNum - unit.SoldierNum;
            if (solMax == 0) mSliderSoldier.interactable = false;
            else mSliderSoldier.interactable = true;
            mSliderSoldier.maxValue = solMax;

            var solLimit = game.PlayerMana / unit.SoldierLimitCost;
            if (solLimit == 0) mSliderMaxSoldier.interactable = false;
            else mSliderMaxSoldier.interactable = true;
            mSliderMaxSoldier.maxValue = solLimit;

            //上昇する数値の更新
            mUpHP = (int)mSliderHP.value;
            mUpSoldierNum = (int)mSliderSoldier.value;
            mUpSoldierLimit = (int)mSliderMaxSoldier.value;

            //総コストの計算
            int sumHpCost = mUpHP * unit.HPCost;
            int sumSoldierCost = mUpSoldierNum * unit.SoldierCost;
            int sumSoldierLimitCost = mUpSoldierLimit * unit.SoldierLimitCost;
            mSumCost = sumHpCost + sumSoldierCost + sumSoldierLimitCost;

            //テキストの設定
            mHPCostText.text = unit.HPCost.ToString();
            mSoldierCostText.text = unit.SoldierCost.ToString();
            mSoldierMaxCostText.text = unit.SoldierLimitCost.ToString();

            mHPText.text = ((int)(mUpHP + unit.HP)).ToString();
            mSoldierText.text = ((int)(mUpSoldierNum + unit.SoldierNum)).ToString();
            mMaxSoldierText.text = ((int)(mUpSoldierLimit + unit.MaxSoldierNum)).ToString();

            mRequiredManaText.text = mSumCost.ToString();
            mCurrentManaText.text = game.PlayerMana.ToString();            

            
            //ボタンの有効無効か
            if (mSumCost > game.PlayerMana)
                mSubmitButton.interactable = false;
            else
                mSubmitButton.interactable = true;
        }

        public void Submit()
        {
            var game = Game.GetInstance();
            var unit = game.UnitData[UnitID];

            //ステータス上昇
            unit.HP += mUpHP;
            unit.SoldierNum += mUpSoldierNum;
            unit.MaxSoldierNum += mUpSoldierLimit;

            //マナ減少
            game.PlayerMana -= mSumCost;

            StatusWindow.Reset();
            Close();
        }

        public void Cancel()
        {

            Close();
        }

        public void Show(int unitID)
        {
            UnitID = unitID;
            mArmyMenu.Closable = false;
            mPanel.SetActive(true);
        }

        private void Close()
        {
            mSliderHP.value = 0;
            mSliderSoldier.value = 0;
            mSliderMaxSoldier.value = 0;

            mArmyMenu.Closable = true;
            mPanel.SetActive(false);
        }
    }
}