//author	: masanori.k
//version	: 0.1.0
//summary	: 戦闘全体を制御するコンポーネント

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace ProjectWitch.Battle
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
		#region インスペクタ
		// 捕獲ゲージの速度
		[SerializeField]
		private float mCaptureGaugeTime = 2f;
		// 矢印の振れ幅、かかる時間
		[SerializeField]
		private float mArrowShakeWidth = 40f, mArrowTime = 0.5f;
		public float ArrowShakeWidth { get { return mArrowShakeWidth; } }
		public float ArrowTime { get { return mArrowTime; } }
		// 操作説明文
		[SerializeField]
		private string mDescriptionText = "";
		// 操作説明文のスライド時間
		[SerializeField]
		private float mDescriptionTextSlideTime = 10f;
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
		private FaceObj mFaceP0 = null, mFaceP1 = null, mFaceP2 = null, mFaceE0 = null, mFaceE1 = null, mFaceE2 = null;
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
		// 捕獲ゲージ背面
		[SerializeField]
		private GameObject mCaptureGaugeBack = null;
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
		// エフェクトUIの親オブジェクト
		[SerializeField]
		private GameObject mUIEffectParent = null;
		// バー関連
		[SerializeField]
		private GameObject mBar = null;
		// スキル説明文
		[SerializeField]
		private GameObject mDescription = null;
		// スキル説明文マスク
		[SerializeField]
		private RectTransform mMaskDescription = null;
		// スキルキャプション
		[SerializeField]
		private GameObject mSkillCaption = null;
		// スキルキャプションスプライト
		[SerializeField]
		private Sprite mSC_Atk = null, mSC_Def = null;
		// 音楽再生
		[SerializeField]
		private PlayMusic mMusic = null;
		// DamageDisplay
		[SerializeField]
		private GameObject mDamageDisplayPlayer = null, mDamageDisplayEnemy = null;
		// 撤退ボタン
		[SerializeField]
		private Button mEscapeButton = null;
		// 最終呼び出しevent
		[SerializeField]
		public UnityEvent EndEvent = null;

		#endregion

		#region 参照データ

		private Game mGame = null;

		// 戦闘入力データ
		public BattleDataIn BattleDataIn { get { return mGame.BattleIn; } }
		// 戦闘出力データ
		public BattleDataOut BattleDataOut { get { return mGame.BattleOut; } }
		// 侵攻(true)か防衛(false)か
		public bool IsInvasion { get { return BattleDataIn.IsInvasion; } }
		// 音楽再生
		public PlayMusic Music { get { return mMusic; } }
		// 戦闘速度
		public int BattleSpeed { get { return mGame.SystemData.Config.BattleSpeed; } set { mGame.SystemData.Config.BattleSpeed = value; } }
		// 戦闘速度倍率
		public float BattleSpeedMagni { get { return (float)System.Math.Pow(2.0f, BattleSpeed); } }
		// ターン(行動)ユニット
		public BattleUnit TurnUnit { get { return OrderController.TurnUnit; } }
		// バーマネージャ
		public HPBarManager Bar { get { return mBar.GetComponent<HPBarManager>(); } }
		// スキル参照ユニット(アリス)
		public BattleUnit UnitAlice { get; private set; }

		#endregion

		// 味方データ
		public List<BattleUnit> PlayerUnits { get; private set; }
		// 敵データ
		public List<BattleUnit> EnemyUnits { get; private set; }
		// 味方召喚ユニット
		public List<BattleUnit> PlayerSummonUnits { get; private set; }
		// 敵召喚ユニット
		public List<BattleUnit> EnemySummonUnits { get; private set; }
		// 味方カード
		public List<CardManager> PlayerCards { get; private set; }
		// 敵カード
		public List<CardManager> EnemyCards { get; private set; }
		// 領地データ
		public BattleArea Area { get; private set; }
		// ターン数
		public int TurnNum { get; set; }
		// ダメージ量
		public float DamageNum { get; private set; }
		// 最終的な攻撃対象
		public BattleUnit EndTargetUnit { get; private set; }
		// エフェクトFXController
		public FXController FXCtrl { get; private set; }
		// 背景シーン名
		public string BackGroundSceneName { get; private set; }

		#region フラグ系

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
		// 撤退したかどうか
		public bool IsEscape { get; private set; }
		// 一時停止しているかどうか
		public bool IsPause { get; set; }
		// 現在状態チェック中かどうか
		public bool IsCheck { get; private set; }
		// 現在兵士の攻撃中かどうか
		public bool IsAttackGroup { get; private set; }
		#endregion

		// データのロード
		void LoadData()
		{
			mGame = Game.GetInstance();

			// 背景シーン名決定
			BackGroundSceneName = "Resources/Scenes/Battle/BackGround/";
			if (BattleDataIn.TimeOfDay <= 1)
				BackGroundSceneName += "Day_";
			else if (BattleDataIn.TimeOfDay == 2)
				BackGroundSceneName += "Eve_";
			else if (BattleDataIn.TimeOfDay == 3)
				BackGroundSceneName += "Nig_";
			BackGroundSceneName += mGame.GameData.Area[mGame.BattleIn.AreaID].BackgroundName;

			// UIを非表示にセット
			mEscapeConfUI.SetActive(false);
			mConfigUI.SetActive(false);
			mBattleWinUI.SetActive(false);
			mBattleLoseUI.SetActive(false);
			mLastSerifUI.SetActive(false);
			mMessageUI.SetActive(false);
			mCardStartUI.SetActive(false);
			mCaptureGauge.SetActive(false);
			mCaptureGaugeBack.SetActive(false);
			Bar.HideAll();
			IsPlayerSelectTime = false;
			HideDescription();
			mDamageDisplayPlayer.GetComponent<DamageDisplay>().Setup();
			mDamageDisplayEnemy.GetComponent<DamageDisplay>().Setup();
			HideSkillCaption();

			// エリア(背景)セット
			Area = new BattleArea();

			// フラグ系セット
			IsEffect = false;
			IsBattleEnd = false;
			IsEscape = false;
			IsEndWaitTime = false;
			IsWaitInputTime = false;
			IsPause = false;
			IsCheck = false;
			IsAttackGroup = false;

			mEscapeButton.interactable = !BattleDataIn.IsTutorial;

			// 顔画像関連セット
			print("顔グラ設定");
			PlayerFaces = new List<FaceObj>();
			PlayerFaces.Add(mFaceP0);
			PlayerFaces.Add(mFaceP1);
			PlayerFaces.Add(mFaceP2);
			for (int i = 0; i < PlayerFaces.Count; ++i)
				PlayerFaces[i].SetPos(i, true);
			EnemyFaces = new List<FaceObj>();
			EnemyFaces.Add(mFaceE0);
			EnemyFaces.Add(mFaceE1);
			EnemyFaces.Add(mFaceE2);
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

			// カード用の参照ユニット(アリス)セット
			UnitAlice = BattleData.Instantiate(mUnitPrefab, "", mTfPlayer).GetComponent<BattleUnit>();
			UnitAlice.Setup(0, true, 0, this);

			// ユニット初期化
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
			PlayerSummonUnits = new List<BattleUnit>();
			EnemySummonUnits = new List<BattleUnit>();

			// カード初期化
			print("プレイヤーカード設定");
			PlayerCards = new List<CardManager>();
			for (int i = 0; i < BattleDataIn.PlayerCards.Count; i++)
			{
				var card = new CardManager();
				card.Setup(BattleDataIn.PlayerCards[i], true, PlayerCardObjs[i]);
				PlayerCards.Add(card);
			}
			print("エネミーカード設定");
			EnemyCards = new List<CardManager>();
			for (int i = 0; i < BattleDataIn.EnemyCards.Count; i++)
			{
				var card = new CardManager();
				card.Setup(BattleDataIn.EnemyCards[i], false, EnemyCardObjs[i]);
				EnemyCards.Add(card);
			}

			// 戦闘ターン数取得
			TurnNum = BattleDataIn.TurnNum == 0 ? mGame.GameData.Area[mGame.BattleIn.AreaID].Time : BattleDataIn.TurnNum;

			// 順番バー初期化
			print("順番枠設定");
			OrderController = mBehaviorOrder.GetComponent<OrderController>();
			OrderController.Setup(this);
			mTurnNumDisplay.GetComponent<TurnNumDisplay>().SetUp(this);

			mDamageDisplayPlayer.SetActive(false);
			mDamageDisplayEnemy.SetActive(false);

			// BGM再生
			//mGame.SoundManager.Play(BattleDataIn.BGM, SoundType.BGM);

			// 戦闘開始
			StartCoroutine(CoBattleStart());
		}

		#region 汎用系(待機)
		// 数秒待つコルーチン
		private IEnumerator CoWaitSeconds(float seconds)
		{
			float time = 0;
			while (true)
			{
				time += Time.deltaTime;
				if (time >= seconds)
					break;
				yield return null;
			}
		}
		public IEnumerator WaitSeconds(float seconds)
		{
			yield return StartCoroutine(CoWaitSeconds(seconds));
		}

		// 何か入力されるか、数秒経つまで待つコルーチン
		private IEnumerator CoWaitInputOrSeconds(float seconds)
		{
			float time = 0;
			IsWaitInputTime = true;
			while (IsWaitInputTime)
			{
				time += Time.deltaTime;
				if (time * BattleSpeedMagni >= seconds)
					IsWaitInputTime = false;
				yield return null;
			}
		}
		public IEnumerator WaitInputOrSeconds(float seconds)
		{
			yield return StartCoroutine(CoWaitInputOrSeconds(seconds));
		}

		// エフェクトを待つ
		private IEnumerator CoWaitEffect()
		{
			while (IsEffect)
				yield return null;
		}

		#endregion

		// エフェクトを発生させる
		private GameObject GenerateEffect(GameObject effectObj, string name, bool isUI)
		{
			// 生成
			if (!effectObj)
			{
				print("エフェクトが存在しません");
				return null;
			}
			var effect = BattleData.Instantiate(effectObj, name).gameObject;
			effect.transform.parent = (isUI ? mUIEffectParent.transform : transform);
			if (isUI)
			{
				effect.transform.localPosition = effectObj.transform.localPosition;
				effect.transform.localScale = effectObj.transform.localScale;
			}


			// コールバックセット
			IsEffect = true;
			FXCtrl = effect.GetComponent<FXController>();
			FXCtrl.EndEvent.AddListener(EndEffect);
			FXCtrl.PlaySpeed *= BattleSpeedMagni;
			FXCtrl.LifeTime /= BattleSpeedMagni;
			return effect;
		}

		private void GenerateEffect(string effectPath, bool isPlayer)
		{
			// 生成
			print("Load：エフェクト　" + effectPath);
			var obj = (GameObject)Resources.Load("Effects/Battle/Prefabs/" + effectPath);
			var effect = GenerateEffect(obj, effectPath, false);
			if (effect)
				effect.transform.localScale = new Vector3((isPlayer ? -1 : 1), 1, 1);
		}


		// カードの発動チェック＆発動　checkCamp: 0=両方, -1=敵, 1=味方
		private IEnumerator DoCardAction(CardDataFormat.CardTiming timeing, BattleUnit induceUnit, int checkCamp = 0)
		{
			List<CardManager> cards = new List<CardManager>();
			for (int i = 0; i < 3; i++)
			{
				if (TurnUnit == null || TurnUnit.IsPlayer)
				{
					if (i < PlayerCards.Count)
						cards.Add(PlayerCards[i]);
					if (i < EnemyCards.Count)
						cards.Add(EnemyCards[i]);
				}
				else
				{
					if (i < EnemyCards.Count)
						cards.Add(EnemyCards[i]);
					if (i < PlayerCards.Count)
						cards.Add(PlayerCards[i]);
				}
			}
			foreach (var card in cards)
			{
				if (card.CardData == null || !card.IsCanUse || checkCamp == (card.IsPlayer ? -1 : 1))
					continue;
				if (card.CardData.Timing == timeing)
				{
					if ((timeing == CardDataFormat.CardTiming.Rand80 && BattleRandom.value > 0.8f) ||
						(timeing == CardDataFormat.CardTiming.Rand50 && BattleRandom.value > 0.5f) ||
						(timeing == CardDataFormat.CardTiming.Rand20 && BattleRandom.value > 0.2f))
						continue;
					if (((timeing == CardDataFormat.CardTiming.UserState_HP10 ||
						timeing == CardDataFormat.CardTiming.EnemyState_HP10) &&
						induceUnit.UnitData.HP > induceUnit.UnitData.MaxHP * 0.1) ||
						((timeing == CardDataFormat.CardTiming.UserState_HP50 ||
						timeing == CardDataFormat.CardTiming.EnemyState_HP50) &&
						induceUnit.UnitData.HP > induceUnit.UnitData.MaxHP * 0.5) ||
						((timeing == CardDataFormat.CardTiming.UserState_S10 ||
						timeing == CardDataFormat.CardTiming.EnemyState_S10) &&
						induceUnit.UnitData.SoldierNum > induceUnit.UnitData.MaxSoldierNum * 0.1) ||
						((timeing == CardDataFormat.CardTiming.UserState_S50 ||
						timeing == CardDataFormat.CardTiming.EnemyState_S50) &&
						induceUnit.UnitData.SoldierNum > induceUnit.UnitData.MaxSoldierNum * 0.5))
						break;
					// カードスキル発動
					BattleUnit action = (card.IsPlayer ? PlayerUnits[0] : EnemyUnits[0]);            // 行動ユニットはその陣営の先頭ユニット
					if (card.Skill.Type == SkillDataFormat.SkillType.SoulSteal && induceUnit != null)// ソウルスティールなら行動ユニットは誘発ユニット
						action = induceUnit;

					BattleUnit target = null;
					if (card.Skill.Range == SkillDataFormat.SkillRange.Single)                       // 対象が単体の時
					{
						if (card.Skill.Type == SkillDataFormat.SkillType.Heal && induceUnit != null) // 回復効果なら誘発ユニット
							target = induceUnit;
						else                                                                    // それ以外ならランダム
						{
							List<BattleUnit> units = new List<BattleUnit>();
							if (card.Skill.Target == SkillDataFormat.SkillTarget.Enemy ||
								card.Skill.Target == SkillDataFormat.SkillTarget.EnemyLeader)        // 対象が敵の時
							{
								units = (card.IsPlayer ? EnemyUnits : PlayerUnits);
							}
							else if (card.Skill.Target == SkillDataFormat.SkillTarget.Player ||
								card.Skill.Target == SkillDataFormat.SkillTarget.PlayerLeader)       // 対象が味方の時
							{
								units = (!card.IsPlayer ? EnemyUnits : PlayerUnits);
							}
							target = units[BattleRandom.Range(0, units.Count - 1)];
						}
					}
					yield return StartCoroutine(CoCardSkillAction(card, action, target));
				}
			}
			yield return null;
		}

		// カード発動コルーチン
		private IEnumerator CoCardSkillAction(CardManager card, BattleUnit action, BattleUnit target)
		{
			// 一時停止
			while (IsPause)
				yield return null;
			var flame = card.Flame;
			if (flame)
			{
				for (int i = 0; i < 5; i++)
				{
					flame.SetActive(!flame.activeSelf);
					yield return WaitSeconds(0.06f);
				}
			}
			mCardStartUI.SetActive(true);
			yield return mCardStartUI.GetComponent<CardStartUI>().CardStart(card.CardData, card.CardObj);
			card.SetUseSprite();
			print("Load：カード起動エフェクト");
			GenerateEffect(mCardStartEffect, "CardStartEffect", true);
			yield return StartCoroutine(CoWaitEffect());
			mCardStartUI.SetActive(false);
			yield return StartCoroutine(CoSkillAction(action, target, card.Skill, card.IsPlayer, true));
			flame.SetActive(false);
			// カード数消費(無限じゃない時)
			card.SetUsedCard();
			yield return StartCoroutine(CoCheckUnit());
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

		// 戦闘開始時のコルーチン
		private IEnumerator CoBattleStart()
		{
			print("背景シーンロード");
			yield return SceneManager.LoadSceneAsync(BackGroundSceneName, LoadSceneMode.Additive);
			yield return null;
			mGame.HideNowLoading();

			mBattleStartUI.SetActive(true);
			var bsUI = mBattleStartUI.GetComponent<BattleStartUISetup>();
			if (bsUI)
				bsUI.Setup();
			Music.PlayVS();
			if (!BattleDataIn.IsAuto)
			{
				print("Load：戦闘開始エフェクト");
				GenerateEffect(mBattleStartEffect, "BattleStartEffect", true);
				// エフェクトの終わるまで待つ
				yield return StartCoroutine(CoWaitEffect());
			}
			yield return WaitInputOrSeconds(1);

			mBattleStartUI.SetActive(false);

			// 戦闘開始時がタイミングのカード発動
			yield return DoCardAction(CardDataFormat.CardTiming.BattleBegin, null);
			if (BattleDataIn.IsTutorial)
			{
				// チュートリアルイベント呼び出し
				EventDataFormat e = new EventDataFormat();
				e.FileName = "s9802";
				mGame.CallScript(e);
			}
			// ターン処理開始
			StartCoroutine(CoTurnProcess());
		}

		// AIの行動コルーチン
		private IEnumerator CoAIAction()
		{
			yield return WaitSeconds(0.125f);
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
					// 一時停止
					while (IsPause)
						yield return null;
					TurnUnit.SetGroupAnimatorState(1);
					bool isSlideOut = TurnUnit.LAtkSkill.Range == SkillDataFormat.SkillRange.All && targetUnit.Position != Position.Front;
					yield return StartCoroutine(CoSoldierAttack(targetUnit, isSlideOut));
					yield return WaitSeconds(0.125f);
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


			if (!IsBattleEnd && targetUnit != null && targetUnit.UnitData.HP != 0)
			{
				// 一時停止
				while (IsPause) yield return null;
				// リーダースキル発動
				TurnUnit.SetLeaderAnimatorState(isAttack ? 1 : 2);
				SetSkillCaption(lSkillData.Name, isAttack);
				MouseOverSkillButton(isAttack ? 0 : 1);
				yield return StartCoroutine(CoSkillAction(TurnUnit, targetUnit, lSkillData, TurnUnit.IsPlayer, false));
				HideDescription();
			}
			TurnUnit.SetLeaderAnimatorState(0);
			TurnUnit.SetGroupAnimatorState(0);
			yield return WaitSeconds(0.125f);
			HideSkillCaption();
		}

		// ダメージを与えた時のコルーチン
		private IEnumerator DamageProcess(DamageType type, BattleUnit atkUnit, BattleUnit defUnit, bool byLeader, bool toLeader,
			BattleUnit originTarget = null)
		{
			// カウンターダメージの時、対象が魔法キャラである・前衛でない・味方である・召喚ユニットである場合、反撃側が魔法キャラである場合反撃しない
			if (type == DamageType.Counter || type == DamageType.CaptureCounter)
			{
				if (defUnit.Position != Position.Front || defUnit.IsPlayer == atkUnit.IsPlayer || defUnit.IsSummonUnit)
					yield break;
				else if (atkUnit.IsExistSoldier ? atkUnit.UnitData.GroupMAtk > atkUnit.UnitData.GroupPAtk :
					 atkUnit.UnitData.LeaderMAtk > atkUnit.UnitData.LeaderPAtk)
					yield break;
				else if (defUnit.IsExistSoldier ? defUnit.UnitData.GroupMAtk > defUnit.UnitData.GroupPAtk :
					 defUnit.UnitData.LeaderMAtk > defUnit.UnitData.LeaderPAtk)
					yield break;
			}

			// ターゲットをスライドインさせる、スライドしている間待機
			yield return defUnit.SlideIn();
			if (type == DamageType.Normal)
				defUnit.Face.SetSelectArrow(true);
			yield return WaitSeconds(0.05f);
			EndTargetUnit = null;

			// ターゲットが庇われていた場合
			if (type == DamageType.Normal)
			{
				foreach (var unit in (defUnit.IsPlayer ? PlayerUnits : EnemyUnits))
				{
					if (unit != defUnit && unit != originTarget && unit.GuardTarget == defUnit)
					{
						// ターゲットを戻す
						yield return defUnit.SlideOut();
						if (type == DamageType.Normal)
							defUnit.Face.SetSelectArrow(false);
						yield return StartCoroutine(DamageProcess(type, atkUnit, unit, byLeader, toLeader,
							(originTarget == null ? defUnit : originTarget)));
						yield return unit.SlideOut();
						yield return defUnit.SlideIn();
						yield break;
					}
				}
			}

			IsNowIncDec = true;
			EndTargetUnit = defUnit;
			DamageNum = 0;
			yield return null;
			if (defUnit.IsStateNoDamage)
			{
				// ダメージを受けない状態であるならば
				while (FXCtrl && FXCtrl.LifeTime >= 0.5)
					yield return null;
				defUnit.IsStateNoDamage = false;
			}
			else
			{
				// ダメージを受ける
				var preLife = (!defUnit.IsExistSoldier || toLeader ? defUnit.UnitData.HP : defUnit.UnitData.SoldierNum);
				DamageNum = defUnit.Damage(type, atkUnit, byLeader, toLeader);

				var damageDisplay = (defUnit.IsPlayer ? mDamageDisplayPlayer : mDamageDisplayEnemy).GetComponent<DamageDisplay>();
				damageDisplay.Display(DamageNum, true);

				DamageNum = System.Math.Min(preLife, DamageNum);

				while (FXCtrl && FXCtrl.LifeTime >= 0.5)
					yield return null;
				for (int i = 5; i >= 0; --i)
				{
					defUnit.ApproachDisplay(i);
					defUnit.SetDisplaySoldier();
					Bar.SetBar(defUnit, 0, 0, 0);
					yield return WaitSeconds(0.05f);
				}
				yield return WaitSeconds(0.05f);
				yield return damageDisplay.Hide();

				// チュートリアルイベント呼び出し
				var isNotSolNumZero = !defUnit.IsPlayer && defUnit.UnitData.SoldierNum != 0;
				if (BattleDataIn.IsTutorial && isNotSolNumZero && defUnit.UnitData.SoldierNum == 0)
				{
					EventDataFormat e = new EventDataFormat();
					e.FileName = "s9803";
					mGame.CallScript(e);
				}
			}

			// 庇い対象を無しにする
			defUnit.GuardTarget = null;
			if (type == DamageType.Normal)
				defUnit.Face.SetSelectArrow(false);

			IsNowIncDec = false;
		}

		// 回復した時のコルーチン
		private IEnumerator HealProcess(BattleUnit target, float healHP, float healSolNum)
		{
			IsNowIncDec = true;
			// ターゲットをスライドインさせる、スライドしている間待機
			yield return target.SlideIn();
			target.Face.SetSelectArrow(true);
			while (FXCtrl && FXCtrl.LifeTime >= 0.5)
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
				Bar.SetBar(target, 0, 0, 0);
				yield return WaitSeconds(0.05f);
			}
			yield return WaitSeconds(0.05f);

			yield return damageDisplay.Hide();
			target.Face.SetSelectArrow(false);
			IsNowIncDec = false;
		}

		// ユニット状態確認コルーチン
		private IEnumerator CoCheckUnit()
		{
			if (IsCheck)
				yield break;
			IsCheck = true;
			List<BattleUnit> removeUnits = new List<BattleUnit>();
			for (int i = 0; i < 2; i++)
			{
				int removeCount = 0;
				List<bool> removeFlags = new List<bool>();
				var units = (i == 0 ? PlayerUnits : EnemyUnits);
				foreach (var unit in units)
				{
					if (unit.IsDamaged)
					{
						bool isDeath = unit.UnitData.HP == 0;
						// 死亡
						if (isDeath)
						{
							yield return DoCardAction(CardDataFormat.CardTiming.UserState_Death, unit, (unit.IsPlayer ? 1 : -1));
							yield return DoCardAction(CardDataFormat.CardTiming.EnemyState_Death, unit, (unit.IsPlayer ? -1 : 1));
						}
						else if (!IsAttackGroup)
						{
							bool isS50 = unit.UnitData.SoldierNum <= unit.UnitData.MaxSoldierNum * 0.5;
							bool isS10 = unit.UnitData.SoldierNum <= unit.UnitData.MaxSoldierNum * 0.1;
							bool isHP50 = unit.UnitData.HP <= unit.MaxHP * 0.5;
							bool isHP10 = unit.UnitData.HP <= unit.MaxHP * 0.1;
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
						}
						unit.IsDamaged = false;
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
			IsCheck = false;
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
			var imSerif = mLastSerifUI.transform.Find("Image");
			//            var imMessage = mMessageUI.transform.FindChild("Image");
			var text = imSerif.Find("Text").GetComponent<Text>();
			imSerif.localPosition = new Vector3(unit.Face.transform.localPosition.x, imSerif.localPosition.y, imSerif.localPosition.z);
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
			text = mMessageUI.transform.Find("Text").GetComponent<Text>();
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
		private IEnumerator CoSoldierAttack(BattleUnit tarUnit, bool isSlideOut)
		{
			IsAttackGroup = true;
			var gSkillData = TurnUnit.GAtkSkill;
			// エフェクト発生
			GenerateEffect(gSkillData.EffectPath, tarUnit.IsPlayer);
			// ダメージ処理
			StartCoroutine(DamageProcess(DamageType.Normal, TurnUnit, tarUnit, false, false));
			while (!IsNowIncDec) yield return null;
			// カウンターダメージ
			StartCoroutine(DamageProcess(DamageType.Counter, tarUnit, TurnUnit, !EndTargetUnit.IsExistSoldier, false));

			// エフェクト発生中は待つ
			yield return StartCoroutine(CoWaitEffect());
			while (IsNowIncDec)
				yield return null;
			yield return WaitSeconds(0.05f);
			// ターゲットをスライドアウトさせる、スライドしている間待機
			if (isSlideOut || tarUnit.UnitData.HP == 0)
				yield return tarUnit.SlideOut();
			yield return StartCoroutine(CoCheckUnit());
			IsAttackGroup = false;
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
				yield return WaitSeconds(0.125f);
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
				if (target && target != targetsList[0])
					yield return target.SlideOut();
				foreach (var tarUnit in targetsList)
				{
					// ターンユニットと同じ陣営のユニットをスライドインさせる前に、ターンユニットをスライドアウトさせる
					if (!isCard && actionUnit.IsDisplay && actionUnit != tarUnit && isPlayer == tarUnit.IsPlayer)
						yield return actionUnit.SlideOut();
					// 以前のターゲットユニットをスライドアウトさせる
					if (preUnit != null)
					{
						yield return preUnit.SlideOut();
						preUnit.Face.SetSelectArrow(false);
					}
					preUnit = tarUnit;
					// ターゲットユニットをスライドインさせる
					yield return tarUnit.SlideIn();
					tarUnit.Face.SetSelectArrow(true);
					// エフェクト発生
					GenerateEffect(skill.EffectPath, tarUnit.IsPlayer);
					// 攻撃ユニット
					var atkUnit = (isCard ? UnitAlice : actionUnit);
					var toLeader = skill.Target == SkillDataFormat.SkillTarget.EnemyLeader ||
								skill.Target == SkillDataFormat.SkillTarget.PlayerLeader;
					// 処理
					if (skill.Type == SkillDataFormat.SkillType.Damage)
					{
						// 0:ダメ―ジ
						// ダメージ処理
						StartCoroutine(DamageProcess(DamageType.Normal, atkUnit, tarUnit, true, toLeader));
						while (!IsNowIncDec)
							yield return null;
						// カウンターダメージ
						if (!isCard)
							StartCoroutine(DamageProcess(DamageType.Counter, tarUnit, atkUnit, !EndTargetUnit.IsExistSoldier, false));

						// 毒を与える効果
						if (skill.Attribute[0] && BattleRandom.value <= 0.5)
						{
							tarUnit.IsStatePoison = true;
							// エフェクト？

							yield return DoCardAction(CardDataFormat.CardTiming.UserState_Poison, tarUnit, (tarUnit.IsPlayer ? 1 : -1));
							yield return DoCardAction(CardDataFormat.CardTiming.EnemyState_Poison, tarUnit, (tarUnit.IsPlayer ? -1 : 1));
						}
					}
					else if (skill.Type == SkillDataFormat.SkillType.Heal)
					{
						// 1:回復
						// 回復処理					
						if (skill.Attribute[0])         // 毒を回復させるなら
							tarUnit.IsStatePoison = false;
						else                            // HPを回復させる
							StartCoroutine(HealProcess(tarUnit, tarUnit.GetLeaderCurativeAmount(skill), tarUnit.GetGroupCurativeAmount(skill)));
					}
					else if (skill.Type == SkillDataFormat.SkillType.StatusUp || skill.Type == SkillDataFormat.SkillType.StatusDown)
					{
						// 2:ステ上昇 or 3:ステ下降
						tarUnit.SufferStatus(skill);
					}
					else if (!isCard && skill.Type == SkillDataFormat.SkillType.SoulSteal)
					{
						// 5:ダメージ還元
						// ダメージ処理
						StartCoroutine(DamageProcess(DamageType.Normal, atkUnit, tarUnit, true, toLeader));
						while (!IsNowIncDec) yield return null;
						// カウンターダメージ
						StartCoroutine(DamageProcess(DamageType.Counter, tarUnit, atkUnit, !EndTargetUnit.IsExistSoldier, false));
						while (IsNowIncDec) yield return null;
						// 回復
						if (DamageNum != 0) StartCoroutine(HealProcess(actionUnit, DamageNum, 0));
					}
					else if (!isCard && skill.Type == SkillDataFormat.SkillType.Guard)
					{
						// 6:ガード
						actionUnit.GuardTarget = tarUnit;
					}
					else if (skill.Type == SkillDataFormat.SkillType.NoDamage)
					{
						// 8:ダメージ無効
						tarUnit.IsStateNoDamage = true;
					}
					else if (skill.Type == SkillDataFormat.SkillType.StatusOff)
					{
						// 10:ステータス取り消し
						tarUnit.Status.Clear();
					}
					else if (skill.Type == SkillDataFormat.SkillType.Random)
					{
						// 11:ランダム
						if (isRandomDamage)     // ダメージ
						{
							StartCoroutine(DamageProcess(DamageType.Normal, atkUnit, tarUnit, true, toLeader));
							while (!IsNowIncDec)
								yield return null;
							// カウンターダメージ
							if (!isCard)
								StartCoroutine(DamageProcess(DamageType.Counter, tarUnit, atkUnit, !EndTargetUnit.IsExistSoldier, false));
						}
						else                    // 回復
							StartCoroutine(HealProcess(tarUnit, tarUnit.GetLeaderCurativeAmount(skill), tarUnit.GetGroupCurativeAmount(skill)));
					}
					// エフェクト発生中は待つ
					yield return StartCoroutine(CoWaitEffect());
					while (IsNowIncDec) yield return null;
					yield return WaitSeconds(0.125f);
				}
				// 以前のターゲットユニットをスライドアウトさせる
				if (preUnit)
				{
					if (preUnit != (isCard ? TurnUnit : actionUnit)) yield return preUnit.SlideOut();
					preUnit.Face.SetSelectArrow(false);
				}
			}
			// ターンユニットがスライドアウトしている場合、スライドインさせる
			if (!isCard && !TurnUnit.IsDisplay) yield return TurnUnit.SlideIn();
		}

		// 捕獲するコルーチン
		private IEnumerator CoCapture(BattleUnit tarUnit, BattleUnit originTarget = null)
		{
			// ターゲットをスライドインさせる、スライドしている間待機
			yield return tarUnit.SlideIn();
			yield return WaitSeconds(0.125f);

			// ターゲットが庇われていた場合
			foreach (var unit in (tarUnit.IsPlayer ? PlayerUnits : EnemyUnits))
			{
				// ターゲットを戻す
				if (unit != tarUnit && unit != originTarget && unit.GuardTarget == tarUnit && unit.UnitData.Deathable)
				{
					yield return tarUnit.SlideOut();
					yield return StartCoroutine(CoCapture(unit, (originTarget == null ? tarUnit : originTarget)));
					yield break;
				}
			}

			// 捕獲カウンターダメージ
			StartCoroutine(DamageProcess(DamageType.CaptureCounter, tarUnit, TurnUnit, !EndTargetUnit.IsExistSoldier, false));

			if (tarUnit.IsStateNoDamage)
				// ダメージを受けない状態であるならば
				tarUnit.IsStateNoDamage = false;
			else
			{
				// 捕獲ダメージ処理
				var preCaptureGauge = tarUnit.CaptureGauge;
				mCaptureGauge.SetActive(true);
				mCaptureGaugeBack.SetActive(true);
				CaptureGauge.fillAmount = preCaptureGauge / 100;
				tarUnit.SufferCaptureDamage(tarUnit);
				// 捕獲音再生
				Music.PlayCaptureGauge();
				var text = mCaptureGauge.transform.Find("Text").GetComponent<Text>();
				float time = 0;
				var endTime = mCaptureGaugeTime / BattleSpeedMagni;
				while (time < endTime)
				{
					var rate = time / endTime;
					var gauge = (preCaptureGauge + (tarUnit.CaptureGauge - preCaptureGauge) * rate);
					CaptureGauge.fillAmount = gauge / 100;
					text.text = (int)gauge + "％";
					Bar.SetBar(tarUnit, 0, 0, 0, (tarUnit.CaptureGauge - preCaptureGauge) * (rate - 1));
					yield return null;
					time += Time.deltaTime;
				}

				text.text = (int)tarUnit.CaptureGauge + "％";
				CaptureGauge.fillAmount = tarUnit.CaptureGauge / 100;
				yield return WaitSeconds(0.3f / BattleSpeedMagni);
			}

			// エフェクト発生中は待つ
			yield return StartCoroutine(CoWaitEffect());
			while (IsNowIncDec) yield return null;
			yield return WaitSeconds(0.1f);
			mCaptureGauge.SetActive(false);
			mCaptureGaugeBack.SetActive(false);

			// ターゲットをスライドアウトさせる、スライドしている間待機
			yield return tarUnit.SlideOut();
		}

		// ターン処理のコルーチン
		private IEnumerator CoTurnProcess()
		{
			IsPlayerSelectTime = false;
			IsPlayerActionTime = false;
			while (OrderController.IsAnimation) yield return null;
			var preTurnUnit = TurnUnit;
			TurnUnit.IsDefense = false;
			yield return TurnUnit.SlideIn();
			if (!TurnUnit.IsSummonUnit) TurnUnit.Face.SetActionArrow(true);

			// 確率発動
			yield return DoCardAction(CardDataFormat.CardTiming.Rand80, TurnUnit, (TurnUnit.IsPlayer ? 1 : -1));
			yield return DoCardAction(CardDataFormat.CardTiming.Rand50, TurnUnit, (TurnUnit.IsPlayer ? 1 : -1));
			yield return DoCardAction(CardDataFormat.CardTiming.Rand20, TurnUnit, (TurnUnit.IsPlayer ? 1 : -1));
			foreach (var unit in PlayerUnits)
				if (unit != TurnUnit) unit.SlideOut();
			foreach (var unit in EnemyUnits)
				if (unit != TurnUnit) unit.SlideOut();
			yield return TurnUnit.SlideIn();

			// 一時停止
			while (IsPause) yield return null;

			if (!IsBattleEnd)
			{
				// 敵、または自動戦闘の場合
				if (!TurnUnit.IsPlayer || BattleDataIn.IsAuto)
					yield return StartCoroutine(CoAIAction());
				else
				{
					// 味方キャラの場合
					IsPlayerSelectTime = true;
					mMouseCover.SetActive(true);
					HideDescription();
					while ((IsPlayerSelectTime || IsPlayerActionTime) && !IsBattleEnd) yield return null;
				}

				// ターン終了
				yield return StartCoroutine(CoCheckUnit());

				if (!preTurnUnit.IsSummonUnit)
					preTurnUnit.Face.SetActionArrow(false);
				if (preTurnUnit.IsSummonUnit || preTurnUnit.Face.IsExsistUnit)
				{
					if (preTurnUnit.IsStatePoison)
					{
						// 毒ダメージエフェクト発生
						GenerateEffect("毒ダメージエフェクトパス名", preTurnUnit.IsPlayer);
						// 毒状態なら毒ダメージを受ける
						yield return StartCoroutine(DamageProcess(DamageType.Poison, null, preTurnUnit, false, false));
						yield return WaitSeconds(0.05f);
						yield return StartCoroutine(CoCheckUnit());
					}
					bool isSummoneUnit = preTurnUnit.IsSummonUnit;
					yield return preTurnUnit.TurnEnd();

					if (!IsBattleEnd && (!isSummoneUnit || !preTurnUnit.IsReturnSummonUnit))
						yield return OrderController.TurnEnd();
				}

				yield return preTurnUnit.SlideOut();

				if (TurnNum > 0) --TurnNum;
				if (TurnNum == 0) IsBattleEnd = true;
				if (IsEscape) IsBattleEnd = true;
			}

			if (IsBattleEnd)
				// 戦闘終了
				StartCoroutine(CoBattleEnd());
			else
			{
				// 次のターン
				Music.PlayNextTurn();
				StartCoroutine(CoTurnProcess());
			}
		}

		// 戦闘終了のコルーチン
		private IEnumerator CoBattleEnd()
		{
			yield return DoCardAction(CardDataFormat.CardTiming.BattleEnd, null);
			bool isWin;
			if ((TurnNum == 0 && EnemyUnits.Count != 0) || (PlayerUnits.Count == 0 && EnemyUnits.Count == 0))
				// 時間切れなら侵攻側が負け または 全滅引き分けなら侵攻側が負け
				isWin = !IsInvasion;
			else if (IsEscape)
				// 撤退すると敗北
				isWin = false;
			else
				// 敵が全滅しているなら勝ち、 プレイヤーが全滅しているなら負け
				isWin = (EnemyUnits.Count == 0);
			BattleDataOut.IsWin = isWin;

			if (BattleDataIn.IsAuto)
			{
				BattleEndEvent();
				yield break;
			}

			if (isWin)
			{
				Music.PlayWin();
				mBattleWinUI.SetActive(true);
				print("WIN");
				print("Load：戦闘勝利エフェクト");
				GenerateEffect(mBattleWinEffect, "BattleWinEffect", true);
			}
			else
			{
				Music.PlayLose();
				mBattleLoseUI.SetActive(true);
				print("LOSE");
				print("Load：戦闘敗北エフェクト");
				GenerateEffect(mBattleLoseEffect, "BattleLoseEffect", true);
			}
			if (BattleDataIn.IsTutorial)
			{
				// チュートリアルイベント呼び出し
				EventDataFormat e = new EventDataFormat();
				e.FileName = "s9804";
				mGame.CallScript(e);
				// 一時停止
				yield return null;
				while (mGame.IsTalk)
					yield return null;
			}
			// エフェクトの終わるまで待つ
			yield return StartCoroutine(CoWaitEffect());
			mMessageUI.SetActive(true);
			mMessageUI.transform.Find("Text").GetComponent<Text>().text = (IsInvasion ? 2 : 1) + (isWin ? 1 : 0) + "ポイントの経験値を得た";
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
					// 一時停止
					while (IsPause) yield return null;
					TurnUnit.SetGroupAnimatorState(1);
					bool isSlideOut = TurnUnit.LAtkSkill.Range == SkillDataFormat.SkillRange.All && target.Position != Position.Front;
					yield return StartCoroutine(CoSoldierAttack(target, isSlideOut));
					yield return WaitSeconds(0.125f);
				}
				if (!IsBattleEnd && target != null && target.UnitData.HP != 0)
				{
					// 一時停止
					while (IsPause) yield return null;
					// リーダースキル発動
					SetSkillCaption(TurnUnit.LAtkSkill.Name, true);
					TurnUnit.SetLeaderAnimatorState(1);
					yield return StartCoroutine(CoSkillAction(TurnUnit, target, TurnUnit.LAtkSkill, TurnUnit.IsPlayer, false));
				}
			}
			else if (type == 1)
			{
				// 一時停止
				while (IsPause) yield return null;
				//　防御
				TurnUnit.SetGroupAnimatorState(2);
				TurnUnit.IsDefense = true;
				// リーダースキル発動
				SetSkillCaption(TurnUnit.LDefSkill.Name, false);
				TurnUnit.SetLeaderAnimatorState(2);
				yield return StartCoroutine(CoSkillAction(TurnUnit, target, TurnUnit.LDefSkill, TurnUnit.IsPlayer, false));
			}
			else
			{
				// 一時停止
				while (IsPause) yield return null;
				// 捕獲
				TurnUnit.SetGroupAnimatorState(1);
				TurnUnit.SetLeaderAnimatorState(1);
				yield return StartCoroutine(CoCapture(target));
			}
			TurnUnit.SetLeaderAnimatorState(0);
			TurnUnit.SetGroupAnimatorState(0);
			yield return WaitSeconds(0.125f);
			HideSkillCaption();
			IsPlayerActionTime = false;
			HideDescription();
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
			PlayerUnits[0].Face.SetAllFaceHideButton();
			Music.PlayClickSkillButton();
			target.Face.SetSelectArrow(false);
			StartCoroutine(CoPlayerAction(target, type));
		}

		// スキル選択ボタンがマウスオーバーしたことを通知する関数
		public void MouseOverSkillButton(int type)
		{
			if (mDescription.activeSelf)
			{
				StopCoroutine(CoSlideDescriptionText());
				var textRect = mDescription.GetComponent<Text>().rectTransform;
				textRect.localPosition = new Vector3(0, 0, 0);
			}
			mDescription.SetActive(true);
			var text = mDescription.GetComponent<Text>();
			if (type == 0)
				text.text = TurnUnit.LAtkSkill.Description;
			else if (type == 1)
				text.text = TurnUnit.LDefSkill.Description;
			else if (type == 2)
				text.text = "対象を捕獲しようと試みる。";
		}

		// 説明文を消す
		public void HideDescription()
		{
			mDescription.SetActive(IsPlayerSelectTime);
			var text = mDescription.GetComponent<Text>();
			if (IsPlayerSelectTime)
			{
				text.text = mDescriptionText;
				text.rectTransform.sizeDelta = new Vector2(text.preferredWidth, text.rectTransform.sizeDelta.y);
				if (text.preferredWidth >= mMaskDescription.sizeDelta.x)
					StartCoroutine(CoSlideDescriptionText());
			}
			else
				text.rectTransform.sizeDelta = mMaskDescription.sizeDelta;
		}

		public IEnumerator CoSlideDescriptionText()
		{
			var text = mDescription.GetComponent<Text>();
			var textRect = text.rectTransform;
			var width = (text.preferredWidth + mMaskDescription.sizeDelta.x) / 2;
			textRect.localPosition = new Vector3(width, 0, 0);
			while (true)
			{
				textRect.localPosition -= new Vector3(text.preferredWidth * 2 / mDescriptionTextSlideTime * Time.deltaTime, 0, 0);
				if (textRect.localPosition.x < -width)
					textRect.localPosition = new Vector3(width, 0, 0);
				yield return null;
			}
		}

		// スキルキャプション表示
		public void SetSkillCaption(string skillName, bool isAttack)
		{
			mSkillCaption.GetComponent<Image>().sprite = (isAttack ? mSC_Atk : mSC_Def);
			mSkillCaption.transform.Find("Text").GetComponent<Text>().text = skillName;
			mSkillCaption.SetActive(true);
		}

		// スキルキャプション非表示
		public void HideSkillCaption()
		{
			mSkillCaption.SetActive(false);
		}

		// 撤退ボタンを押したときの関数
		public void PushEscape()
		{
			if (!IsPlayerSelectTime) return;
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
			var slider = mConfigUI.transform.Find("Slider").GetComponent<Slider>();
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
				BattleEndEvent();
			if (IsWaitInputTime && Input.GetMouseButtonDown(0))
				IsWaitInputTime = false;
		}

		public void BattleEndEvent()
		{
			if (EndEvent != null)
			{
				EndEvent.Invoke();
				//					if (SceneManager.GetSceneByName(BackGroundSceneName).IsValid())
				SceneManager.UnloadSceneAsync(BackGroundSceneName);
				Resources.UnloadUnusedAssets();
				print("戦闘終了");
			}
		}
	}

}