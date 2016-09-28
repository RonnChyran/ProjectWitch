using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class TalkController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var game = Game.GetInstance();

        //トークシーンの開始
        game.IsTalk = true;
	}

    public void EndScript()
    {
        var game = Game.GetInstance();

        //トークシーンの終了
        game.IsTalk = false;

        //シーンのアンロード
        SceneManager.UnloadScene(game.SceneName_Talk);
    }
}
