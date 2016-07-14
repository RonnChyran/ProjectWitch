using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ArmyJump : MonoBehaviour {
    public void ButtonPush()
    {
        SceneManager.LoadScene("Army menu");
    }
}
