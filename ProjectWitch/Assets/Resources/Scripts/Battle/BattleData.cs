//author	: masanori.k
//version	: 0.1.0
//summary	: 戦闘に関するデータを扱うクラス群

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GameData;

namespace Battle
{
	public class BattleData : MonoBehaviour
	{
		private static BattleData battleData;

		public static BattleData GetInstance()
		{
			if (battleData == null)
				battleData = GameObject.Find("BattleObject").GetComponent<BattleData>();
			return battleData;
		}

		[SerializeField, Tooltip("物理ダメージ、全体"), HeaderAttribute("係数")]
		private float mCoe1 = 0;
		[SerializeField, Tooltip("物理軽減ダメージ、全体")]
		private float mCoe2 = 0;
		[SerializeField, Tooltip("魔法ダメージ、全体")]
		private float mCoe3 = 0;
		[SerializeField, Tooltip("魔法軽減ダメージ、全体")]
		private float mCoe4 = 0;
		[SerializeField, Tooltip("集団攻撃ダメージ、指揮能力（物理）")]
		private float mCoe5 = 0;
		[SerializeField, Tooltip("集団攻撃ダメージ、集団物理攻撃力")]
		private float mCoe6 = 0;
		[SerializeField, Tooltip("集団攻撃ダメージ、指揮能力（魔法）")]
		private float mCoe7 = 0;
		[SerializeField, Tooltip("集団攻撃ダメージ、集団魔法攻撃力")]
		private float mCoe8 = 0;
		[SerializeField, Tooltip("集団軽減ダメージ、指揮能力（物理）")]
		private float mCoe9 = 0;
		[SerializeField, Tooltip("集団軽減ダメージ、集団物理防御力")]
		private float mCoe10 = 0;
		[SerializeField, Tooltip("集団軽減ダメージ、指揮能力（魔法）")]
		private float mCoe11 = 0;
		[SerializeField, Tooltip("集団軽減ダメージ、集団魔法防御力")]
		private float mCoe12 = 0;
		[SerializeField, Tooltip("リーダー軽減ダメージ、物理防御力1")]
		private float mCoe13 = 0;
		[SerializeField, Tooltip("リーダー軽減ダメージ、魔法防御力1")]
		private float mCoe14 = 0;
		[SerializeField, Tooltip("捕獲ダメージ")]
		private float mCoe15 = 0;
		[SerializeField, Tooltip("物理カウンターダメージ")]
		private float mCoe16 = 0;
		[SerializeField, Tooltip("捕獲カウンターダメージ")]
		private float mCoe17 = 0;
		[SerializeField, Tooltip("兵士回復量")]
		private float mCoe18 = 0;
		[SerializeField, Tooltip("HP回復量")]
		private float mCoe19 = 0;
		[SerializeField, Tooltip("毒ダメージ")]
		private float mCoe20 = 0;
		[SerializeField, Tooltip("リーダー攻撃ダメージ、物理攻撃力1")]
		private float mCoe21 = 0;
		[SerializeField, Tooltip("リーダー攻撃ダメージ、魔法攻撃力1")]
		private float mCoe22 = 0;
		[SerializeField, Tooltip("リーダー攻撃ダメージ、物理攻撃力2")]
		private float mCoe23 = 0;
		[SerializeField, Tooltip("リーダー攻撃ダメージ、魔法攻撃力2")]
		private float mCoe24 = 0;
		[SerializeField, Tooltip("リーダー軽減ダメージ、物理攻撃力2")]
		private float mCoe25 = 0;
		[SerializeField, Tooltip("リーダー軽減ダメージ、魔法攻撃力2")]
		private float mCoe26 = 0;
		[SerializeField, Tooltip("集団攻撃ダメージ（物理）、中央値"), HeaderAttribute("乱数")]
		private int mRandMed1 = 0;
		[SerializeField, Tooltip("集団攻撃ダメージ（物理）、振幅")]
		private int mRandAmp1 = 0;
		[SerializeField, Tooltip("集団攻撃ダメージ（魔法）、中央値")]
		private int mRandMed2 = 0;
		[SerializeField, Tooltip("集団攻撃ダメージ（魔法）、振幅")]
		private int mRandAmp2 = 0;
		[SerializeField, Tooltip("集団軽減ダメージ（物理）、中央値")]
		private int mRandMed3 = 0;
		[SerializeField, Tooltip("集団軽減ダメージ（物理）、振幅")]
		private int mRandAmp3 = 0;
		[SerializeField, Tooltip("集団軽減ダメージ（魔法）、中央値")]
		private int mRandMed4 = 0;
		[SerializeField, Tooltip("集団軽減ダメージ（魔法）、振幅")]
		private int mRandAmp4 = 0;
		[SerializeField, Tooltip("リーダー攻撃ダメージ（物理）、中央値")]
		private int mRandMed5 = 0;
		[SerializeField, Tooltip("リーダー攻撃ダメージ（物理）、振幅")]
		private int mRandAmp5 = 0;
		[SerializeField, Tooltip("リーダー攻撃ダメージ（魔法）、中央値")]
		private int mRandMed6 = 0;
		[SerializeField, Tooltip("リーダー攻撃ダメージ（魔法）、振幅")]
		private int mRandAmp6 = 0;
		[SerializeField, Tooltip("リーダー軽減ダメージ（物理）、中央値")]
		private int mRandMed7 = 0;
		[SerializeField, Tooltip("リーダー軽減ダメージ（物理）、振幅")]
		private int mRandAmp7 = 0;
		[SerializeField, Tooltip("リーダー軽減ダメージ（魔法）、中央値")]
		private int mRandMed8 = 0;
		[SerializeField, Tooltip("リーダー軽減ダメージ（魔法）、振幅")]
		private int mRandAmp8 = 0;
		[SerializeField, Tooltip("毒ダメージ、中央値")]
		private int mRandMed9 = 0;
		[SerializeField, Tooltip("毒ダメージ、振幅")]
		private int mRandAmp9 = 0;
		[SerializeField, Tooltip("行動順基準値、中央値")]
		private int mRandMed10 = 0;
		[SerializeField, Tooltip("行動順基準値、振幅")]
		private int mRandAmp10 = 0;

		public float Coe1 { get { return mCoe1; } }
		public float Coe2 { get { return mCoe2; } }
		public float Coe3 { get { return mCoe3; } }
		public float Coe4 { get { return mCoe4; } }
		public float Coe5 { get { return mCoe5; } }
		public float Coe6 { get { return mCoe6; } }
		public float Coe7 { get { return mCoe7; } }
		public float Coe8 { get { return mCoe8; } }
		public float Coe9 { get { return mCoe9; } }
		public float Coe10 { get { return mCoe10; } }
		public float Coe11 { get { return mCoe11; } }
		public float Coe12 { get { return mCoe12; } }
		public float Coe13 { get { return mCoe13; } }
		public float Coe14 { get { return mCoe14; } }
		public float Coe15 { get { return mCoe15; } }
		public float Coe16 { get { return mCoe16; } }
		public float Coe17 { get { return mCoe17; } }
		public float Coe18 { get { return mCoe18; } }
		public float Coe19 { get { return mCoe19; } }
		public float Coe20 { get { return mCoe20; } }
		public float Coe21 { get { return mCoe21; } }
		public float Coe22 { get { return mCoe22; } }
		public float Coe23 { get { return mCoe23; } }
		public float Coe24 { get { return mCoe24; } }
		public float Coe25 { get { return mCoe25; } }
		public float Coe26 { get { return mCoe26; } }
		int GetRand(int med, int amp)
		{
			return med - amp / 2 + BattleRandom.Range(0, amp);
		}
		public int Rand1 { get { return GetRand(mRandMed1, mRandAmp1); } }
		public int Rand2 { get { return GetRand(mRandMed2, mRandAmp2); } }
		public int Rand3 { get { return GetRand(mRandMed3, mRandAmp3); } }
		public int Rand4 { get { return GetRand(mRandMed4, mRandAmp4); } }
		public int Rand5 { get { return GetRand(mRandMed5, mRandAmp5); } }
		public int Rand6 { get { return GetRand(mRandMed6, mRandAmp6); } }
		public int Rand7 { get { return GetRand(mRandMed7, mRandAmp7); } }
		public int Rand8 { get { return GetRand(mRandMed8, mRandAmp8); } }
		public int Rand9 { get { return GetRand(mRandMed9, mRandAmp9); } }
		public int Rand10 { get { return GetRand(mRandMed10, mRandAmp10); } }

		public static Transform Instantiate(GameObject gameObject, string name, Transform parent = null)
		{
			var tf = GameObject.Instantiate(gameObject).transform;
			tf.name = name;
			if (parent != null)
				tf.parent = parent;
			return tf;
		}
	}

	public class BattleArea
	{
		private Game mGame = null;
		public int AreaID { get { return mGame.BattleIn.AreaID; } }
		public AreaDataFormat AreaData { get { return mGame.AreaData[AreaID]; } }
		public AreaBattleFactor BattleFactor { get { return AreaData.BattleFactor; } }
		private GameObject mAreaObj;

		//コンストラクタ
		public BattleArea(GameObject parent)
		{
			mGame = Game.GetInstance();

			if (AreaID < 0 || AreaID >= mGame.AreaData.Count)
				return;
			mAreaObj = (GameObject)Resources.Load("Prefabs/Background3D/" + AreaData.BackgroundName);
			var inst = MonoBehaviour.Instantiate(mAreaObj);
			inst.transform.SetParent(parent.transform);
		}

		// 地形補正
		// 物理攻撃力
		public float CorPAtk { get { return 1.0f + (BattleFactor.PAtk / 100.0f); } }
		// 魔法攻撃力
		public float CorMAtk { get { return 1.0f + (BattleFactor.MAtk / 100.0f); } }
		// 物理防御力
		public float CorPDef { get { return 1.0f + (BattleFactor.PDef / 100.0f); } }
		// 魔法防御力
		public float CorMDef { get { return 1.0f + (BattleFactor.MDef / 100.0f); } }
		// 指揮力
		public float CorLeadership { get { return 1.0f + (BattleFactor.Leadership / 100.0f); } }
		// 機動力
		public float CorAgility { get { return 1.0f + (BattleFactor.Agility / 100.0f); } }
	}
}
