using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LeaderMenu : MonoBehaviour {

    public void ButtonPush()
    {
        SceneManager.LoadScene("Leader menu", LoadSceneMode.Additive);
    }
}
