using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ListClick : MonoBehaviour {

    public GameObject TipsDisplay;
    public GameObject Content;

    // Use this for initialization
    void Start () {
        Onclick();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    void Onclick()
    {
        Destroy(GameObject.FindGameObjectWithTag("TipsDisplayed"));

        //GameObject ob1 = (GameObject) Instantiate (TipsExample);
        

        var Cont = Instantiate(TipsDisplay) as GameObject;//Instantiate(ContentsPref);
        Cont.name = "TipsInst";
        TipsDisplay.tag = ("TipsDisplayed");
              
        Cont.transform.parent = Content.transform;

        Cont.transform.localPosition = new Vector3 (0, 0, 0);

        GameObject.Find("TipsInst").transform.Find("Page2").gameObject.SetActive(false);
        GameObject.Find("TipsInst").transform.Find("Page3").gameObject.SetActive(false);
        //GameObject.Find("TipsInst").transform.Find("Page4").gameObject.SetActive(false);




    }


}
