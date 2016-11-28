using UnityEngine;
using System.Collections;

namespace ProjectWitch
{
    //ただただ回転するだけ
    public class Rotation : MonoBehaviour
    {

        [SerializeField]
        private float mRotateSpeed = 0.0f;

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            transform.Rotate(0.0f, 0.0f, mRotateSpeed);
        }
    }
}