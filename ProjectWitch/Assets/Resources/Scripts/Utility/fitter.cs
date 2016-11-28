using UnityEngine;
using System.Collections;

namespace ProjectWitch
{
    //Unityのバグによりアンカーが勝手に変わるのでそれまでの補正用
    public class fitter : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {
            GetComponent<RectTransform>().anchorMax = new Vector2(1.0f, 1.0f);
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}