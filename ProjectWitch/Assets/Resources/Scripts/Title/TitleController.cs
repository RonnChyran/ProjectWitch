﻿using UnityEngine;
using System.Collections;

public class TitleController : MonoBehaviour {

	// Use this for initialization
	void Start () {
        var game = Game.GetInstance();
        game.HideNowLoading();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
