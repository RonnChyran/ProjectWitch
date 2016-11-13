﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Menu
{
    public class StatusWindow : MonoBehaviour
    {

        [SerializeField]
        private GameObject mPanel = null;
        [SerializeField]
        private LvUpWindow mLvUpWindow = null;



        [Space(1)]
        [Header("フォルダパス")]
        [SerializeField]
        private string mCharacterImagePath = "Textures/Menu/Character/";

        [Space(1)]
        [Header("テキストフィールド")]
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mLV = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private Text mExperience = null;
        [SerializeField]
        private Text mLPATK = null;
        [SerializeField]
        private Text mLMATK = null;
        [SerializeField]
        private Text mLPDEF = null;
        [SerializeField]
        private Text mLMDEF = null;
        [SerializeField]
        private Text mLead = null;
        [SerializeField]
        private Text mAgi = null;
        [SerializeField]
        private Text mSkillAtk = null;
        [SerializeField]
        private Text mSkillDef = null;
        [SerializeField]
        private Text mEquipment = null;
        [SerializeField]
        private Text mLove = null;
        [SerializeField]
        private Text mSoldierNum = null;
        [SerializeField]
        private Text mGPATK = null;
        [SerializeField]
        private Text mGMATK = null;
        [SerializeField]
        private Text mGPDEF = null;
        [SerializeField]
        private Text mGMDEF = null;
        [SerializeField]
        private Text mSoldierRank = null;
        [SerializeField]
        private Text mComment = null;

        [Header("イメージ")]
        [SerializeField]
        private Image mMainImage = null;
        [SerializeField]
        private Image mSubImage = null;
        [SerializeField]
        private Sprite mDefMainImage = null;
        [SerializeField]
        private Sprite mDefSubImage = null;

        [Header("ボタン")]
        [SerializeField]
        private Button mLvUpButton = null;
        [SerializeField]
        private Button mSoldierEditButton = null;

        [Header("兵士ランク境界")]
        [Space(1)]
        [SerializeField]
        private int mPowerRankA = 900;
        [SerializeField]
        private int mPowerRankB = 600;
        [SerializeField]
        private int mPowerRankC = 200;

        [Header("経験値境界値")]
        [SerializeField]
        private int mThresholdExperience = 10;

        //プロパティ
        public int UnitID { get; set; }

        //内部変数
        private bool mLvUpIsEnable = false;

        // Use this for initialization
        void Start()
        {
            UnitID = -1;
            Reset();
        }

        public void Reset()
        {
            if (UnitID == -1)
            {
                mPanel.SetActive(false);
            }
            else
            {
                mPanel.SetActive(true);
                SetState();
            }
        }

        public void OnClickLvUp()
        {
            mLvUpWindow.StatusWindow = this;
            mLvUpWindow.Show(UnitID);
        }

        public void OnClickSoldierEdit()
        {

        }

        private void SetState()
        {
            var game = Game.GetInstance();
            var unit = game.UnitData[UnitID];

            //mainwindow
            mName.text = unit.Name;
            var maxLv = (unit.MaxLevel == -1) ? "∞" : unit.MaxLevel.ToString();
            mLV.text = unit.Level.ToString() + " / " + maxLv;
            mHP.text = unit.HP.ToString() + " / " + unit.MaxHP.ToString();
            mExperience.text = unit.Experience.ToString();
            mLPATK.text = unit.LeaderPAtk.ToString();
            mLMATK.text = unit.LeaderMAtk.ToString();
            mLPDEF.text = unit.LeaderPDef.ToString();
            mLMDEF.text = unit.LeaderMDef.ToString();
            mLead.text = unit.Leadership.ToString();
            mAgi.text = unit.Agility.ToString();
            mSkillAtk.text = game.SkillData[unit.LAtkSkill].Name;
            mSkillDef.text = game.SkillData[unit.LDefSkill].Name;
            if (unit.Equipment != -1)
                mEquipment.text = game.EquipmentData[unit.Equipment].Name;
            else
                mEquipment.text = "";
            mLove.text = unit.Love.ToString();

            //subwindow
            mSoldierNum.text = unit.SoldierNum.ToString() + " / " + unit.MaxSoldierNum.ToString();
            mGPATK.text = unit.GroupPAtk.ToString();
            mGMATK.text = unit.GroupMAtk.ToString();
            mGPDEF.text = unit.GroupPDef.ToString();
            mGMDEF.text = unit.GroupMDef.ToString();
            SetRank(unit);

            //comment
            mComment.text = unit.Comment;

            //image
            var sprite = Resources.Load<Sprite>(mCharacterImagePath + unit.FaceIamgePath);
            mMainImage.sprite = (sprite) ? sprite : mDefMainImage;
            sprite = Resources.Load<Sprite>(mCharacterImagePath + unit.BattleGroupPrefabPath);
            mSubImage.sprite = (sprite) ? sprite : mDefSubImage;

            //button
            if (unit.Experience >= mThresholdExperience &&
                (unit.Level < unit.MaxLevel || unit.MaxLevel == -1))
            {
                mLvUpButton.interactable = true;
            }
            else
            {
                mLvUpButton.interactable = false;
            }
        }

        private void SetRank(GameData.UnitDataFormat unit)
        {
            var power = unit.GPAtk100 + unit.GMAtk100;

            if (power > mPowerRankA)
                mSoldierRank.text = "A";
            else if (power > mPowerRankB)
                mSoldierRank.text = "B";
            else if (power > mPowerRankC)
                mSoldierRank.text = "C";
            else
                mSoldierRank.text = "D";

        }
    }
}