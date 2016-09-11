using UnityEngine;
using System.Collections;

public class BattleController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var game = Game.GetInstance();

        game.IsBattle = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
