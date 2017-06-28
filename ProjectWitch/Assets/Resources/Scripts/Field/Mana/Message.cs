using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Field.Mana
{
    public class Message : MonoBehaviour
    {
        [SerializeField]
        private Text mcText = null;
        public string Text { set { mcText.text = value; } }

        [SerializeField]
        private float mLifeTime = 1.0f;

        private float mTime = 0.0f;

        // Update is called once per frame
        void Update()
        {
            mTime += Time.deltaTime;
            if (mTime > mLifeTime) Close();
            

        }

        void Close()
        {
            Destroy(this.gameObject);
        }
    }
}