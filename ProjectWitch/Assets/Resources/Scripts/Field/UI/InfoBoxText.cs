using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

public class InfoBoxText : MonoBehaviour {

    private Text mcText;
    private bool isRunning = false;

	// Use this for initialization
	void Start () {
        mcText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!isRunning)
            StartCoroutine(_Update());
	}

    IEnumerator _Update()
    { 
        var game = Game.GetInstance();

        isRunning = true;

        //表示内容を構成
        var text = "";

        //所持マナ
        text += game.PlayerMana.ToString() + "\n";

        //ユニット
        var groupID = game.TerritoryData[0].GroupList[0];
        var groupData = game.GroupData[groupID];
        text += groupData.UnitList.Count.ToString() + "\n";

        //所持領地
        text += game.TerritoryData[0].AreaList.Count.ToString() + "/"
            + (game.AreaData.Count - 1).ToString() + "\n";

        //総兵力
        var soldireNumList =
            from p in groupData.UnitList
            select game.UnitData[p].SoldierNum;

        int sumSoldire = 0;
        foreach (var soldireNum in soldireNumList)
        {
            sumSoldire += soldireNum;
        }

        text += sumSoldire.ToString() + "\n";


        mcText.text = text;

        isRunning = false;
        yield return new WaitForSeconds(1.0f);
    }
}
