using UnityEngine;
using System.Collections;

public class SoundTester : MonoBehaviour {

    [SerializeField]
    string[] Name=null;

    private int currentSound = 0;
    
	// Update is called once per frame
	void Update () {

        if (Name.Length < 1) return;

        //ボタンを押すごとに指定したBGMを順に再生する
        if(Input.GetButtonDown("Submit"))
        {
            var game = Game.GetInstance();
            game.SoundManager.Play(Name[currentSound], SoundType.BGM);
            
            if (++currentSound >= Name.Length) currentSound = 0;
        }

        //キャンセルボタンを押すとBGMを停止
        if(Input.GetButtonDown("Cancel"))
        {
            var game = Game.GetInstance();
            game.SoundManager.Stop(SoundType.BGM);
        }
	}
}
