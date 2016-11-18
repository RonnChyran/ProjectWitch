using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class BattleBeginEffect : MonoBehaviour {

    [SerializeField]
    private Text mPlayerTerritoryName = null;
    [SerializeField]
    private Text mEnemyTerritoryName = null;
    [SerializeField]
    private Text mPlayerSoldierSum = null;
    [SerializeField]
    private Text mEnemySoldierSum = null;

	// Use this for initialization
	void Start () {
        var game = Game.GetInstance();
        
        var pter = game.TerritoryData[game.BattleIn.PlayerTerritory];
        var eter = game.TerritoryData[game.BattleIn.EnemyTerritory];

        mPlayerTerritoryName.text = pter.OwnerName + "軍";
        mEnemyTerritoryName.text = eter.OwnerName + "軍";

        var pUnitIDs = game.BattleIn.PlayerUnits;
        var eUnitIDs = game.BattleIn.EnemyUnits;

        var sum = 0;
        foreach(var id in pUnitIDs)
        {
            if (id == -1) continue;
            var unit = game.UnitData[id];

            sum += unit.MaxSoldierNum;
        }
		mPlayerSoldierSum.text = sum.ToString();
		sum = 0;
		foreach (var id in eUnitIDs)
        {
			if (id == -1) continue;
			var unit = game.UnitData[id];

			sum += unit.MaxSoldierNum;
		}
		mEnemySoldierSum.text = sum.ToString();

	}

	// Update is called once per frame
	void Update () {
	
	}
}
