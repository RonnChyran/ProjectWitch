﻿//author	: masanori.k
//version	: 0.1.0
//summary	: 戦闘全体を制御するコンポーネント

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;

using GameData;

namespace Battle
{
	public enum DamageType : int
	{
		Normal = 0,
		Counter,
		CaptureCounter,
		Poison,
	}

	public class BattleObj : MonoBehaviour
	{
		private Game mGame = null;

		// ユニットプレハブ
		[SerializeField]
		private GameObject mUnitPrefab = null;
		// プレイヤー空オブジェクト
		[SerializeField]
		private Transform mTfPlayer = null, mTfEnemy = null;
		// 兵士配置位置プレハブ
		[SerializeField]
		private Transform mSoldierSmall = null, mSoldierMedium = null, mSoldierLarge = null, mSoldierSuperLarge = null;
		public Transform SoldierSmall { get { return mSoldierSmall; } }
		public Transform SoldierMedium { get { return mSoldierMedium; } }
		public Transform SoldierLarge { get { return mSoldierLarge; } }
		public Transform SoldierSuperLarge { get { return mSoldierSuperLarge; } }
		// 顔グラ
		[SerializeField]
		private GameObject mFaceP0 = null, mFaceP1 = null, mFaceP2 = null, mFaceE0 = null, mFaceE1 = null, mFaceE2 = null;
		public FaceObj FaceP0 { get { return mFaceP0.GetComponent<FaceObj>(); } }
		public FaceObj FaceP1 { get { return mFaceP1.GetComponent<FaceObj>(); } }
		public FaceObj FaceP2 { get { return mFaceP2.GetComponent<FaceObj>(); } }
		public FaceObj FaceE0 { get { return mFaceE0.GetComponent<FaceObj>(); } }
		public FaceObj FaceE1 { get { return mFaceE1.GetComponent<FaceObj>(); } }
		public FaceObj FaceE2 { get { return mFaceE2.GetComponent<FaceObj>(); } }
		public List<FaceObj> PlayerFaces { get; private set; }
		public List<FaceObj> EnemyFaces { get; private set; }
		// カード
		[SerializeField]
		private GameObject mCardP0 = null, mCardP1 = null, mCardP2 = null, mCardE0 = null, mCardE1 = null, mCardE2 = null;
		public List<GameObject> PlayerCardObjs { get; private set; }
		public List<GameObject> EnemyCardObjs { get; private set; }
		// ターン数表示
		[SerializeField]
		private GameObject mTurnNumDisplay = null;
		// 順番表示
		[SerializeField]
		private GameObject mBehaviorOrder = null;
		// 捕獲ゲージ
		[SerializeField]
		private GameObject mCaptureGauge = null;
		public Image CaptureGauge { get { return (mCaptureGauge != null ? mCaptureGauge.GetComponent<Image>() : null); } }
		// マウスカバー
		[SerializeField]
		private GameObject mMouseCover = null;
		public OrderController OrderController { get; private set; }
		// 撤退確認UI
		[SerializeField]
		private GameObject mEscapeConfUI = null;
		// 設定UI
		[SerializeField]
		private GameObject mConfigUI = null;
		// 戦闘開始UI
		[SerializeField]
		private GameObject mBattleStartUI = null;
		// 戦闘開始エフェクト
		[SerializeField]
		private GameObject mBattleStartEffect = null;
		// 戦闘勝利UI
		[SerializeField]
		private GameObject mBattleWinUI = null;
		// 戦闘勝利エフェクト
		[SerializeField]
		private GameObject mBattleWinEffect = null;
		// 戦闘敗北UI
		[SerializeField]
		private GameObject mBattleLoseUI = null;
		// 戦闘敗北エフェクト
		[SerializeField]
		private GameObject mBattleLoseEffect = null;
		// ユニット消去セリフUI
		[SerializeField]
		private GameObject mLastSerifUI = null;
		// メッセージUI
		[SerializeField]
		private GameObject mMessageUI = null;
		// カード起動UI
		[SerializeField]
		private GameObject mCardStartUI = null;
		// カード起動エフェクト
		[SerializeField]
		private GameObject mCardStartEffect = null;
		// 背景の親オブジェクト
		[SerializeField]
		private GameObject mBGParent = null;
		// 音楽再生
		[SerializeField]
		private PlayMusic mMusic = null;
		// SkyBoxMaterial
		[SerializeField]
		private Material mSkyBoxDay = null, mSkyBoxSunset = null, mSkyBoxNight = null;
		// DamageDisplay
		[SerializeField]
		private GameObject mDamageDisplayPlayer = null, mDamageDisplayEnemy = null;
		// 最終呼び出しevent
		[SerializeField]
		public UnityEvent EndEvent = null;

		// 戦闘入力データ
		public BattleDataIn BattleDataIn { get { return mGame.BattleIn; } }
		// 戦闘出力データ
		public BattleDataOut BattleDataOut { get { return mGame.BattleOut; } }
		// 味方データ
		public List<BattleUnit> PlayerUnits { get; private set; }
		// 敵データ
		public List<BattleUnit> EnemyUnits { get; private set; }
		// スキル参照ユニット(アリス)
		public BattleUnit UnitAlice { get; private set; }
		// 味方召喚ユニット
		public List<BattleUnit> PlayerSummonUnits { get; private set; }
		// 敵召喚ユニット
		public List<BattleUnit> EnemySummonUnits { get; private set; }
		// 味方カード
		public List<CardDataFormat> PlayerCards { get; private set; }
		// 敵カード
		public List<CardDataFormat> EnemyCards { get; private set; }
		// 領地データ
		public BattleArea Area { get; private set; }
		// 侵攻(true)か防衛(false)か
		public bool IsInvasion { get { return BattleDataIn.IsInvasion; } }
		// 音楽再生
		public PlayMusic Music { get { return mMusic; } }
		// ターン数
		public int TurnNum { get; set; }
		// エフェクトが動作中である
		public bool IsEffect { get; private set; }
		// 戦闘が終了したがどうか
		public bool IsBattleEnd { get; private set; }
		// 戦闘終了後の入力待機時間かどうか
		public bool IsEndWaitTime { get; private set; }
		// プレイヤーが選択するときかどうか
		public bool IsPlayerSelectTime { get; private set; }
		// 入力待ちかどうか
		public bool IsWaitInputTime { get; private set; }
		// プレイヤーの行動中かどうか
		public bool IsPlayerActionTime { get; private set; }
		// ユニット増減中かどうか
		public bool IsNowIncDec { get; private set; }
		// 最終的な攻撃対象
		public BattleUnit EndTargetUnit { get; private set; }
		public float DamageNum { get; private set; }
		// 撤退したかどうか
		public bool IsEscape { get; private set; }
		// 戦闘速度
		public int BattleSpeed { get { return mGame.Config.BattleSpeed; } set { mGame.Config.BattleSpeed = value; } }
		// 戦闘速度倍率
		public float BattleSpeedMagni { get { return (float)System.Math.Pow(2.0f, BattleSpeed); } }
		// エフェクトFXController
		public FXController FXCtrl { get; private set; }

		public BattleUnit TurnUnit { get { return OrderController.TurnUnit; } }

		// 適当なデータを入れる関数
		void SetTest()
		{
			var battleIn = mGame.BattleIn;

			// エリア
			battleIn.AreaID = 5;
			var adf = mGame.AreaData[battleIn.AreaID];
			adf.BattleFactor.PAtk =
				adf.BattleFactor.MAtk =
				adf.BattleFactor.PDef =
				adf.BattleFactor.MDef =
				adf.BattleFactor.Leadership =
				adf.BattleFactor.Agility = 1;


			// キャラクター
			var ud = mGame.UnitData[0];
			ud.BattleGroupPrefabPath = "bv_devil";
			mGame.SkillData[ud.LAtkSkill].Range = SkillDataFormat.SkillRange.All;
			ud = mGame.UnitData[1];
			ud.BattleLeaderPrefabPath = "bv_orivia";
			ud.GUnitSize = 0;
			ud.BattleGroupPrefabPath = "bv_magicleader";
			ud = mGame.UnitData[6];
			ud.BattleLeaderPrefabPath = "bv_orivia";
			ud.BattleGroupPrefabPath = "bv_dragon";
			ud.GUnitSize = 2;
			ud = mGame.UnitData[7];
			ud.BattleLeaderPrefabPath = "bv_alice";
			ud.BattleGroupPrefabPath = "bv_dragon";
			ud.GUnitSize = 2;

			battleIn.PlayerUnits[0] = 1;
			battleIn.PlayerUnits[1] = 0;
			battleIn.EnemyUnits[0] = 7;
			battleIn.EnemyUnits[1] = 6;

			// 乱数はランダム
			BattleRandom.IsFix = false;
		}

		// データのロード
		void LoadData()
		{
			mGame = Game.GetInstance();

			//			BattleSpeed = 2;
			//			SetTest();
			//BattleDataIn.PlayerCards[0] = 0;

			mEscapeConfUI.SetActive(false);
			mConfigUI.SetActive(false);
			mBattleWinUI.SetActive(false);
			mBattleLoseUI.SetActive(false);
			mLastSerifUI.SetActive(false);
			mMessageUI.SetActive(false);
			mCardStartUI.SetActive(false);
			Area = new BattleArea(mBGParent);

			IsEffect = false;
			IsBattleEnd = false;
			IsEscape = false;
			IsEndWaitTime = false;
			IsWaitInputTime = false;

			print("顔グラ設定");
			PlayerFaces = new List<FaceObj>();
			PlayerFaces.Add(FaceP0);
			PlayerFaces.Add(FaceP1);
			PlayerFaces.Add(FaceP2);
			for (int i = 0; i < PlayerFaces.Count; ++i)
				PlayerFaces[i].SetPos(i, true);
			EnemyFaces = new List<FaceObj>();
			EnemyFaces.Add(FaceE0);
			EnemyFaces.Add(FaceE1);
			EnemyFaces.Add(FaceE2);
			for (int i = 0; i < EnemyFaces.Count; ++i)
				EnemyFaces[i].SetPos(i, false);
			PlayerCardObjs = new List<GameObject>();
			PlayerCardObjs.Add(mCardP0);
			PlayerCardObjs.Add(mCardP1);
			PlayerCardObjs.Add(mCardP2);
			EnemyCardObjs = new List<GameObject>();
			EnemyCardObjs.Add(mCardE0);
			EnemyCardObjs.Add(mCardE1);
			EnemyCardObjs.Add(mCardE2);

			UnitAlice = BattleData.Instantiate(mUnitPrefab, "", mTfPlayer).GetComponent<BattleUnit>();
			UnitAlice.Setup(0, true, 0, this);

			print("プレイヤーユニット設定");
			PlayerUnits = new List<BattleUnit>();
			for (int i = 0; i < BattleDataIn.PlayerUnits.Count; ++i)
			{
				var id = BattleDataIn.PlayerUnits[i];
				if (id == -1)
				{
					PlayerFaces[i].SetNoneUnit();
					continue;
				}
				Transform go = BattleData.Instantiate(mUnitPrefab, "", mTfPlayer);
				var bu = go.GetComponent<BattleUnit>();
				bu.Setup(id, true, i, this);
				PlayerUnits.Add(bu);
				PlayerFaces[i].SetUnit(PlayerUnits[i]);
			}
			print("エネミーユニット設定");
			EnemyUnits = new List<BattleUnit>();
			for (int i = 0; i < BattleDataIn.EnemyUnits.Count; ++i)
			{
				var id = BattleDataIn.EnemyUnits[i];
				if (id == -1)
				{
					EnemyFaces[i].SetNoneUnit();
					continue;
				}
				if (id == -1)
					break;
				Transform go = BattleData.Instantiate(mUnitPrefab, "", mTfEnemy);
				var bu = go.GetComponent<BattleUnit>();
				bu.Setup(id, false, i, this);
				EnemyUnits.Add(bu);
				EnemyFaces[i].SetUnit(EnemyUnits[i]);
			}
			print("プレイヤーカード設定");
			PlayerCards = new List<CardDataFormat>();
			for (int i = 0; i < BattleDataIn.PlayerCards.Count; i++)
			{
				var id = BattleDataIn.PlayerCards[i];
				PlayerCardObjs[i].SetActive(id != -1);
				PlayerCardObjs[i].transform.FindChild("Flame").gameObject.SetActive(false);
				if (id == -1)
					continue;
				var card = mGame.CardData[id];
				PlayerCardObjs[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Card/" + card.ImageBack);
				PlayerCards.Add(card);
			}
			print("エネミーカード設定");
			EnemyCards = new List<CardDataFormat>();
			for (int i = 0; i < BattleDataIn.EnemyCards.Count; i++)
			{
				var id = BattleDataIn.EnemyCards[i];
				EnemyCardObjs[i].SetActive(id != -1);
				EnemyCardObjs[i].transform.FindChild("Flame").gameObject.SetActive(false);
				if (id == -1)
					continue;
				var card = mGame.CardData[id];
				EnemyCardObjs[i].GetComponent<Image>().sprite = Resources.Load<Sprite>("Textures/Card/" + card.ImageBack);
				EnemyCards.Add(card);
			}
			PlayerSummonUnits = new List<BattleUnit>();
			EnemySummonUnits = new List<BattleUnit>();

			TurnNum = Area.AreaData.Time;

			print("順番枠設定");
			OrderController = mBehaviorOrder.GetComponent<OrderController>();
			OrderController.Setup(this);
			mTurnNumDisplay.GetComponent<TurnNumDisplay>().SetUp(this);

			print("SkyBox設定");
			// SkyBox設定
			if (BattleDataIn.TimeOfDay <= 1)
			{
				if (mSkyBoxDay)
					RenderSettings.skybox = mSkyBoxDay;
			}
			else if (BattleDataIn.TimeOfDay == 2)
			{
				if (mSkyBoxSunset)
					RenderSettings.skybox = mSkyBoxSunset;
			}
			else if (BattleDataIn.TimeOfDay == 3)
			{
				if (mSkyBoxNight)
					RenderSettings.skybox = mSkyBoxNight;
			}
			mDamageDisplayPlayer.SetActive(false);
			mDamageDisplayEnemy.SetActive(false);

			// BGM再生
			mGame.SoundManager.Play(BattleDataIn.BGM, SoundType.BGM);

			StartCoroutine("CoBattleStart");
		}

		// カードの発動チェック＆発動　checkCamp: 0=両方, -1=敵, 1=味方
		private IEnumerator DoCardAction(CardDataFormat.CardTiming timeing, BattleUnit induceUnit, int checkCamp = 0)
		{
			for (int i = 0; i < 2; i++)
			{
				var isPlayer = (i == 0);
				if ((isPlayer && checkCamp >= 0) || (!isPlayer && checkCamp <= 0))
				{
					var cards = (isPlayer ? PlayerCards : EnemyCards);
					for (int j = 0; j < cards.Count; j++)
					{
						var card = cards[j];
						var cardObj = (isPlayer ? PlayerCardObjs : EnemyCardObjs)[j];
						if (card.Timing == timeing)
						{
							if ((timeing == CardDataFormat.CardTiming.Rand80 && BattleRandom.value > 0.8f) ||
								(timeing == CardDataFormat.CardTiming.Rand50 && BattleRandom.value > 0.5f) ||
								(timeing == CardDataFormat.CardTiming.Rand20 && BattleRandom.value > 0.2f))
								continue;
							// カードスキル発動
							SkillDataFormat skill = mGame.SkillData[card.SkillID];
							BattleUnit action = (isPlayer ? PlayerUnits[0] : EnemyUnits[0]);            // 行動ユニットはその陣営の先頭ユニット
							if (skill.Type == SkillDataFormat.SkillType.SoulSteal && induceUnit != null)// ソウルスティールなら行動ユニットは誘発ユニット
								action = induceUnit;

							BattleUnit target = null;
							if (skill.Range == SkillDataFormat.SkillRange.Single)                       // 対象が単体の時
							{
								if (skill.Type == SkillDataFormat.SkillType.Heal && induceUnit != null) // 回復効果なら誘発ユニット
									target = induceUnit;
								else                                                                    // それ以外ならランダム
								{
									List<BattleUnit> units = new List<BattleUnit>();
									if (skill.Target == SkillDataFormat.SkillTarget.Enemy ||
										skill.Target == SkillDataFormat.SkillTarget.EnemyLeader)        // 対象が敵の時
									{
										units = (isPlayer ? EnemyUnits : PlayerUnits);
									}
									else if (skill.Target == SkillDataFormat.SkillTarget.Player ||
										skill.Target == SkillDataFormat.SkillTarget.PlayerLeader)       // 対象が味方の時
									{
										units = (!isPlayer ? EnemyUnits : PlayerUnits);
									}
									target = units[BattleRandom.Range(0, units.Count - 1)];
								}
							}
							yield return StartCoroutine(CoCardSkillAction(card, cardObj, action, target, skill, isPlayer));
							// カード数消費(無限じゃない時)
							if (card.Duration != -1)
							{
								card.Duration--;
								if (card.Duration <= 0)
								{
									cards.Remove(card);
									// 表示処理
									cardObj.SetActive(false);
								}
							}
						}
					}
				}
			}
			yield return null;
		}

		// カード発動コルーチン
		private IEnumerator CoCardSkillAction(CardDataFormat card, GameObject cardObj, BattleUnit action, BattleUnit target, SkillDataFormat skill, bool isPlayer)
		{
			var flame = cardObj.transform.FindChild("Flame").gameObject;
			if (flame)
			{
				for (int i = 0; i < 5; i++)
				{
					flame.SetActive(!flame.activeSelf);
					yield return WaitTimer(0.25f);
				}
			}
			mCardStartUI.SetActive(true);
			yield return mCardStartUI.GetComponent<CardStartUI>().CardStart(card, cardObj);
			print("Load：カード起動エフェクト");
			if (mCardStartEffect == null)
			{
				print("エフェクトが存在しません");
			}
			else
			{
				var effect = BattleData.Instantiate(mCardStartEffect, "CardStartEffect").gameObject;
				// コールバックセット
				IsEffect = true;
				var fxCtrl = effect.GetComponent<FXController>();
				fxCtrl.EndEvent.AddListener(EndEffect);
				fxCtrl.PlaySpeed *= BattleSpeedMagni;
				fxCtrl.LifeTime /= BattleSpeedMagni;
			}
			yield return StartCoroutine("CoWaitEffect");
			mCardStartUI.SetActive(false);
			yield return StartCoroutine(CoSkillAction(action, target, skill, isPlayer, true));
			flame.SetActive(false);
			yield return StartCoroutine("CoCheckUnit");
		}

		// AI対象決定
		private BattleUnit GetEnemyTarget()
		{
			var targetUnits = (TurnUnit.IsPlayer ? EnemyUnits : PlayerUnits);
			if (targetUnits.Count == 1)             // 前衛を攻撃					
				return targetUnits[0];
			else
			{
				if (TurnUnit.LPAtk == 0)
				{
					// 魔法型
					var vl = BattleRandom.value;
					if (vl < 0.1)       // 前衛を攻撃
						return targetUnits[0];
					else
					{
						if (targetUnits.Count == 2) // 中衛を攻撃
							return targetUnits[1];
						else if (vl < 0.4)          // 中衛を攻撃
							return targetUnits[1];
						else                        // 後衛を攻撃
							return targetUnits[2];
					}
				}
				else
				{
					// 物理型
					if (BattleRandom.value <= 0.5)  // 前衛を攻撃
						return targetUnits[0];
					else                            // 中衛を攻撃
						return targetUnits[1];
				}
			}
		}

		// 数秒待つ
		public IEnumerator WaitTimer(float seconds)
		{
			yield return new WaitForSeconds(seconds / BattleSpeedMagni);
		}

		// 何か入力されるか、数秒経つまで待つ
		private IEnumerator CoWaitInputOrSeconds(float seconds)
		{
			float time = 0;
			IsWaitInputTime = true;
			while (IsWaitInputTime)
			{
				time += Time.deltaTime;
				if (time >= seconds)
					IsWaitInputTime = false;
				yield return null;
			}
		}

		public IEnumerator WaitInputOrSeconds(float seconds)
		{
			yield return StartCoroutine(CoWaitInputOrSeconds(seconds));
		}

		// 戦闘開始時のコルーチン
		private IEnumerator CoBattleStart()
		{
			mBattleStartUI.SetActive(true);
			mBattleStartUI.GetComponent<BattleStartUISetup>().Setup();
			Music.PlayVS();
			print("Load：戦闘開始エフェクト");
			if (mBattleStartEffect == null)
			{
				print("エフェクトが存在しません");
			}
			else
			{
				var effect = BattleData.Instantiate(mBattleStartEffect, "BattleStartEffect").gameObject;
				// コールバックセット
				IsEffect = true;
				var fxCtrl = effect.GetComponent<FXController>();
				//				fxCtrl.EndEvent += EndEffect;
				fxCtrl.EndEvent.AddListener(EndEffect);
				fxCtrl.PlaySpeed *= BattleSpeedMagni;
				fxCtrl.LifeTime /= BattleSpeedMagni;
			}
			// エフェクトの終わるまで待つ
			yield return StartCoroutine("CoWaitEffect");
			yield return WaitInputOrSeconds(3);
			mBattleStartUI.SetActive(false);
			// 戦闘開始時がタイミングのカード発動
			yield return DoCardAction(CardDataFormat.CardTiming.BattleBegin, null);
			// ターン処理開始
			StartCoroutine("CoTurnProcess");
		}

		// AIの行動コルーチン
		private IEnumerator CoAIAction()
		{
			yield return WaitTimer(0.5f);
			bool isAttack = BattleRandom.value < TurnUnit.AI.AttackRate;
			var lSkillData = (isAttack ? TurnUnit.LAtkSkill : TurnUnit.LDefSkill);
			BattleUnit targetUnit = null;
			if (isAttack)
			{
				// 攻撃時
				// 攻撃対象決定
				targetUnit = GetEnemyTarget();

				// 兵士ユニットがいるなら兵士ユニットで攻撃
				if (TurnUnit.IsExistSoldier)
				{
					TurnUnit.SetGroupAnimatorState(1);
					bool isSlideOut = TurnUnit.LAtkSkill.Range == SkillDataFormat.SkillRange.All && targetUnit.Position != Position.Front;
					yield return StartCoroutine(CoSoldierAttack(targetUnit, isSlideOut));
					yield return WaitTimer(0.5f);
				}
			}
			else
			{
				// 防御時
				TurnUnit.SetGroupAnimatorState(2);
				TurnUnit.IsDefense = true;
				// スキル対象決定
				if (lSkillData.Range == SkillDataFormat.SkillRange.Single)
				{
					if (lSkillData.Target == SkillDataFormat.SkillTarget.Enemy || lSkillData.Target == SkillDataFormat.SkillTarget.EnemyLeader)
					{
						var targetUnits = (TurnUnit.IsPlayer ? EnemyUnits : PlayerUnits);
						if (lSkillData.Type == SkillDataFormat.SkillType.Damage)
							targetUnit = GetEnemyTarget();
						else
							targetUnit = targetUnits[BattleRandom.Range(0, targetUnits.Count - 1)];

					}
					else if (lSkillData.Target == SkillDataFormat.SkillTarget.Player || lSkillData.Target == SkillDataFormat.SkillTarget.PlayerLeader)
					{
						var targetUnits = (TurnUnit.IsPlayer ? PlayerUnits : EnemyUnits);
						if (lSkillData.Type == SkillDataFormat.SkillType.Heal)
						{
							// 回復の場合
							float minRate = 1.0f;
							int id = 0;
							for (int i = 0; i < targetUnits.Count; ++i)
							{
								var uData = targetUnits[i].UnitData;
								float rate = (lSkillData.Target == SkillDataFormat.SkillTarget.Player ?
									(float)uData.SoldierNum / uData.MaxSoldierNum : (float)uData.HP / uData.MaxHP);
								if (rate < minRate)
								{
									minRate = rate;
									id = i;
								}
							}
							targetUnit = targetUnits[id];
						}
						else
							targetUnit = targetUnits[BattleRandom.Range(0, targetUnits.Count - 1)];
					}
					else if (lSkillData.Target == SkillDataFormat.SkillTarget.Own)
						targetUnit = TurnUnit;
					else if (lSkillData.Target == SkillDataFormat.SkillTarget.EnemyAndPlayer)
						targetUnit = null;
				}
				else
					targetUnit = null;
			}

			if (!IsBattleEnd)
			{
				// リーダースキル発動
				TurnUnit.SetLeaderAnimatorState(isAttack ? 1 : 2);
				yield return StartCoroutine(CoSkillAction(TurnUnit, targetUnit, lSkillData, TurnUnit.IsPlayer, false));
			}
			TurnUnit.SetLeaderAnimatorState(0);
			TurnUnit.SetGroupAnimatorState(0);
			yield return WaitTimer(0.5f);
		}

		// エフェクトを発生させる
		private void GenerateEffect(string effectPath, bool isPlayer)
		{
			// 生成
			print("Load：エフェクト　" + effectPath);
			var obj = (GameObject)Resources.Load("Effects/Battle/Prefabs/" + effectPath);
			if (obj == null)
			{
				print("エフェクトが存在しません");
				return;
			}
			var effect = BattleData.Instantiate(obj, effectPath).gameObject;
			effect.transform.localScale = new Vector3((isPlayer ? -1 : 1), 1, 1);
			// コールバックセット
			IsEffect = true;
			FXCtrl = effect.GetComponent<FXController>();
			//				fxCtrl.EndEvent += EndEffect;
			FXCtrl.EndEvent.AddListener(EndEffect);
			FXCtrl.PlaySpeed *= BattleSpeedMagni;
			FXCtrl.LifeTime /= BattleSpeedMagni;
		}

		// エフェクトを待つ
		private IEnumerator CoWaitEffect()
		{
			while (IsEffect)
				yield return null;
		}

		// ダメージを与えた時のコルーチン
		private IEnumerator DamageProcess(BattleUnit target, float phyDamage, float magDamage, DamageType type, bool toLeader = false, BattleUnit originTarget = null)
		{
			// ターゲットをスライドインさせる、スライドしている間待機
			yield return target.SlideIn();
			if (type == DamageType.Normal)
				target.Face.SetSelectFlame(true);
			yield return WaitTimer(0.2f);
			EndTargetUnit = null;

			// ターゲットが庇われていた場合
			if (type == DamageType.Normal)
			{
				foreach (var unit in (target.IsPlayer ? PlayerUnits : EnemyUnits))
				{
					if (unit != target && unit != originTarget && unit.GuardTarget == target)
					{
						// ターゲットを戻す
						yield return target.SlideOut();
						if (type == DamageType.Normal)
							target.Face.SetSelectFlame(false);
						yield return StartCoroutine(DamageProcess(unit, phyDamage, magDamage, type, toLeader, (originTarget == null ? target : originTarget)));
						yield return unit.SlideOut();
						yield return target.SlideIn();
						yield break;
					}
				}
			}

			IsNowIncDec = true;
			EndTargetUnit = target;
			DamageNum = 0;
			yield return null;
			if (target.IsStateNoDamage)
			{
				// ダメージを受けない状態であるならば
				while (FXCtrl.LifeTime >= 0.5)
					yield return null;
				target.IsStateNoDamage = false;
			}
			else
			{
				if (false) { }
				else if (type == DamageType.Normal)
					DamageNum = target.GetNormalDamage(phyDamage, magDamage, toLeader);
				else if (type == DamageType.Counter)
					DamageNum = target.GetCounterDamage(phyDamage, 0);
				else if (type == DamageType.CaptureCounter)
					DamageNum = target.GetCaptureCounterDamage(phyDamage, 0);
				else if (type == DamageType.Poison)
					DamageNum = target.GetPoisonDamagePerTurn();
				DamageNum = System.Math.Min(DamageNum, (!target.IsExistSoldier || toLeader ? target.UnitData.HP : target.UnitData.SoldierNum));
				target.SufferDamage(DamageNum, toLeader);
				var damageDisplay = (target.IsPlayer ? mDamageDisplayPlayer : mDamageDisplayEnemy).GetComponent<DamageDisplay>();
				damageDisplay.Display(DamageNum, true);
				while (FXCtrl.LifeTime >= 0.5)
					yield return null;
				for (int i = 5; i >= 0; --i)
				{
					target.ApproachDisplay(i);
					target.SetDisplaySoldier();
					yield return WaitTimer(0.2f);
				}
				yield return WaitTimer(0.2f);
				damageDisplay.Hide();
			}

			// 庇い対象を無しにする
			target.GuardTarget = null;
			if (type == DamageType.Normal)
				target.Face.SetSelectFlame(false);

			IsNowIncDec = false;
		}

		// 回復した時のコルーチン
		private IEnumerator HealProcess(BattleUnit target, float healHP, float healSolNum)
		{
			IsNowIncDec = true;
			// ターゲットをスライドインさせる、スライドしている間待機
			yield return target.SlideIn();
			target.Face.SetSelectFlame(true);
			while (FXCtrl.LifeTime >= 0.5)
				yield return null;

			var damageDisplay = (target.IsPlayer ? mDamageDisplayPlayer : mDamageDisplayEnemy).GetComponent<DamageDisplay>();
			var realHealHP = System.Math.Min(healHP, System.Math.Max(target.UnitData.MaxHP - target.UnitData.HP, 0));
			var realHealSolNum = System.Math.Min(healSolNum, System.Math.Max(target.UnitData.MaxSoldierNum - target.UnitData.SoldierNum, 0));
			float displayHP = System.Math.Max(realHealHP, realHealSolNum);
			damageDisplay.Display(displayHP, false);
			target.Healed(healHP, healSolNum);
			for (int i = 5; i >= 0; --i)
			{
				target.ApproachDisplay(i);
				target.SetDisplaySoldier();
				yield return WaitTimer(0.2f);
			}
			yield return WaitTimer(0.2f);

			damageDisplay.Hide();
			target.Face.SetSelectFlame(false);
			IsNowIncDec = false;
		}

		// ユニット状態確認コルーチン
		private IEnumerator CoCheckUnit()
		{
			List<BattleUnit> removeUnits = new List<BattleUnit>();
			for (int i = 0; i < 2; i++)
			{
				int removeCount = 0;
				List<bool> removeFlags = new List<bool>();
				var units = (i == 0 ? PlayerUnits : EnemyUnits);
				foreach (var unit in units)
				{
					bool isS50 = unit.UnitData.SoldierNum <= unit.UnitData.MaxSoldierNum * 0.5;
					bool isS10 = unit.UnitData.SoldierNum <= unit.UnitData.MaxSoldierNum * 0.1;
					bool isHP50 = unit.UnitData.HP <= unit.MaxHP * 0.5;
					bool isHP10 = unit.UnitData.HP <= unit.MaxHP * 0.1;
					bool isDeath = unit.UnitData.HP == 0;
					// 兵士数が50％以下
					if (isS50)
					{
						yield return DoCardAction(CardDataFormat.CardTiming.UserState_S50, unit, (unit.IsPlayer ? 1 : -1));
						yield return DoCardAction(CardDataFormat.CardTiming.EnemyState_S50, unit, (unit.IsPlayer ? -1 : 1));
					}
					// 兵士数が10％以下
					if (isS10)
					{
						yield return DoCardAction(CardDataFormat.CardTiming.UserState_S10, unit, (unit.IsPlayer ? 1 : -1));
						yield return DoCardAction(CardDataFormat.CardTiming.EnemyState_S10, unit, (unit.IsPlayer ? -1 : 1));
					}
					// HPが50％以下
					if (isHP50)
					{
						yield return DoCardAction(CardDataFormat.CardTiming.UserState_HP50, unit, (unit.IsPlayer ? 1 : -1));
						yield return DoCardAction(CardDataFormat.CardTiming.EnemyState_HP50, unit, (unit.IsPlayer ? -1 : 1));
					}
					// HPが10％以下
					if (isHP10)
					{
						yield return DoCardAction(CardDataFormat.CardTiming.UserState_HP10, unit, (unit.IsPlayer ? 1 : -1));
						yield return DoCardAction(CardDataFormat.CardTiming.EnemyState_HP10, unit, (unit.IsPlayer ? -1 : 1));
					}
					// 死亡
					if (isDeath)
					{
						yield return DoCardAction(CardDataFormat.CardTiming.UserState_Death, unit, (unit.IsPlayer ? 1 : -1));
						yield return DoCardAction(CardDataFormat.CardTiming.EnemyState_Death, unit, (unit.IsPlayer ? -1 : 1));
					}

					removeFlags.Add(unit.UnitData.HP == 0 || unit.IsCapture);
					if (unit.UnitData.HP == 0)
					{
						// 死亡確認
						if (unit.UnitData.Deathable)
						{
							// 死ぬユニットなら顔グラを赤く
							unit.Face.SetDead();
							BattleDataOut.DeadUnits.Add(unit.UnitID);
							yield return StartCoroutine(CoDeleteUnit(DeleteUnitType.Dead, unit));
						}
						else
						{
							// 撤退ユニットなら顔グラを赤く
							unit.Face.SetRetreat();
							BattleDataOut.EscapedUnits.Add(unit.UnitID);
							yield return StartCoroutine(CoDeleteUnit(DeleteUnitType.Escape, unit));
						}
						removeUnits.Add(unit);
						++removeCount;
					}
					else if (unit.IsCapture)
					{
						// 捕獲確認
						// 顔グラを青く
						unit.Face.SetCapture();
						BattleDataOut.CapturedUnits.Add(unit.UnitID);
						removeUnits.Add(unit);
						++removeCount;
						yield return StartCoroutine(CoDeleteUnit(DeleteUnitType.Capture, unit));
					}
				}
				if (removeCount != 0)
				{
					if (units.Count == 3 && removeCount != 3)
					{
						if (removeCount == 1)
						{
							if (removeFlags[0])
							{
								for (int j = 0; j < 3; j++)
									units[j].Face.MovePos((j + 2) % 3);
							}
							else if (removeFlags[1])
							{
								units[1].Face.MovePos(2);
								units[2].Face.MovePos(1);
							}
						}
						else
						{
							if (!removeFlags[2])
							{
								for (int j = 0; j < 3; j++)
									units[j].Face.MovePos((j + 1) % 3);
							}
							else if (!removeFlags[1])
							{
								units[0].Face.MovePos(1);
								units[1].Face.MovePos(0);
							}
						}
					}
					else if (units.Count == 2 && removeCount == 1 && removeFlags[0])
					{
						units[0].Face.MovePos(1);
						units[1].Face.MovePos(0);
					}
				}
			}
			yield return OrderController.DeadOut(removeUnits);

			// 顔オブジェクトが移動している間待機
			while (true)
			{
				for (int i = 0; i < 2; i++)
				{
					foreach (var unit in (i == 0 ? PlayerUnits : EnemyUnits))
						if (unit.Face.IsMoving)
							yield return null;
				}
				break;
			}

			// Unitを削除
			foreach (var unit in removeUnits)
				(unit.IsPlayer ? PlayerUnits : EnemyUnits).Remove(unit);
			// どちらかのユニット数が０になっていれば戦闘終了
			if (PlayerUnits.Count == 0 || EnemyUnits.Count == 0)
				IsBattleEnd = true;
		}

		private enum DeleteUnitType : int
		{
			Dead = 0,
			Capture,
			Escape,
		}

		// ユニット消去演出コルーチン
		private IEnumerator CoDeleteUnit(DeleteUnitType type, BattleUnit unit)
		{
			yield return unit.SlideIn();
			mLastSerifUI.SetActive(true);
			mMessageUI.SetActive(false);
			var imSerif = mLastSerifUI.transform.FindChild("Image");
			var imMessage = mMessageUI.transform.FindChild("Image");
			var text = mLastSerifUI.transform.FindChild("Text").GetComponent<Text>();
			imSerif.localScale = new Vector3((unit.IsPlayer ? 1 : -1), 1, 1);
			// 固有メッセージ表示
			if (type == DeleteUnitType.Dead)
				text.text = unit.UnitData.OnDeadSerif;
			else if (type == DeleteUnitType.Capture)
				text.text = unit.UnitData.OnCapturedSerif;
			else if (type == DeleteUnitType.Escape)
				text.text = unit.UnitData.OnEscapedSerif;
			yield return WaitInputOrSeconds(2);
			mLastSerifUI.SetActive(false);
			mMessageUI.SetActive(true);
			text = mMessageUI.transform.FindChild("Text").GetComponent<Text>();
			// メッセージ表示
			if (type == DeleteUnitType.Dead)
				text.text = unit.UnitData.Name + "は戦場に散った";
			else if (type == DeleteUnitType.Capture)
				text.text = unit.UnitData.Name + "を捕獲した";
			else if (type == DeleteUnitType.Escape)
				text.text = unit.UnitData.Name + "は戦場を離脱した";
			yield return WaitInputOrSeconds(2);
			mMessageUI.SetActive(false);
			if (type == DeleteUnitType.Escape)
				yield return unit.SlideOut();
			else
				unit.SetDisplayState(false);
			yield return null;
		}

		// 兵士ユニットで攻撃するコルーチン
		private IEnumerator CoSoldierAttack(BattleUnit target, bool isSlideOut)
		{
			var gSkillData = TurnUnit.GAtkSkill;
			// エフェクト発生
			GenerateEffect(gSkillData.EffectPath, target.IsPlayer);
			// ダメージ処理
			StartCoroutine(DamageProcess(target, TurnUnit.GetGroupPhyDamage(), TurnUnit.GetGroupMagDamage(), DamageType.Normal));
			while (!IsNowIncDec)
				yield return null;
			// カウンターダメージ
			if (!TurnUnit.IsSummonUnit && TurnUnit.Position == Position.Front)
				StartCoroutine(DamageProcess(TurnUnit, (EndTargetUnit.IsExistSoldier ? EndTargetUnit.GetGroupPhyDamage() : EndTargetUnit.GetLeaderPhyDamage()), 0, DamageType.Counter));
			// エフェクト発生中は待つ
			yield return StartCoroutine("CoWaitEffect");
			while (IsNowIncDec)
				yield return null;
			yield return WaitTimer(0.2f);
			// ターゲットをスライドアウトさせる、スライドしている間待機
			if (isSlideOut || target.UnitData.HP == 0)
				yield return target.SlideOut();
			yield return StartCoroutine("CoCheckUnit");
		}

		// スキルの行動コルーチン
		private IEnumerator CoSkillAction(BattleUnit actionUnit, BattleUnit target, SkillDataFormat skill, bool isPlayer, bool isCard)
		{
			// ターゲットユニット設定
			List<BattleUnit> targetsList = new List<BattleUnit>();
			if (target != null && skill.Range == SkillDataFormat.SkillRange.Single)
				targetsList.Add(target);
			else
			{
				if (skill.Target == SkillDataFormat.SkillTarget.Enemy ||
					skill.Target == SkillDataFormat.SkillTarget.EnemyLeader ||
					skill.Target == SkillDataFormat.SkillTarget.EnemyAndPlayer)
				{
					foreach (var unit in (isPlayer ? EnemyUnits : PlayerUnits))
						targetsList.Add(unit);
				}
				if (skill.Target == SkillDataFormat.SkillTarget.Player ||
					skill.Target == SkillDataFormat.SkillTarget.PlayerLeader ||
					skill.Target == SkillDataFormat.SkillTarget.EnemyAndPlayer)
				{
					foreach (var unit in (isPlayer ? PlayerUnits : EnemyUnits))
						targetsList.Add(unit);
				}
			}

			if (skill.Type == SkillDataFormat.SkillType.Summon)
			{
				//4:召喚
				Transform go = BattleData.Instantiate(mUnitPrefab, "", (isPlayer ? mTfPlayer : mTfEnemy));
				var bu = go.GetComponent<BattleUnit>();
				bu.SetupSummon(skill, isPlayer, this);
				(isPlayer ? PlayerSummonUnits : EnemySummonUnits).Add(bu);
				if (!isCard)
					yield return actionUnit.SlideOut();
				bu.SetDisplayState(false);
				// 召喚エフェクト?
				yield return bu.SlideIn();
				bu.SetDisplayState(true);
				yield return WaitTimer(0.5f);
				// Orderに追加
				yield return OrderController.PlusUnit(bu);
				yield return bu.SlideOut();
			}
			else if (skill.Type == SkillDataFormat.SkillType.TurnWait)
			{
				//7:順番下げ
				if (skill.Range == SkillDataFormat.SkillRange.Single)
					yield return OrderController.DownOrderSingle(target);
				else
					yield return OrderController.DownOrderAll(!isPlayer);
			}
			else if (skill.Type == SkillDataFormat.SkillType.PutTime)
			{
				//9:時間消費
				TurnNum -= skill.Power;
				if (TurnNum < 0)
					TurnNum = 0;
			}
			else
			{
				// ランダム効果の場合のダメージかどうか
				bool isRandomDamage = (BattleRandom.value <= 0.5);
				BattleUnit preUnit = null;
				foreach (var unit in targetsList)
				{
					// ターンユニットと同じ陣営のユニットをスライドインさせる前に、ターンユニットをスライドアウトさせる
					if (!isCard && actionUnit.IsDisplay && actionUnit != unit && isPlayer == unit.IsPlayer)
						yield return actionUnit.SlideOut();
					// 以前のターゲットユニットをスライドアウトさせる
					if (preUnit != null)
					{
						yield return preUnit.SlideOut();
						preUnit.Face.SetSelectFlame(false);
					}
					preUnit = unit;
					// ターゲットユニットをスライドインさせる
					yield return unit.SlideIn();
					unit.Face.SetSelectFlame(true);
					// エフェクト発生
					GenerateEffect(skill.EffectPath, unit.IsPlayer);
					// 与えるダメージ
					float phyDamage = 0;
					float magDamage = 0;
					if (isCard)
					{
						phyDamage = UnitAlice.GetLeaderPhyDamage();
						magDamage = UnitAlice.GetLeaderMagDamage();
					}
					else
					{
						phyDamage = actionUnit.GetLeaderPhyDamage();
						magDamage = actionUnit.GetLeaderMagDamage();
					}
					// 処理
					if (skill.Type == SkillDataFormat.SkillType.Damage)
					{
						//0:ダメ―ジ
						// ダメージ処理
						StartCoroutine(DamageProcess(unit, phyDamage, magDamage, DamageType.Normal,
							skill.Target == SkillDataFormat.SkillTarget.EnemyLeader || skill.Target == SkillDataFormat.SkillTarget.PlayerLeader));
						while (!IsNowIncDec)
							yield return null;
						// カウンターダメージ
						if (!isCard && !actionUnit.IsSummonUnit && actionUnit.Position == Position.Front)
							StartCoroutine(DamageProcess(actionUnit, (EndTargetUnit.IsExistSoldier ? EndTargetUnit.GetGroupPhyDamage() : EndTargetUnit.GetLeaderPhyDamage()), 0,
								DamageType.Counter));

						// 毒を与える効果
						if (skill.Attribute[0] && BattleRandom.value <= 0.5)
						{
							unit.IsStatePoisom = true;
							// エフェクト？

							yield return DoCardAction(CardDataFormat.CardTiming.UserState_Poison, unit, (unit.IsPlayer ? 1 : -1));
							yield return DoCardAction(CardDataFormat.CardTiming.EnemyState_Poison, unit, (unit.IsPlayer ? -1 : 1));
						}
					}
					else if (skill.Type == SkillDataFormat.SkillType.Heal)
					{
						//1:回復
						// 回復処理					
						if (skill.Attribute[0])         // 毒を回復させるなら
							unit.IsStatePoisom = false;
						else                            // HPを回復させる
							StartCoroutine(HealProcess(unit, unit.GetLeaderCurativeAmount(skill), unit.GetGroupCurativeAmount(skill)));
					}
					else if (skill.Type == SkillDataFormat.SkillType.StatusUp || skill.Type == SkillDataFormat.SkillType.StatusDown)
					{
						//2:ステ上昇 or 3:ステ下降
						unit.SufferStatus(skill);
					}
					else if (!isCard && skill.Type == SkillDataFormat.SkillType.SoulSteal)
					{
						//5:ダメージ還元
						// ダメージ処理
						StartCoroutine(DamageProcess(unit, phyDamage, magDamage, DamageType.Normal,
							skill.Target == SkillDataFormat.SkillTarget.EnemyLeader || skill.Target == SkillDataFormat.SkillTarget.PlayerLeader));
						while (!IsNowIncDec)
							yield return null;
						// カウンターダメージ
						if (!actionUnit.IsSummonUnit && actionUnit.Position == Position.Front)
							StartCoroutine(DamageProcess(actionUnit, (EndTargetUnit.IsExistSoldier ? EndTargetUnit.GetGroupPhyDamage() : EndTargetUnit.GetLeaderPhyDamage()), 0,
								DamageType.Counter));
						while (IsNowIncDec)
							yield return null;
						if (DamageNum != 0)
						{
							// 回復
							StartCoroutine(HealProcess(actionUnit, DamageNum, 0));
						}
					}
					else if (!isCard && skill.Type == SkillDataFormat.SkillType.Guard)
					{
						//6:ガード
						actionUnit.GuardTarget = unit;
					}
					else if (skill.Type == SkillDataFormat.SkillType.NoDamage)
					{
						//8:ダメージ無効
						unit.IsStateNoDamage = true;
					}
					else if (skill.Type == SkillDataFormat.SkillType.StatusOff)
					{
						//10:ステータス取り消し
						unit.Status.Clear();
					}
					else if (skill.Type == SkillDataFormat.SkillType.Random)
					{
						//11:ランダム
						if (isRandomDamage)
						{
							// ダメージ
							StartCoroutine(DamageProcess(unit, phyDamage, magDamage, DamageType.Normal,
							skill.Target == SkillDataFormat.SkillTarget.EnemyLeader || skill.Target == SkillDataFormat.SkillTarget.PlayerLeader));
							while (!IsNowIncDec)
								yield return null;
							// カウンターダメージ
							if (!isCard && !actionUnit.IsSummonUnit && actionUnit.Position == Position.Front && actionUnit.IsPlayer != unit.IsPlayer)
								StartCoroutine(DamageProcess(actionUnit, (EndTargetUnit.IsExistSoldier ? EndTargetUnit.GetGroupPhyDamage() : EndTargetUnit.GetLeaderPhyDamage()), 0,
									DamageType.Counter));
						}
						else
						{
							// 回復
							StartCoroutine(HealProcess(unit, unit.GetLeaderCurativeAmount(skill), unit.GetGroupCurativeAmount(skill)));
						}
					}
					// エフェクト発生中は待つ
					yield return StartCoroutine("CoWaitEffect");
					while (IsNowIncDec)
						yield return null;
					yield return WaitTimer(0.5f);
				}
				// 以前のターゲットユニットをスライドアウトさせる
				if (preUnit)
				{
					if (preUnit != actionUnit || (isCard && preUnit != TurnUnit))
						yield return preUnit.SlideOut();
					preUnit.Face.SetSelectFlame(false);
				}
			}
			// ターンユニットがスライドアウトしている場合、スライドインさせる
			if (!isCard && !TurnUnit.IsDisplay)
				yield return TurnUnit.SlideIn();
		}

		// 捕獲するコルーチン
		private IEnumerator CoCapture(BattleUnit target, BattleUnit originTarget = null)
		{
			// ターゲットをスライドインさせる、スライドしている間待機
			yield return target.SlideIn();
			yield return WaitTimer(0.5f);

			// ターゲットが庇われていた場合
			foreach (var unit in (target.IsPlayer ? PlayerUnits : EnemyUnits))
			{
				if (unit != target && unit != originTarget && unit.GuardTarget == target && unit.UnitData.Deathable)
				{
					// ターゲットを戻す
					yield return target.SlideOut();
					yield return StartCoroutine(CoCapture(unit, (originTarget == null ? target : originTarget)));
					yield break;
				}
			}

			// エフェクト
			//			StartCoroutine("CoDoEffect", TurnUnit.GAtkSkill.EffectPath);

			// 捕獲カウンターダメージ
			if (!TurnUnit.IsSummonUnit && TurnUnit.Position == Position.Front)
				StartCoroutine(DamageProcess(TurnUnit, target.GetLeaderPhyDamage(), 0, DamageType.CaptureCounter));

			if (target.IsStateNoDamage)
			{
				// ダメージを受けない状態であるならば
				target.IsStateNoDamage = false;
			}
			else
			{
				// 捕獲ダメージ処理
				var preCaptureGauge = target.CaptureGauge;
				mCaptureGauge.SetActive(true);
				CaptureGauge.fillAmount = preCaptureGauge / 100;
				target.SufferCaptureDamage(target.GetCaptureDamage(TurnUnit.IsExistSoldier ? TurnUnit.GetGroupPhyDamage() : TurnUnit.GetLeaderPhyDamage(),
					TurnUnit.IsExistSoldier ? TurnUnit.GetGroupMagDamage() : TurnUnit.GetLeaderMagDamage()));
				var length = 200 / BattleSpeedMagni;
				var parCaptureGauge = (target.CaptureGauge - preCaptureGauge) / length;
				var text = mCaptureGauge.transform.FindChild("Text").GetComponent<Text>();
				for (int i = 0; i < length; i++)
				{
					CaptureGauge.fillAmount = (preCaptureGauge + parCaptureGauge * i) / 100;
					text.text = (int)(preCaptureGauge + parCaptureGauge * i) + "％";
					yield return WaitTimer(0.2f / length);
				}
				text.text = (int)target.CaptureGauge + "％";
				CaptureGauge.fillAmount = target.CaptureGauge / 100;
				yield return WaitTimer(0.2f);
			}

			// エフェクト発生中は待つ
			yield return StartCoroutine("CoWaitEffect");
			while (IsNowIncDec)
				yield return null;
			yield return WaitTimer(0.2f);
			mCaptureGauge.SetActive(false);

			// ターゲットをスライドアウトさせる、スライドしている間待機
			yield return target.SlideOut();
		}

		// ターン処理のコルーチン
		private IEnumerator CoTurnProcess()
		{
			IsPlayerSelectTime = false;
			IsPlayerActionTime = false;
			while (OrderController.IsAnimation)
				yield return null;
			var preTurnUnit = TurnUnit;
			TurnUnit.IsDefense = false;
			yield return TurnUnit.SlideIn();
			if (!TurnUnit.IsSummonUnit)
				TurnUnit.Face.SetActionFlame(true);

			// 確率発動
			yield return DoCardAction(CardDataFormat.CardTiming.Rand80, TurnUnit, (TurnUnit.IsPlayer ? 1 : -1));
			yield return DoCardAction(CardDataFormat.CardTiming.Rand50, TurnUnit, (TurnUnit.IsPlayer ? 1 : -1));
			yield return DoCardAction(CardDataFormat.CardTiming.Rand20, TurnUnit, (TurnUnit.IsPlayer ? 1 : -1));

			// 敵、または自動戦闘の場合
			if (!TurnUnit.IsPlayer || BattleDataIn.IsAuto)
				yield return StartCoroutine("CoAIAction");
			else
			{
				// 味方キャラの場合
				IsPlayerSelectTime = true;
				mMouseCover.SetActive(true);
				while ((IsPlayerSelectTime || IsPlayerActionTime) && !IsBattleEnd)
					yield return null;
			}

			// ターン終了
			yield return StartCoroutine("CoCheckUnit");

			if (!preTurnUnit.IsSummonUnit)
			{
				preTurnUnit.Face.SetActionFlame(false);
			}
			if (preTurnUnit.IsSummonUnit || preTurnUnit.Face.IsExsistUnit)
			{
				if (preTurnUnit.IsStatePoisom)
				{
					// 毒ダメージエフェクト
					StartCoroutine("CoDoEffect", "毒ダメージエフェクトパス名");
					// 毒状態なら毒ダメージを受ける
					yield return StartCoroutine(DamageProcess(preTurnUnit, 0, 0, DamageType.Poison));
					yield return WaitTimer(0.2f);
				}
				bool isSummoneUnit = preTurnUnit.IsSummonUnit;
				yield return preTurnUnit.TurnEnd();

				if (!IsBattleEnd && (!isSummoneUnit || !preTurnUnit.IsReturnSummonUnit))
					yield return OrderController.TurnEnd();

			}

			yield return preTurnUnit.SlideOut();

			if (TurnNum > 0)
				--TurnNum;
			if (TurnNum == 0)
				IsBattleEnd = true;
			if (IsEscape)
				IsBattleEnd = true;

			if (IsBattleEnd)
				// 戦闘終了
				StartCoroutine("CoBattleEnd");
			else
			{
				// 次のターン
				Music.PlayNextTurn();
				StartCoroutine("CoTurnProcess");
			}
		}

		// 戦闘終了のコルーチン
		private IEnumerator CoBattleEnd()
		{
			yield return DoCardAction(CardDataFormat.CardTiming.BattleEnd, null);
			bool isWin;
			if ((TurnNum == 0) || (PlayerUnits.Count == 0 && EnemyUnits.Count == 0))
				// 時間切れなら侵攻側が負け または 全滅引き分けなら侵攻側が負け
				isWin = !IsInvasion;
			else if (IsEscape)
				// 撤退すると敗北
				isWin = false;
			else
				// 敵が全滅しているなら勝ち、 プレイヤーが全滅しているなら負け
				isWin = (EnemyUnits.Count == 0);

			if (isWin)
			{
				Music.PlayWin();
				mBattleWinUI.SetActive(true);
				print("WIN");
				print("Load：戦闘勝利エフェクト");
				if (mBattleWinEffect == null)
				{
					print("エフェクトが存在しません");
				}
				else
				{
					var effect = BattleData.Instantiate(mBattleWinEffect, "BattleWinEffect").gameObject;
					// コールバックセット
					IsEffect = true;
					var fxCtrl = effect.GetComponent<FXController>();
					//				fxCtrl.EndEvent += EndEffect;
					fxCtrl.EndEvent.AddListener(EndEffect);
					fxCtrl.PlaySpeed *= BattleSpeedMagni;
					fxCtrl.LifeTime /= BattleSpeedMagni;
				}
			}
			else
			{
				Music.PlayLose();
				mBattleLoseUI.SetActive(true);
				print("LOSE");
				print("Load：戦闘敗北エフェクト");
				if (mBattleLoseEffect == null)
				{
					print("エフェクトが存在しません");
				}
				else
				{
					var effect = BattleData.Instantiate(mBattleLoseEffect, "BattleLoseEffect").gameObject;
					// コールバックセット
					IsEffect = true;
					var fxCtrl = effect.GetComponent<FXController>();
					//				fxCtrl.EndEvent += EndEffect;
					fxCtrl.EndEvent.AddListener(EndEffect);
					fxCtrl.PlaySpeed *= BattleSpeedMagni;
					fxCtrl.LifeTime /= BattleSpeedMagni;
				}
			}
			BattleDataOut.IsWin = isWin;

			yield return WaitInputOrSeconds(2);
			mMessageUI.SetActive(true);
			mMessageUI.transform.FindChild("Text").GetComponent<Text>().text = (IsInvasion ? 2 : 1) + (isWin ? 1 : 0) + "ポイントの経験値を得た";
			yield return WaitInputOrSeconds(2);
			mMessageUI.SetActive(false);

			IsEndWaitTime = true;
			yield return null;
		}

		// プレイヤー行動のコルーチン
		private IEnumerator CoPlayerAction(BattleUnit target, int type)
		{
			IsPlayerActionTime = true;
			IsPlayerSelectTime = false;
			if (type == 0)
			{
				// 攻撃
				// 兵士での攻撃
				if (TurnUnit.IsExistSoldier)
				{
					TurnUnit.SetGroupAnimatorState(1);
					bool isSlideOut = TurnUnit.LAtkSkill.Range == SkillDataFormat.SkillRange.All && target.Position != Position.Front;
					yield return StartCoroutine(CoSoldierAttack(target, isSlideOut));
					yield return WaitTimer(0.5f);
				}
				if (!IsBattleEnd)
				{
					// リーダースキル発動
					TurnUnit.SetLeaderAnimatorState(1);
					yield return StartCoroutine(CoSkillAction(TurnUnit, target, TurnUnit.LAtkSkill, TurnUnit.IsPlayer, false));
				}
			}
			else if (type == 1)
			{
				//　防御
				TurnUnit.SetGroupAnimatorState(2);
				TurnUnit.IsDefense = true;
				// リーダースキル発動
				TurnUnit.SetLeaderAnimatorState(2);
				yield return StartCoroutine(CoSkillAction(TurnUnit, target, TurnUnit.LDefSkill, TurnUnit.IsPlayer, false));
			}
			else
			{
				// 捕獲
				TurnUnit.SetGroupAnimatorState(1);
				TurnUnit.SetLeaderAnimatorState(1);
				yield return StartCoroutine(CoCapture(target));
			}
			TurnUnit.SetLeaderAnimatorState(0);
			TurnUnit.SetGroupAnimatorState(0);
			yield return WaitTimer(0.5f);
			IsPlayerActionTime = false;
			yield return null;
		}

		// エフェクトが終わったことを外部から通知する関数
		public void EndEffect()
		{
			IsEffect = false;
		}

		// スキル選択ボタンが押されたことを通知する関数
		public void PushSkillButton(BattleUnit target, int type)
		{
			// スキルボタンを消す
			PlayerUnits[0].Face.SetAllFaceHide();
			Music.PlayClickSkillButton();
			target.Face.SetSelectFlame(false);
			StartCoroutine(CoPlayerAction(target, type));
		}

		// 撤退ボタンを押したときの関数
		public void PushEscape()
		{
			if (!IsPlayerSelectTime)
				return;
			mEscapeConfUI.SetActive(true);
		}

		// 撤退確認ボタン
		public void PushConfEscape(bool flag)
		{
			mEscapeConfUI.SetActive(false);
			if (flag)
			{
				IsEscape = true;
				IsPlayerSelectTime = false;
			}
		}

		// 設定ボタンを押したときの関数
		public void PushConfig()
		{
			mConfigUI.SetActive(true);
			var slider = mConfigUI.transform.FindChild("Slider").GetComponent<Slider>();
			slider.value = BattleSpeed;
		}

		// 戦闘スピードスライダーを変化させたときの関数
		public void ChangeBattleSpeed(float val)
		{
			BattleSpeed = (int)val;
			Music.PlayMoveConfigSlider();
		}

		// 設定画面を閉じる関数
		public void PushCloseConfig()
		{
			mConfigUI.SetActive(false);
		}

		// Use this for initialization
		void Start()
		{
			LoadData();
		}

		// Update is called once per frame
		void Update()
		{
			if (IsEndWaitTime && Input.GetMouseButtonDown(0))
			{
				if (EndEvent != null)
					EndEvent.Invoke();
				print("戦闘終了");
			}
			if (IsWaitInputTime && Input.GetMouseButtonDown(0))
			{
				IsWaitInputTime = false;
			}
		}
	}

}
