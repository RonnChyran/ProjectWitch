using UnityEngine;
using System.Collections;
using System;

namespace ProjectWitch.PreBattle
{
    public class TalkCommandHelper : MonoBehaviour
    {
        //コントローラ
        [SerializeField]
        private PreBattleController mController = null;

        //ユニットリスト
        [SerializeField]
        private UnitList mUnitList = null;

        //カードリスト
        [SerializeField]
        private CardList mCardList = null;

        //ユニットリストからユニットを選択して、出撃リストに入れる
        //index:ユニットリストの先頭からの番号
        public void SetUnit(int index)
        {
            try
            {
                mUnitList.UnitComponentList[index].OnClicked();
            }
            catch (ArgumentException)
            {
                Debug.LogWarning("SetUnit : indexが不正です。");
            }
        }

        //出撃リストからユニットを除外
        //index:0~2で順に前衛中衛後衛
        public void RemoveUnit(int index)
        {
            try
            {
                mController.UnitList[index] = -1;
            }
            catch(ArgumentException)
            {
                Debug.LogWarning("RemoveUnit : indexが不正です。");
            }
        }

        //カードリストからカードを選択して、カードリストに入れる
        //index:カードリストの先頭からの番号
        public void SetCard(int index)
        {
            try
            {
                mCardList.CardComponentList[index].OnClicked();
            }
            catch (ArgumentException)
            {
                Debug.LogWarning("SetCard : indexが不正です。");
            }
        }

        //使用カードリストからカードを除外
        //index:0~2で順にI II III
        public void RemoveCard(int index)
        {
            try
            {
                mController.CardList[index] = -1;
            }
            catch (ArgumentException)
            {
                Debug.LogWarning("RemoveCard : indexが不正です。");
            }
        }

        //バトルを呼び出し
        public void CallBattle()
        {
            mController.GoToBattle();
        }
    }

}