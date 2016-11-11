using UnityEngine;
using System.Collections;

public class GoTitleWindow : MonoBehaviour { 

    //component
    private Canvas mcCanvas = null;

	// Use this for initialization
	void Start () {
        mcCanvas = GetComponent<Canvas>();
	}
	
	// Update is called once per frame
	void Update () {
	    if(Input.GetKey("e") &&
            Input.GetKey("s") &&
            Input.GetKey("c"))
        {
            Show();
        }
	}

    public void Show()
    {
        mcCanvas.enabled = true;
    }

    public void OnYes()
    {
        var game = Game.GetInstance();
        mcCanvas.enabled = false;
        StartCoroutine(game.CallTitle());
    }

    public void OnNo()
    {
        mcCanvas.enabled = false;
    }
}
