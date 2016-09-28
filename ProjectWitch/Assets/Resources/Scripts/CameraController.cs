using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    Vector3 mOldMousePos;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
	    
	}

    void OnMouseDown()
    {
        var ray = RectTransformUtility.ScreenPointToRay(Camera.main, Input.mousePosition);

        RaycastHit rHit;
        if(Physics.Raycast(ray, out rHit))
        {
            mOldMousePos = rHit.point;
        }
        else
        {
            mOldMousePos = Vector3.zero;
        }
        Debug.Log("PushButton");
    }

    void OnMouseDrag()
    {
        Vector3 mousePos;
        var ray = RectTransformUtility.ScreenPointToRay(Camera.main, Input.mousePosition);

        RaycastHit rHit;
        if(Physics.Raycast(ray, out rHit))
        {
            mousePos = rHit.point;
        }
        else
        {
            mousePos = Vector3.zero;
        }

        var tmp = mousePos - mOldMousePos;
        tmp.z = 0.0f;
        Camera.main.transform.position -= tmp;
    }
}
