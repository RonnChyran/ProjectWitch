using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ProjectWitch.Battle
{
	public class HPBarManager : MonoBehaviour
	{
		[SerializeField]
		private GameObject mPlayer = null, mEnemy = null;
		[SerializeField]
		private GameObject mPSNA = null, mPSNB = null, mPHPA = null, mPHPB = null;
		[SerializeField]
		private GameObject mTextPSN = null, mTextPHP = null;
		[SerializeField]
		private GameObject mESNA = null, mESNB = null, mEHPA = null, mEHPB = null, mECaA = null, mECaB = null;
		[SerializeField]
		private GameObject mTextESN = null, mTextEHP = null, mTextECa = null;

		public void HideAll()
		{
			mPlayer.SetActive(false);
			mEnemy.SetActive(false);
		}

		public void Hide(BattleUnit unit)
		{
			(unit.IsPlayer ? mPlayer : mEnemy).SetActive(false);
		}

		public void SetBar(BattleUnit unit, float expectedDamageSolNum, float expectedDamageHP, float expectedCap, float cap = 0)
		{
			Image SNA, SNB, HPA, HPB;
			Text textSN, textHP;
			if (unit.IsPlayer)
			{
				mPlayer.SetActive(true);
				SNA = mPSNA.GetComponent<Image>();
				SNB = mPSNB.GetComponent<Image>();
				HPA = mPHPA.GetComponent<Image>();
				HPB = mPHPB.GetComponent<Image>();
				textSN = mTextPSN.GetComponent<Text>();
				textHP = mTextPHP.GetComponent<Text>();
			}
			else
			{
				mEnemy.SetActive(true);
				SNA = mESNA.GetComponent<Image>();
				SNB = mESNB.GetComponent<Image>();
				HPA = mEHPA.GetComponent<Image>();
				HPB = mEHPB.GetComponent<Image>();
				mECaA.GetComponent<Image>().fillAmount = 1f - (unit.CaptureGauge + cap) / 100;
				mECaB.GetComponent<Image>().fillAmount = 1f - (unit.CaptureGauge + cap) / 100;
				textSN = mTextESN.GetComponent<Text>();
				textHP = mTextEHP.GetComponent<Text>();
				mTextECa.GetComponent<Text>().text = (int)(100 - (unit.CaptureGauge + cap)) + "/100";
			}
			if (unit.UnitData.MaxSoldierNum != 0)
			{
				SNA.fillAmount = (float)unit.DisplaySoldierNum / unit.UnitData.MaxSoldierNum;
				SNB.fillAmount = (unit.DisplaySoldierNum - expectedDamageSolNum) / unit.UnitData.MaxSoldierNum;
				textSN.text = unit.DisplaySoldierNum + "/" + unit.UnitData.MaxSoldierNum;
			}
			else
			{
				SNA.fillAmount = 0;
				SNB.fillAmount = 0;
				textSN.text = "0/0";
			}
			HPA.fillAmount = (float)unit.DisplayHP / unit.UnitData.MaxHP;
			HPB.fillAmount = (unit.DisplayHP - expectedDamageHP) / unit.UnitData.MaxHP;
			textHP.text = unit.DisplayHP + "/" + unit.UnitData.MaxHP;
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

