using UnityEngine;
using System.Collections;

//ただただ回転するだけ
public class Rotation : MonoBehaviour {

    [SerializeField]
    private float mRotateSpeed;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.Rotate(0.0f, 0.0f, mRotateSpeed);
	}
}
