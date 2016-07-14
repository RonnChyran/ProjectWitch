using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;




public class AreaButton : MonoBehaviour {

    [SerializeField]
    private bool isPlayer = true;

    [SerializeField]
    private string mPlayerAreaMenuScene = "";

    [SerializeField]
    private string mEnemyAreaMenuScene = "";

    //フィールドコントローラ
    private FieldController mFieldController;

    void Start()
    {
        var gameobject = GameObject.Find("FieldController");
        mFieldController = gameobject.GetComponent<FieldController>();
    }

    public void ButtonPush()
    {
        //メニュー開いていたら何もしない
        if (mFieldController.OpeningMenu)
            return;

        if (isPlayer)
            SceneManager.LoadScene(mPlayerAreaMenuScene, LoadSceneMode.Additive);
        else
            SceneManager.LoadScene(mEnemyAreaMenuScene, LoadSceneMode.Additive);

        mFieldController.OpeningMenu = true;
    }
}
