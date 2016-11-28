using UnityEngine;
using System.Collections;

namespace ProjectWitch
{
    public class EndMovieController : MonoBehaviour
    {
        void Start()
        {
            var game = Game.GetInstance();
            game.HideNowLoading();
        }
        public void Update()
        {
            if (Input.GetButtonDown("Submit"))
            {
                End();
            }
        }

        public void End()
        {
            var game = Game.GetInstance();

            StartCoroutine(game.CallTitle());
        }
    }
}
