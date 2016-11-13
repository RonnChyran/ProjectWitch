using UnityEngine;
using System.Collections;
using UnityEngine.UI;

using GameData;

namespace Battle
{
	public class BattleStartUISetup : MonoBehaviour
	{
		[SerializeField]
		private GameObject PlayerText = null;
		[SerializeField]
		private GameObject EnemyText = null;
		[SerializeField]
		private GameObject PlayerNum = null;
		[SerializeField]
		private GameObject EnemyNum = null;

		// 初期化
		public void Setup()
		{
			Game mGame = Game.GetInstance();
			BattleDataIn battleDataIn = mGame.BattleIn;
			TerritoryDataFormat playerTerritory = mGame.TerritoryData[battleDataIn.PlayerTerritory];
			TerritoryDataFormat enemyTerritory = mGame.TerritoryData[battleDataIn.EnemyTerritory];
			Text text = PlayerText.GetComponent<Text>();
			text.text = playerTerritory.OwnerName;
			text = EnemyText.GetComponent<Text>();
			text.text = enemyTerritory.OwnerName;
			for (int i = 0; i < 2; i++)
			{
				int sNum = 0;
				for (int j = 0; j < 3; j++)
				{
					var id = (i == 0 ? battleDataIn.PlayerUnits[j] : battleDataIn.EnemyUnits[j]);
					if (id == -1)
						break;
					UnitDataFormat unit = mGame.UnitData[id];
					sNum += unit.SoldierNum;
				}
				text = (i == 0 ? PlayerNum : EnemyNum).GetComponent<Text>();
				text.text = (sNum != 0 ? sNum.ToString() : "");
			}

		}

		// Use this for initialization
		void Start()
		{
		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}
