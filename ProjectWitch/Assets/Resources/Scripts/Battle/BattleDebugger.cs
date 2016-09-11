using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleDebugger : MonoBehaviour {

    public void BattleWin()
    {
        var game = Game.GetInstance();
        game.BattleOut.IsWin = true;

        BattleEnd();
    }

    public void BattleLose()
    {
        var game = Game.GetInstance();
        game.BattleOut.IsWin = false;
        
        BattleEnd();

    }

    public void BattleEnd()
    {
        var game = Game.GetInstance();

        game.IsBattle = false;
        game.CallAfterBattle();
       
    }
	
}
