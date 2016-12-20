using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace ProjectWitch.PreBattle
{
    public class PreBattleController : MonoBehaviour
    {
        //チュートリアルのシナリオ名
        [SerializeField]
        private string mTutorialScenarioName = "s9801";

        //コマンドヘルパーの参照
        [SerializeField]
        private TalkCommandHelper mTalkCommandHelper = null;
        public TalkCommandHelper TalkCommandHelper { get { return mTalkCommandHelper; } private set { } }

        //ユニットID
        public List<int> UnitList { get; set; }

        //カードID
        public List<int> CardList { get; set; }

        //ユニットとカードののセット履歴
        public List<int> UnitSetHistory { get; set; }
        public List<int> CardSetHistory { get; set; }

        //キャンセルの対象(true:unit false:card)
        public bool CancelTargetIsUnit { get; set; }

        PreBattleController()
        {
            UnitList = Enumerable.Repeat<int>(-1, 3).ToList();
            CardList = Enumerable.Repeat<int>(-1, 3).ToList();
            UnitSetHistory = new List<int>();
            CardSetHistory = new List<int>();
        }

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            game.IsBattle = true;
            game.HideNowLoading();

            //チュートリアルだった場合
            if (game.BattleIn.IsTutorial)
                StartTutorial();
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();

            //履歴に従ってセットしたユニットもしくはカードを削除
            if(Input.GetButtonDown("Cancel"))
            {
                //ユニットを対象に削除
                if (CancelTargetIsUnit)
                {
                    RemoveUnitWithHistory();
                }
                //カードを対象に削除
                else
                {
                    RemoveCardWithHistory();
                }
            }
        }

        public void GoToBattle()
        {
            var game = Game.GetInstance();

            //データのセット
            //前詰めになるよう設定
            var units = Enumerable.Repeat<int>(-1, 3).ToList();
            for (int i = 0, j = 0; i < 3; i++)
            {
                if (UnitList[i] != -1)
                    units[j++] = UnitList[i];
            }
            game.BattleIn.PlayerUnits = units;

            var cards = Enumerable.Repeat<int>(-1, 3).ToList();
            for(int i=0,j=0;i<3;i++)
            {
                if (CardList[i] != -1)
                    cards[j++] = ConvertToGlobalCardIndex(CardList[i]);
            }
            game.BattleIn.PlayerCards = cards;

            StartCoroutine(game.CallBattle());

        }

        public void CancelBattle()
        {
            var game = Game.GetInstance();

            //敗北扱いにする
            game.BattleOut.IsWin = false;

            game.IsBattle = false;
            SceneManager.UnloadScene(game.SceneName_PreBattle);
        }

        //履歴をもとにユニットを削除
        public void RemoveUnitWithHistory()
        {
            var game = Game.GetInstance();

            //削除するものがなかったらカードを削除する
            var count = UnitSetHistory.Count;
            if (count == 0)
            {
                count = CardSetHistory.Count;
                if(count > 0)
                {
                    CardList[CardSetHistory[count - 1]] = -1;
                    CardSetHistory.HistoryRemove(CardSetHistory[count - 1]);
                    game.SoundManager.PlaySE(SE.Cancel);
                }
            }
           else
            {
                UnitList[UnitSetHistory[count - 1]] = -1;
                UnitSetHistory.HistoryRemove(UnitSetHistory[count - 1]);
                game.SoundManager.PlaySE(SE.Cancel);
            }
        }

        //履歴をもとにカードを削除
        public void RemoveCardWithHistory()
        {
            var game = Game.GetInstance();

            //削除するものがなかったらカードを削除する
            var count = CardSetHistory.Count;
            if (count == 0)
            {
                count = UnitSetHistory.Count;
                if (count > 0)
                {
                    UnitList[UnitSetHistory[count - 1]] = -1;
                    UnitSetHistory.HistoryRemove(UnitSetHistory[count - 1]);
                    game.SoundManager.PlaySE(SE.Cancel);
                }
            }
            else
            {
                CardList[CardSetHistory[count - 1]] = -1;
                CardSetHistory.HistoryRemove(CardSetHistory[count - 1]);
                game.SoundManager.PlaySE(SE.Cancel);
            }
        }

        //チュートリアル開始
        private void StartTutorial()
        {
            var game = Game.GetInstance();

            EventDataFormat e = new EventDataFormat();
            e.FileName = mTutorialScenarioName;
            game.CallScript(e);
        }

        //グループのカードリストインデックスから、
        //データのカードリストインデックスへ返還
        public int ConvertToGlobalCardIndex(int index)
        {
            var game = Game.GetInstance();
            var territory = game.GameData.Territory[0];
            var group = game.GameData.Group[territory.GroupList[0]];
            var cardlist = group.CardList;
            return cardlist[index];
        }

    }

    public static class PreBattleExtentions
    {
        //履歴に追加する
        public static void HistoryAdd(this List<int> list, int listIndex)
        {
            //被ってるインデックスを削除
            list.Remove(listIndex);

            //ユニットを履歴の先頭に追加
            list.Add(listIndex);
        }

        //履歴から消す
        public static void HistoryRemove(this List<int> list, int listIndex)
        {
            list.Remove(listIndex);
        }
    }

}