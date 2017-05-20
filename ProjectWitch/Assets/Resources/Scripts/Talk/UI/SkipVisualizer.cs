using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Talk
{
    [RequireComponent(typeof(Image))]
    public class SkipVisualizer : MonoBehaviour
    {
        private Image mcImage = null;

        // Use this for initialization
        void Start()
        {
            mcImage = GetComponent<Image>();
            mcImage.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            if(Input.GetButton("TalkSkip"))
            {
                mcImage.enabled = true;
            }
            else
            {
                mcImage.enabled = false;
            }
        }
    }
}