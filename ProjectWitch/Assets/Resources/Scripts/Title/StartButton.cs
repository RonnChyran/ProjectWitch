using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class StartButton : MonoBehaviour {

    [SerializeField]
    private string mFieldSceneName = "";

    public void OnClick()
    {
        //初回用リソースをロード
        var game = Game.GetInstance();
        game.FirstLoad();

        //フィールドの呼び出し
        SceneManager.LoadScene(game.SceneName_Opening);
    }
}
