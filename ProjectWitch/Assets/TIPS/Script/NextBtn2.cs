using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NextBtn2 : MonoBehaviour {

    public void OnClick()
    {
        Debug.Log("Next Button Pushed !!");
        //GetComponent<>().enabled = false;
        //gameObject.SetActive(false);
        GameObject.Find("Page2").gameObject.SetActive(false);
        GameObject.Find("TipsInst").transform.Find("Page3").gameObject.SetActive(true);
    }


    // Use this for initialization
    void Start () {
        //OnClick();
    }
	
	// Update is called once per frame
	void Update () {
        
	}
}
