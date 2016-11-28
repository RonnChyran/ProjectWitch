using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch
{
    public class StartController : MonoBehaviour
    {

        [SerializeField]
        private Animator mAnimator;

        //飛ばせるかどうか
        private bool isSkipable = false;

        void Update()
        {
            if (isSkipable)
            {
                if (Input.GetButtonDown("Submit"))
                {
                    GoToEnd();
                }
            }
        }

        public void SkipEnable()
        {
            isSkipable = true;
        }

        public void SkipDisable()
        {
            isSkipable = false;
        }

        public void GoToEnd()
        {
            mAnimator.Play("teamrogoC");
        }

        public void End()
        {
            SceneManager.LoadScene("Title");
        }
    }
}