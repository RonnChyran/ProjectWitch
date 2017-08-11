using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBtn1 : MonoBehaviour {

    public void OnClick()
    {
        Debug.Log("Next Button Pushed !!");
        //GetComponent<>().enabled = false;
        GameObject.Find("Page1").gameObject.SetActive(false);
        GameObject.Find("TipsInst").transform.Find("Page2").gameObject.SetActive(true);
    }


	// Use this for initialization
	void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
        
    }
}
