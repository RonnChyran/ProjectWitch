using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class LoadButton : MonoBehaviour {

    [SerializeField]
    private string mLoadSceneName = "";

	public void OnClick()
    {
        //ロード画面を呼び出す
        SceneManager.LoadScene(mLoadSceneName);
    }
}
