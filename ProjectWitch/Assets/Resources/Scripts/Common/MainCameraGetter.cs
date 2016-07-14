using UnityEngine;
using System.Collections;

public class MainCameraGetter : MonoBehaviour {

   private Canvas canvas;

    void Start()
    {
        canvas = GetComponent<Canvas>();

        canvas.worldCamera = Camera.main;
    }


}
