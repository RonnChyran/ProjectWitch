using UnityEngine;
using System.Collections;

public class SoundController : MonoBehaviour {

    [SerializeField]
    private string mSoundFolderPath = "";

    private AudioSource mcAudio;

	// Use this for initialization
	void Start () {
        mcAudio = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void Play(string filename)
    {
        
    }

    public void Stop()
    {

    }

    public void FadeOut()
    {

    }

    public void FadeIn()
    {

    }

}
