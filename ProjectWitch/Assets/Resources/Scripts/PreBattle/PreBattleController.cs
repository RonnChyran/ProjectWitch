using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System.Linq;

namespace PreBattle
{
    public class PreBattleController : MonoBehaviour
    {

        //ユニットID
        public List<int> UnitList { get; set; }

        //カードID
        public List<int> CardList { get; set; }

        PreBattleController()
        {
            UnitList = Enumerable.Repeat<int>(-1, 3).ToList();
            CardList = Enumerable.Repeat<int>(-1, 3).ToList();
        }

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            game.IsBattle = true;
            game.HideNowLoading();
        }

        // Update is called once per frame
        void Update()
        {

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
                    cards[j++] = CardList[i];
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
    }
}