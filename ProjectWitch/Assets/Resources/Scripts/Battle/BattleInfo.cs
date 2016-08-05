using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleInfo : MonoBehaviour {

    private Text mcText;

	// Use this for initialization
	void Start () {
        mcText = GetComponent<Text>();


        string text;
        var game = Game.GetInstance();

        text = "バトル情報\n";
        text += "地点番号:" + game.BattleIn.AreaID.ToString() + "\n";
        text += "時間帯:" + game.BattleIn.TimeOfDay.ToString() + "\n";
        text += "侵攻戦:" + game.BattleIn.IsInvasion.ToString() + "\n";
        text += "自動戦闘:" + game.BattleIn.IsAuto.ToString() + "\n";

        mcText.text = text;
    }
	
	// Update is called once per frame
	void Update () {
	}
}
