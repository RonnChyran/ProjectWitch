using UnityEngine;
using System.Collections;

namespace PreBattle
{
    public class PreBattleButton : MonoBehaviour
    {

        //戦闘の呼び出し
        public void CallBattle()
        {
            var game = Game.GetInstance();
            game.CallBattle();
        }
    }
}