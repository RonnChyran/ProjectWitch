using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartButton : MonoBehaviour {

    public void OnClick()
    {
        //初回用リソースをロード
        var game = Game.GetInstance();
        game.Setup();

        //フィールドの呼び出し
        SceneManager.LoadScene(game.SceneName_Opening);
    }
}
