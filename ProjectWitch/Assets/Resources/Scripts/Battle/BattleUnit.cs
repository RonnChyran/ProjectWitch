using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using GameData;

namespace Battle
{
	// ポジション
	public enum Position : int
	{
		Front = 0,
		Middle,
		Rear,
	}

	public class UnitStatus
	{
		//威力
		public int Power { get; private set; }
		//種類
		public bool IsStatusUp { get; private set; }
		//ステ種類
		//[0]物功,[1]物防,[2]魔攻,[3]魔防,[4]機動,[5]指揮力,[6]地形補正
		public List<bool> Status { get; private set; }
		//効果時間
		public int Duration { get; set; }

		private UnitStatus(){}

		public UnitStatus(SkillDataFormat skillData)
		{
			Power = skillData.Power;
			IsStatusUp = (skillData.Type == SkillDataFormat.SkillType.StatusUp);
			Status = skillData.Status;
			Duration = skillData.Duration;
		}

	}

	// ユニットの全体操作クラス
	public class BattleUnit : MonoBehaviour
	{
        //デバッグテキスト出力用
        [SerializeField]
        private GameObject mDebugText = null;
        private DebugText mcDebugText = null;

        private readonly float OutDisplay = 7f;

		private Game mGame;
		private BattleData mBattle;
		private BattleObj mBattleObj;

		// リーダーオブジェクト
		private GameObject mLeaderObj;
		// 兵士オブジェクト
		private List<GameObject> mSoldierObj = new List<GameObject>();
		// スライドイン中かどうか
		private bool mIsSildeIn = false;
		// スライドアウト中かどうか
		private bool mIsSildeOut = false;


		#region 参照データ

		// ユニットID
		public int UnitID { get; private set; }

		public UnitDataFormat UnitData { get { return mGame.UnitData[UnitID]; } }
		// リーダーのAnimator
		public Animator LeaderAnimator { get; private set; }
		// 兵士のAnimator
		public List<Animator> GroupAnimators { get; private set; }
		// エリアデータ
		public BattleArea Area { get { return mBattleObj.Area; } }
		// リーダー攻撃スキル
		public SkillDataFormat LAtkSkill { get { return mGame.SkillData[UnitData.LAtkSkill]; } }
		// リーダー攻撃スキル
		public SkillDataFormat LDefSkill { get { return mGame.SkillData[UnitData.LDefSkill]; } }
		// 部下攻撃スキル
		public SkillDataFormat GAtkSkill { get { return mGame.SkillData[UnitData.GAtkSkill]; } }
		// AI
		public AIDataFormat AI { get { return mGame.AIData[UnitData.AIID]; } }
		// 装備
		public EquipmentDataFormat Equipment { get { return (UnitData.Equipment != -1 ? mGame.EquipmentData[UnitData.Equipment] : null); } }
		// 顔グラ
		public FaceObj Face { get; set; }

		#endregion

		#region 戦闘データ

		// 表示HP
		public int DisplayHP { get; private set; }
		// 表示兵士数
		public int DisplaySoldierNum { get; private set; }
		// 最大HP
		public int MaxHP { get { return UnitData.MaxHP + (IsEquipment ? Equipment.MaxHP : 0); } }
		// 捕獲ゲージ量
		public float CaptureGauge { get; private set; }

		// 行動順基準値
		public int OrderValue { get; set; }
		
		// 地形物理攻撃力修正
		private float AreaCorPAtk { get { return Area.CorPAtk * AreaCoePercentCoe; } }
		// 地形魔法攻撃力修正
		private float AreaCorMAtk { get { return Area.CorMAtk * AreaCoePercentCoe; } }
		// 地形物理防御力修正
		private float AreaCorPDef { get { return Area.CorPDef * AreaCoePercentCoe; } }
		// 地形魔法防御力修正
		private float AreaCorMDef { get { return Area.CorMDef * AreaCoePercentCoe; } }
		// 地形指揮力修正
		private float AreaCorLeadership { get { return Area.CorLeadership * AreaCoePercentCoe; } }
		// 地形機動力修正
		private float AreaCorAgility { get { return Area.CorAgility * AreaCoePercentCoe; } }

		// 物理攻撃力
		public float LPAtk { get { return (UnitData.LeaderPAtk + (IsEquipment ? Equipment.LeaderPAtk : 0)) * PhyAtkPercentCoe; } }
		// 魔法攻撃力
		public float LMAtk { get { return (UnitData.LeaderMAtk + (IsEquipment ? Equipment.LeaderMAtk : 0)) * MagAtkPercentCoe; } }
		// 物理防御力
		public float LPDef { get { return (UnitData.LeaderPDef + (IsEquipment ? Equipment.LeaderPDef : 0)) * (IsDefense ? 2 : 1) * PhyDefPercentCoe; } }
		// 魔法防御力
		public float LMDef { get { return (UnitData.LeaderMDef + (IsEquipment ? Equipment.LeaderMDef : 0)) * (IsDefense ? 2 : 1) * MagDefPercentCoe; } }
		// 指揮力
		public float Leadership { get { return (UnitData.Leadership + (IsEquipment ? Equipment.Leadership : 0)) * AreaCorLeadership * LeadershipPercentCoe; } }
		// 機動力
		public float Agility { get { return (UnitData.Agility + (IsEquipment ? Equipment.Agility : 0)) * AreaCorAgility * AgilityPercentCoe; } }
		
		// 集団物理攻撃力
		public float GPAtk { get { return (UnitData.GroupPAtk + (IsEquipment ? Equipment.GroupPAtk : 0)) * PhyAtkPercentCoe; } }
		// 集団魔法攻撃力
		public float GMAtk { get { return (UnitData.GroupMAtk + (IsEquipment ? Equipment.GroupMAtk : 0)) * MagAtkPercentCoe; } }
		// 集団物理防御力
		public float GPDef { get { return (UnitData.GroupPDef + (IsEquipment ? Equipment.GroupPDef : 0)) * (IsDefense ? 2 : 1) * PhyDefPercentCoe; } }
		// 集団魔法防御力
		public float GMDef { get { return (UnitData.GroupMDef + (IsEquipment ? Equipment.GroupMDef : 0)) * (IsDefense ? 2 : 1) * MagDefPercentCoe; } }

		// ポジション
		public Position Position { get; set; }
		// ポジションに対する物理係数
		private float PositionCoe
		{
			get
			{
				if (Position == Position.Front)
					return 1.0f;
				else if (Position == Position.Middle)
					return 0.8f;
				else if (Position == Position.Rear)
					return 0.5f;
				else
					return 1.0f;
			}
		}

		#endregion

		#region 状態異常系

		public List<UnitStatus> Status { get; set; }
		// 毒状態である
		public bool IsStatePoisom { get; set; }
		// 一度だけ無敵状態である
		public bool IsStateNoDamage { get; set; }
		// 身代わりスキル対象、nullのときはなし
		public BattleUnit GuardTarget { get; set; }
		// 召喚ユニットの場合持続時間、通常ユニットの場合-1
		public int SummonDuration { get; private set; }

		// 物理攻撃力修正パーセント
		public float PhyAtkPercent
		{
			get
			{
				float per = 0;
				foreach(var sta in Status)
					if (sta.Status[0])
						per += sta.Power * (sta.IsStatusUp ? 1 : -1);
				return per / 10;
			}
		}
		private float PhyAtkPercentCoe { get { return (1 + PhyAtkPercent / 100); } }
		// 物理防御力修正パーセント
		public float PhyDefPercent
		{
			get
			{
				float per = 0;
				foreach (var sta in Status)
					if (sta.Status[1])
						per += sta.Power * (sta.IsStatusUp ? 1 : -1);
				if (GuardTarget != null)
					per += LDefSkill.Power;
				return per / 10;
			}
		}
		private float PhyDefPercentCoe { get { return (1 + PhyDefPercent / 100); } }
		// 魔法攻撃力修正パーセント
		public float MagAtkPercent
		{
			get
			{
				float per = 0;
				foreach (var sta in Status)
					if (sta.Status[2])
						per += sta.Power * (sta.IsStatusUp ? 1 : -1);
				return per / 10;
			}
		}
		private float MagAtkPercentCoe { get { return (1 + MagAtkPercent / 100); } }
		// 魔法防御力修正パーセント
		public float MagDefPercent
		{
			get
			{
				float per = 0;
				foreach (var sta in Status)
					if (sta.Status[3])
						per += sta.Power * (sta.IsStatusUp ? 1 : -1);
				if (GuardTarget != null)
					per += LDefSkill.Power;
				return per / 10;
			}
		}
		private float MagDefPercentCoe { get { return (1 + MagDefPercent / 100); } }
		// 機動力修正パーセント
		public float AgilityPercent
		{
			get
			{
				float per = 0;
				foreach (var sta in Status)
					if (sta.Status[4])
						per += sta.Power * (sta.IsStatusUp ? 1 : -1);
				return per / 10;
			}
		}
		private float AgilityPercentCoe { get { return (1 + AgilityPercent / 100); } }
		// 指揮力修正パーセント
		public float LeadershipPercent
		{
			get
			{
				float per = 0;
				foreach (var sta in Status)
					if (sta.Status[5])
						per += sta.Power * (sta.IsStatusUp ? 1 : -1);
				return per / 10;
			}
		}
		private float LeadershipPercentCoe { get { return (1 + LeadershipPercent / 100); } }
		// 地形補正修正パーセント
		public float AreaCoePercent
		{
			get
			{
				float per = 0;
				foreach (var sta in Status)
					if (sta.Status[6])
						per += sta.Power * (sta.IsStatusUp ? 1 : -1);
				return per / 10;
			}
		}
		private float AreaCoePercentCoe { get { return (1 + AreaCoePercent / 100); } }

		#endregion

		#region フラグ系

		// 陣営(trueで自軍)
		public bool IsPlayer { get; private set; }
		// 装備品があるかどうか
		public bool IsEquipment { get { return UnitData.Equipment != -1; } }
		// 兵士が生き残っているか
		public bool IsExistSoldier { get { return UnitData.SoldierNum > 0; } }
		// 防御中かどうか
		public bool IsDefense { get; set; }
		// 移動中かどうか
		public bool IsMoving { get { return mIsSildeIn || mIsSildeOut; } }
		// 表示中かどうか
		public bool IsDisplay { get; private set; }
		// 召喚ユニットかどうか
		public bool IsSummonUnit { get { return SummonDuration != -1; } }
		// 捕獲されたかどうか
		public bool IsCapture { get { return CaptureGauge == 100; } }

		#endregion

		#region 戦闘データ算出

        //補正兵数計算
        private float GetCorrectSoldierNum()
        {
            var s = UnitData.SoldierNum;

            if (s < 1000) return s;
            else if (s < 2000) return (s - 1000f) / 2f + 1000f;
            else return (s - 2000f) / 4f + 1500f;
        }

        //集団ダメージ補正機
        private float CalcCorrectGroupDamage(float damage)
        {
            if (damage < 200) return damage;
            else if (damage < 500) return (damage - 200) / 2 + 200;
            else return (damage - 500) / 4 + 350;
        }

		// 集団攻撃ダメージ（物理）
		public float GetGroupPhyDamage()
		{
            if (GPAtk == 0) return 0;

            var result = ((Leadership + GPAtk) * GetCorrectSoldierNum() * AreaCorPAtk + mBattle.Rand1)
                * PositionCoe * mBattle.Coe5;
            result = CalcCorrectGroupDamage(result);

            return result;
		}

		// 集団攻撃ダメージ（魔法）
		public float GetGroupMagDamage()
		{
            if (GMAtk == 0) return 0;

            var result = ((Leadership + GMAtk) * GetCorrectSoldierNum() * AreaCorMAtk + mBattle.Rand2)
                *mBattle.Coe7;
            result = CalcCorrectGroupDamage(result);
            
            return result;
		}

		// 集団軽減ダメージ（物理）
		private float GetGroupPhyRedDamage()
		{
            var result = (Leadership + GPDef) * mBattle.Coe9 * AreaCorPDef + mBattle.Rand3;

            return result;
        }

		// 集団軽減ダメージ（物理）
		private float GetGroupMagRedDamage()
		{
            var result = (Leadership + GMDef) * mBattle.Coe11 * AreaCorMDef + mBattle.Rand4;

            return result;
        }


		// スキルダメージ
		public float GetSkillDamage(float baseATK, SkillDataFormat skill, bool isPhy)
		{
			return baseATK * skill.Power / 100 + (isPhy ? mBattle.Rand5 : mBattle.Rand6);
		}

		// リーダー攻撃ダメージ（物理）
		public float GetLeaderPhyDamage()
		{
            if (LPAtk == 0) return 0;

			return (LPAtk * LAtkSkill.Power / 100.0f * mBattle.Coe21 + mBattle.Rand5) * PositionCoe;
		}

		// リーダー攻撃ダメージ（魔法）
		public float GetLeaderMagDamage()
		{
            if (LMAtk == 0) return 0;

            return LMAtk * LAtkSkill.Power / 100.0f * mBattle.Coe22 + mBattle.Rand6;
		}

		// リーダー軽減ダメージ（物理）
		private float GetLeaderPhyRedDamage()
		{
			return LPDef * mBattle.Coe13 + mBattle.Rand7;
		}

		// リーダー軽減ダメージ（魔法）
		private float GetLeaderMagRedDamage()
		{
			return LMDef * mBattle.Coe14 + mBattle.Rand8;
		}

		// 軽減ダメージ（物理）
		private float GetPhyRedDamage()
		{
			return (IsExistSoldier ? GetGroupPhyRedDamage() : GetLeaderPhyRedDamage());
		}

		// 軽減ダメージ（魔法）
		private float GetMagRedDamage()
		{
			return (IsExistSoldier ? GetGroupMagRedDamage() : GetLeaderMagRedDamage());
		}

		// 通常ダメージ
		public float GetNormalDamage(float phyDamage, float magDamage, bool targetIsLeader)
		{
            float sufPhyDamage=0.0f;
            float sufMagDamage=0.0f;
            if (!IsExistSoldier || targetIsLeader)
            {
                sufPhyDamage = System.Math.Max(phyDamage * mBattle.Coe1 - GetPhyRedDamage() * mBattle.Coe2, 0);
                sufMagDamage = System.Math.Max(magDamage * mBattle.Coe3 - GetMagRedDamage() * mBattle.Coe4, 0);
            }
            else
            {
                sufPhyDamage = System.Math.Max(phyDamage * mBattle.Coe6 - GetPhyRedDamage() * mBattle.Coe8, 0);
                sufMagDamage = System.Math.Max(magDamage * mBattle.Coe10 - GetMagRedDamage() * mBattle.Coe12, 0);
            }
            var result = sufPhyDamage + sufMagDamage;
            return result;
        }

		// 捕獲ダメージ
		public float GetCaptureDamage(float phyDamage, float magDamage)
		{
			return GetNormalDamage(phyDamage, magDamage, true) * mBattle.Coe15;
		}

		// カウンターダメージ
		public float GetCounterDamage(float phyDamage, float magDamage)
		{
			return GetNormalDamage(phyDamage, magDamage, false) * mBattle.Coe16;
		}

		// 捕獲カウンターダメージ
		public float GetCaptureCounterDamage(float phyDamage, float magDamage)
		{
			return GetNormalDamage(phyDamage, magDamage, false) * mBattle.Coe17;
		}

		// 兵士回復量
		public float GetGroupCurativeAmount(SkillDataFormat skillData)
		{
			return UnitData.MaxSoldierNum * skillData.Power * mBattle.Coe18;
		}

		// HP回復量
		public float GetLeaderCurativeAmount(SkillDataFormat skillData)
		{
			return MaxHP * skillData.Power * mBattle.Coe19;
		}

		// 毒ダメージ
		public float GetPoisonDamagePerTurn()
		{
			return (IsExistSoldier ? UnitData.SoldierNum : UnitData.HP) * mBattle.Coe20 * mBattle.Rand9;
		}

		// 行動順基準値
		public int GetActionOrderValue()
		{
			return 1000 - (int)Agility + mBattle.Rand10;
		}

		#endregion

		// 各種最初のセットアップ
		public void Setup(int id, bool isPlayer, int pos, BattleObj bo, int summonDuration = -1)
		{
			IsPlayer = isPlayer;
			mGame = Game.GetInstance();
			mBattle = BattleData.GetInstance();
			UnitID = id;
			mBattleObj = bo;
			ApproachDisplay(0);

			IsDisplay = false;
			IsStatePoisom = false;
			IsStateNoDamage = false;
			GuardTarget = null;
			SummonDuration = summonDuration;
			Status = new List<UnitStatus>();

			if (pos == 0)
				Position = Position.Front;
			else if (pos == 1)
				Position = Position.Middle;
			else
				Position = Position.Rear;
			transform.name = "Unit" + id;

			// オブジェクト生成
			// リーダー生成
			var leaderPos = transform.FindChild("LeaderMediumPos");
			var leader = (GameObject)Resources.Load("Prefabs/Battle/" + UnitData.BattleLeaderPrefabPath);
			mLeaderObj = BattleData.Instantiate(leader, "leader", transform).gameObject;
			mLeaderObj.transform.localPosition = new Vector3(System.Math.Abs(leaderPos.localPosition.x) *
				(IsPlayer ? -1 : 1), leader.transform.localPosition.y, leaderPos.localPosition.z);
			mLeaderObj.transform.localScale = new Vector3(System.Math.Abs(mLeaderObj.transform.localScale.x) *
				(IsPlayer ? -1 : 1), mLeaderObj.transform.localScale.y, mLeaderObj.transform.localScale.z);
			// リーダーのアニメーター
			foreach (Transform child in mLeaderObj.transform)
			{
				LeaderAnimator = child.GetComponent<Animator>();
				if (LeaderAnimator != null)
					break;
			}

			// ユニット生成
			GroupAnimators = new List<Animator>();
			Transform soldiderPosObj;
			var soldierPrefab = (GameObject)Resources.Load("Prefabs/Battle/" + UnitData.BattleGroupPrefabPath);
			var soldier = transform.FindChild("soldier");
			switch (UnitData.GUnitSize)
			{
				case 0:
					soldiderPosObj = bo.SoldierSmall;
					break;
				case 1:
					soldiderPosObj = bo.SoldierMedium;
					break;
				case 2:
					soldiderPosObj = bo.SoldierLarge;
					break;
				case 3:
					soldiderPosObj = bo.SoldierSuperLarge;
					break;
				default:
					soldiderPosObj = new GameObject().transform;
					break;
			}
			foreach (Transform child in soldiderPosObj)
			{
				var obj = BattleData.Instantiate(soldierPrefab, child.gameObject.name, soldier).gameObject;
				obj.transform.localPosition = new Vector3(System.Math.Abs(child.localPosition.x) *
					(IsPlayer ? -1 : 1), child.localPosition.y, child.localPosition.z);
				obj.transform.localScale = new Vector3(System.Math.Abs(obj.transform.localScale.x) *
					(IsPlayer ? -1 : 1), obj.transform.localScale.y, obj.transform.localScale.z);
				mSoldierObj.Add(obj);
				// 兵士のアニメーター
				foreach (Transform c in obj.transform)
				{
					var ani = c.GetComponent<Animator>();
					if (ani != null)
					{
						ani.Rebind();
						GroupAnimators.Add(ani);
						break;
					}
				}
			}
			SetDisplaySoldier();
			transform.position = new Vector3(OutDisplay * (IsPlayer ? -1 : 1), 0, 0);

			// 状態を待機にセット
			SetLeaderAnimatorState(0);
			SetGroupAnimatorState(0);
		}

		// 召喚ユニットのセットアップ
		public void SetupSummon(SkillDataFormat skillData, bool isPlayer, BattleObj bo)
		{
			Setup(skillData.SummonUnit, isPlayer, 0, bo, skillData.Duration);
		}

		#region 表示関連

		// 表示する兵士数を増減する
		public void SetDisplaySoldier()
		{
            int nextDisplayNum = 0;
            if (UnitData.MaxSoldierNum != 0)
                nextDisplayNum = (int)System.Math.Ceiling((float)(mSoldierObj.Count) * DisplaySoldierNum / UnitData.MaxSoldierNum);

            int preDisplayNum = 0;
			foreach (var soldier in mSoldierObj)
			{
				if (soldier.activeSelf)
					++preDisplayNum;
			}
			if (nextDisplayNum < preDisplayNum)
			{
				// 減らす
				for (; nextDisplayNum != preDisplayNum; --preDisplayNum)
				{
					int rand = Random.Range(0, preDisplayNum);
					int id = 0;
					for (int count = 0; count < rand; ++id)
					{
						if (mSoldierObj[id].activeSelf)
							++count;
					}
					while (!mSoldierObj[id].activeSelf)
						++id;
					mSoldierObj[id].SetActive(false);
				}
			}
			else
			{
				// 増やす
				for (; nextDisplayNum != preDisplayNum; ++preDisplayNum)
				{
					int rand = Random.Range(0, mSoldierObj.Count - preDisplayNum);
					int id = 0;
					for (int count = 0; count < rand; ++id)
					{
						if (!mSoldierObj[id].activeSelf)
							++count;
					}
					while (mSoldierObj[id].activeSelf)
						++id;
					mSoldierObj[id].SetActive(true);
				}
			}
		}

		// 描画位置にスライドさせるコルーチン
		private IEnumerator CoSlideIn()
		{
			if (mIsSildeIn)
				yield break;
			if (mIsSildeOut)
				StopCoroutine("CoSlideOut");
			mIsSildeIn = true;
			mIsSildeOut = false;
			float speedPerSec = 25f * mBattleObj.BattleSpeedMagni;
			while (IsPlayer ? transform.position.x < 0 : transform.position.x > 0)
			{
				transform.position += new Vector3(speedPerSec * Time.deltaTime * (IsPlayer ? 1 : -1), 0, 0);
				if(!(IsPlayer ? transform.position.x < 0 : transform.position.x > 0))
					transform.position = new Vector3(0, transform.position.y, transform.position.z);
				yield return null;
			}
			transform.position = new Vector3(0, transform.position.y, transform.position.z);
			mIsSildeIn = false;
			IsDisplay = true;
		}

		// 描画画面外にスライドさせるコルーチン
		private IEnumerator CoSlideOut()
		{
			if (mIsSildeOut)
				yield break;
			if (mIsSildeIn)
				StopCoroutine("CoSlideIn");
			mIsSildeOut = true;
			mIsSildeIn = false;
			float speedPerSec = 50f * mBattleObj.BattleSpeedMagni;
			while (IsPlayer ? transform.position.x > -OutDisplay : transform.position.x < OutDisplay)
			{
				transform.position += new Vector3(speedPerSec * Time.deltaTime * (IsPlayer ? -1 : 1), 0, 0);
				yield return null;
			}
			transform.position = new Vector3(OutDisplay * (IsPlayer ? -1 : 1), transform.position.y, transform.position.z);

			mIsSildeOut = false;
			IsDisplay = false;
		}

		public IEnumerator SlideIn()
		{
			yield return StartCoroutine("CoSlideIn");
		}

		public IEnumerator SlideOut()
		{
			yield return StartCoroutine("CoSlideOut");
		}

		// リーダーのアニメーターの表示をセットする
		//　0:待機　1:攻撃　2:防御
		public void SetLeaderAnimatorState(int state)
		{
			LeaderAnimator.SetInteger("State", state);
		}

		// 兵士のアニメーターの表示をセットする
		//　0:待機　1:攻撃　2:防御
		public void SetGroupAnimatorState(int state)
		{
			foreach (var ani in GroupAnimators)
			{
				if (ani.isActiveAndEnabled)
					ani.SetInteger("State", state);
			}
		}

		#endregion

		// 表示用HPを現在HPに近づける
		public void ApproachDisplay(int num)
		{
			if (num <= 0)
			{
				DisplayHP = UnitData.HP;
				DisplaySoldierNum = UnitData.SoldierNum;
			}
			else
			{
				DisplayHP += (UnitData.HP - DisplayHP) / num;
				DisplaySoldierNum += (UnitData.SoldierNum - DisplaySoldierNum) / num;
			}
		}

		// ダメージを受ける
		public void SufferDamage(float damage, bool toLeader = false)
		{
			if (!IsExistSoldier || toLeader)
				UnitData.HP = System.Math.Max(UnitData.HP - (int)damage, 0);
			else
				UnitData.SoldierNum = System.Math.Max(UnitData.SoldierNum - (int)damage, 0);
		}

		// 回復する
		public void Healed(float healHP, float healSolNum)
		{
			UnitData.HP = System.Math.Min(UnitData.HP + (int)healHP, MaxHP);
			UnitData.SoldierNum = System.Math.Min(UnitData.SoldierNum + (int)healSolNum, UnitData.MaxSoldierNum);
		}

		// 捕獲ダメージを受ける
		public void SufferCaptureDamage(float damage)
		{
			CaptureGauge += damage / UnitData.HP;
			if (CaptureGauge > 100)
				CaptureGauge = 100;
		}

		// 状態異常を受ける
		public void SufferStatus(SkillDataFormat skillData)
		{
			Status.Add(new UnitStatus(skillData));
		}

		// 状態異常をリセットする
		public void ResetStatus()
		{
			Status.Clear();
		}

		// ターン終了時処理
		public IEnumerator TurnEnd()
		{
			if (SummonDuration != -1)
			{
				--SummonDuration;
				if (SummonDuration == 0)
				{
					// 召喚ユニット帰還
					List<BattleUnit> blist = new List<BattleUnit>();
					blist.Add(this);
					yield return mBattleObj.OrderController.DeadOut(blist);
					(IsPlayer ? mBattleObj.PlayerSummonUnits : mBattleObj.EnemySummonUnits).Remove(this);
				}
			}
			for (int i = Status.Count - 1; i >= 0; --i)
			{
				if (Status[i].Duration != 0)
				{
					--Status[i].Duration;
					if(Status[i].Duration <= 0)
						Status.Remove(Status[i]);
				}
			}
		}

		// Use this for initialization
		void Start()
        {
        }

		// Update is called once per frame
		void Update()
		{
			//if (Input.GetMouseButtonDown(0))
			//	SlideIn();
			//if (Input.GetMouseButtonDown(1))
			//	SlideOut();
			//if (BattleUnitData != null && Input.GetMouseButtonDown(2))
			//{
			//	UnitData.SoldierNum = Random.Range(0, UnitData.MaxSoldierNum + 1);
			//	SetDisplaySoldier();
			//}
		}
	}

}
