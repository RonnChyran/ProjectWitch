using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class FieldAreaMenuController : MonoBehaviour

{
    [SerializeField]
    private string mUnloadSceneName = "";

    //field controller
    private FieldController mFieldController;

    void Start()
    {
        var obj = GameObject.Find("FieldController");
        mFieldController = obj.GetComponent<FieldController>();
    }
    
    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(1))
        {
            SceneManager.UnloadScene(mUnloadSceneName);
            mFieldController.OpeningMenu = false;
        }
     }
}
