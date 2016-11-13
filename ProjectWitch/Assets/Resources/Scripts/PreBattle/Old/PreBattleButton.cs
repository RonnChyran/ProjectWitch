using UnityEngine;
using UnityEngine.UI;
using System.Linq;

namespace PreBattleOld
{
    public class PreBattleButton : MonoBehaviour
    {
        [SerializeField]
        private RecieveUnit[] mUnitBoxes = null;

        //component
        private Button mcButton = null;

        void Start()
        {
            mcButton = GetComponent<Button>();
        }

        void Update()
        {
            if (mUnitBoxes[0].UnitID == -1 &&
                mUnitBoxes[1].UnitID == -1 &&
                mUnitBoxes[2].UnitID == -1)
            {
                mcButton.interactable = false;
            }
            else
                mcButton.interactable = true;
        }

        //戦闘の呼び出し
        public void CallBattle()
        {
            var game = Game.GetInstance();

            //データのセット
            //前詰めになるよう設定
            var units = Enumerable.Repeat<int>(-1, 3).ToList();
            for (int i = 0, j = 0; i < 3; i++)
            {
                if (mUnitBoxes[i].UnitID != -1)
                    units[j++] = mUnitBoxes[i].UnitID;
            }

            game.BattleIn.PlayerUnits = units;

            StartCoroutine(game.CallBattle());
        }
    }
}