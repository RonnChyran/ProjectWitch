using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Talk
{
    [RequireComponent(typeof(Image))]
    public class AutoVisualizer : MonoBehaviour
    {
        //スクリプトエンジンへの参照
        private WorkSpace.ScenarioWorkSpace mEngine = null;

        private Image mcImage = null;

        // Use this for initialization
        void Start()
        {
            mcImage = GetComponent<Image>();
            mcImage.enabled = false;

            mEngine = GameObject.FindWithTag("TalkEngine").GetComponent<WorkSpace.ScenarioWorkSpace>();
        }

        // Update is called once per frame
        void Update()
        {
            if (mEngine.AutoMode)
            {
                mcImage.enabled = true;
            }
            else
                mcImage.enabled = false;
        }
    }
}