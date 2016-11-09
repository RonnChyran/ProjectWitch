using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class BattleController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var game = Game.GetInstance();

        game.IsBattle = true;
        game.HideNowLoading();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void BattleEnd()
    {
        var game = Game.GetInstance();

        game.ShowNowLoading();
        game.IsBattle = false;
        SceneManager.UnloadScene(game.SceneName_Battle);
    }
}
