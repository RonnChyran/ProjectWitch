using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrevBtn2 : MonoBehaviour {

    public void OnClick()
    {
        Debug.Log("Prev Button Pushed !!");
        //GetComponent<>().enabled = false;
        //gameObject.SetActive(false);
        GameObject.Find("Page2").gameObject.SetActive(false);
        GameObject.Find("TipsInst").transform.Find("Page1").gameObject.SetActive(true);
    }


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
	}
}
