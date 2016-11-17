using UnityEngine;
using System.Collections;

public class EndMovieController : MonoBehaviour
{
    public void Update()
    {
        if(Input.GetButtonDown("Submit"))
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
