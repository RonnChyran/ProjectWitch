using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MenuButton : MonoBehaviour {

    [SerializeField]
    private string mSceneName = "";

    //field controller
    private FieldController mFieldController;

    void Start()
    {
        var obj = GameObject.Find("FieldController");
        mFieldController = obj.GetComponent<FieldController>();
    }

    public void OnClick()
    {
        //メニューが開いている場合は何もしない
        if (mFieldController.OpeningMenu) return;

        SceneManager.LoadScene(mSceneName, LoadSceneMode.Additive);

        mFieldController.OpeningMenu = true;
    }
}
