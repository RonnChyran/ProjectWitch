using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ProjectWitch.Battle
{
	public class TurnNumDisplay : MonoBehaviour
	{
		private BattleObj mBattleObj;
		private int mTurnNum = 0;
		private Text TurnNumText { get { return transform.Find("TurnNum").GetComponent<Text>(); } }

		public void SetUp(BattleObj battleObj)
		{
			mBattleObj = battleObj;
			mTurnNum = mBattleObj.TurnNum;
			if (mTurnNum == -1)
				TurnNumText.text = "∞";
			else
				TurnNumText.text = mTurnNum.ToString();
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{
			if(mTurnNum != mBattleObj.TurnNum)
			{
				mTurnNum = mBattleObj.TurnNum;
				TurnNumText.text = mTurnNum.ToString();
			}
		}
	}
}
