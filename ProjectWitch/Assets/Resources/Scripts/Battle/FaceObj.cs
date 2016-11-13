using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using System.Linq; //iOSで動かないかも

namespace Battle
{
	// 顔グラの周囲全般を管理するオブジェクト
	public class FaceObj : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		[SerializeField]
		private GameObject mAttackButton = null, mDefenseButton = null, mCaptureButton = null;
		[SerializeField]
		private GameObject mActionFlame = null, mSelectFlame = null;
		[SerializeField]
		private GameObject mNameText = null, mStateText = null;
		[SerializeField]
		private Transform mStatusContainer = null;
		[SerializeField]
		private GameObject mStatusUpDown = null, mStatusPoison = null, mStatusGuard = null, mStatusNoDamage = null;
		[SerializeField]
		private Sprite mSpriteStatusUp = null, mSpriteStatusDown = null,
			mSpriteStatusLead = null, mSpriteStatusAgi = null, mSpriteStatusPhyAtk = null, mSpriteStatusPhyDef = null, mSpriteStatusMagAtk = null, mSpriteStatusMagDef = null;

		private int mHP, mSoldierNum;
		private Vector3 mBaseRectPos;
		private Vector2 mBaseRectSize;
		private List<float> mPreStatusDataUpDown;
		private List<bool> mPreStatusDataOther;
		private List<GameObject> Icons { get; set; }
		private List<GameObject> PrefabStatusOthers { get; set; }
		private List<Sprite> SpriteUpDownType { get; set; }

		public BattleObj BattleObj { get; private set; }
		public SkillButton AttackButton { get { return mAttackButton.GetComponent<SkillButton>(); } }
		public SkillButton DefenseButton { get { return mDefenseButton.GetComponent<SkillButton>(); } }
		public SkillButton CaptureButton { get { return mCaptureButton.GetComponent<SkillButton>(); } }
		public RectTransform Rect { get { return gameObject.GetComponent<RectTransform>(); } }
		public BattleUnit Unit { get; private set; }
		public Transform HPText { get { return transform.FindChild("HPText"); } }
		public Text SoldierNumText { get { return HPText.FindChild("SNum").GetComponent<Text>(); } }
		public Text LeaderHPText { get { return HPText.FindChild("HNum").GetComponent<Text>(); } }
		public bool IsExsistUnit { get; private set; }
		public int Pos { get; private set; }
		public bool IsPlayer { get; private set; }
		public bool IsMoving { get; private set; }
		// 音楽再生
		public PlayMusic Music { get { return BattleObj.Music; } }


		// ユニットを設定する
		public void SetUnit(BattleUnit unit)
		{
			Unit = unit;
			Unit.Face = this;
			var image = gameObject.GetComponent<Image>();
			image.sprite = Resources.Load<Sprite>("Textures/Face/" + unit.UnitData.FaceIamgePath);
			if (image.sprite == null)
				image.sprite = Resources.Load<Sprite>("Textures/Face/634_fv_leadersoldierA");
			HPText.gameObject.SetActive(true);
			mHP = Unit.DisplayHP;
			mSoldierNum = Unit.DisplaySoldierNum;
			LeaderHPText.text = Unit.DisplayHP.ToString();
			SoldierNumText.text = Unit.DisplaySoldierNum.ToString();

			mNameText.GetComponent<Text>().text = Unit.UnitData.Name;
			mStateText.GetComponent<Text>().text = "";

			AttackButton.Setup(this);
			DefenseButton.Setup(this);
			CaptureButton.Setup(this);
			SetActionFlame(false);
			IsExsistUnit = true;
			Icons = new List<GameObject>();
			PrefabStatusOthers = new List<GameObject>();
			PrefabStatusOthers.Add(mStatusPoison);
			PrefabStatusOthers.Add(mStatusGuard);
			PrefabStatusOthers.Add(mStatusNoDamage);
			SpriteUpDownType = new List<Sprite>();
			SpriteUpDownType.Add(mSpriteStatusPhyAtk);
			SpriteUpDownType.Add(mSpriteStatusPhyDef);
			SpriteUpDownType.Add(mSpriteStatusMagAtk);
			SpriteUpDownType.Add(mSpriteStatusMagDef);
			SpriteUpDownType.Add(mSpriteStatusAgi);
			SpriteUpDownType.Add(mSpriteStatusLead);

			mPreStatusDataUpDown = Enumerable.Repeat<float>(0, 6).ToList();
			mPreStatusDataOther = Enumerable.Repeat<bool>(false, 3).ToList();
			SetStatusIcons();
		}

	// ステータスアイコンを設定する
	public void SetStatusIcons()
		{
			for (int i = 0; i < mPreStatusDataUpDown.Count; i++)
				mPreStatusDataUpDown[i] = Unit.GetStatusPercent(i);
			for (int i = 0; i < mPreStatusDataOther.Count; i++)
				mPreStatusDataOther[i] = Unit.GetStatusOtherFlag(i);

			foreach (var icon in Icons)
				Destroy(icon);
			Icons.Clear();
			int iconNum = 0;
			for (int i = 0; i < mPreStatusDataOther.Count; i++)
			{
				if (mPreStatusDataOther[i])
				{
					var prefab = PrefabStatusOthers[i];
					var icon = BattleData.Instantiate(prefab, prefab.name, mStatusContainer);
					icon.localPosition = new Vector3(-95 + (iconNum % 4) * 65, 210 + (iconNum / 4) * 85, 0);
					icon.localScale = PrefabStatusOthers[i].transform.localScale;
					Icons.Add(icon.gameObject);
					++iconNum;
				}
			}
			for (int i = 0; i < mPreStatusDataUpDown.Count; i++)
			{
				if (mPreStatusDataUpDown[i] != 0)
				{
					var icon = BattleData.Instantiate(mStatusUpDown, mStatusUpDown.name, mStatusContainer);
					icon.localPosition = new Vector3(-95 + (iconNum % 4) * 65, 210 + (iconNum / 4) * 85, 0);
					icon.localScale = mStatusUpDown.transform.localScale;
					var arrow = icon.FindChild("Arrow").GetComponent<Image>();
					var type = icon.FindChild("Type").GetComponent<Image>();
					var text = icon.FindChild("Text").GetComponent<Text>();
					arrow.sprite = (mPreStatusDataUpDown[i] > 0 ? mSpriteStatusUp : mSpriteStatusDown);
					type.sprite = SpriteUpDownType[i];
					text.text = (int)mPreStatusDataUpDown[i] + "%";
					Icons.Add(icon.gameObject);
					++iconNum;
				}
			}
		}

		// 場所を設定する
		public void SetPos(int pos, bool isPlayer)
		{
			IsMoving = false;
			IsPlayer = isPlayer;
			Pos = pos;
			gameObject.GetComponent<RectTransform>().localPosition = new Vector3((mBaseRectPos.x + mBaseRectSize.x * Pos) * (IsPlayer ? -1 : 1), mBaseRectPos.y, mBaseRectPos.z);
		}

		// 場所を移動するコルーチン
		private IEnumerator CoMovePos(int pos)
		{
			IsMoving = true;
			var rect = gameObject.GetComponent<RectTransform>();
			float speedPerSec = 20f * BattleObj.BattleSpeedMagni;
			var targetPosX = (mBaseRectPos.x + mBaseRectSize.x * pos) * (IsPlayer ? -1 : 1);
			var isLeft = (IsPlayer ? Pos < pos : pos < Pos);
			while (isLeft ? rect.localPosition.x > targetPosX : rect.localPosition.x < targetPosX)
			{
				rect.localPosition += new Vector3(speedPerSec * (isLeft ? -1 : 1), 0, 0);
				yield return null;
			}
			SetPos(pos, IsPlayer);
		}

		// 場所を移動する
		public void MovePos(int pos)
		{
			if (pos == Pos)
				return;
			StartCoroutine("CoMovePos", pos);
			if (pos == 0)
				Unit.Position = Position.Front;
			else if (pos == 1)
				Unit.Position = Position.Middle;
			else
				Unit.Position = Position.Rear;
		}

		// 空ユニットを設定する
		public void SetNoneUnit()
		{
			Unit = null;
			IsExsistUnit = false;
			var image = gameObject.GetComponent<Image>();
			image.sprite = Resources.Load<Sprite>("Textures/Face/640_fv_non");
			HPText.gameObject.SetActive(false);
			mNameText.GetComponent<Text>().text = "";
			mStateText.GetComponent<Text>().text = "";
		}

		// 行動設定
		public void SetActionFlame(bool action)
		{
			mActionFlame.SetActive(action);
		}

		// 死亡設定
		public void SetDead()
		{
			var image = gameObject.GetComponent<Image>();
			image.color = new Color(255 / 255.0f, 47 / 255.0f, 47 / 255.0f);
			HPText.gameObject.SetActive(false);
			mStateText.SetActive(true);
			mStateText.GetComponent<Text>().text = "死亡";
			IsExsistUnit = false;
		}

		// 捕獲設定
		public void SetCapture()
		{
			var image = gameObject.GetComponent<Image>();
			image.color = new Color(47 / 255.0f, 47 / 255.0f, 255 / 255.0f);
			HPText.gameObject.SetActive(false);
			mStateText.SetActive(true);
			mStateText.GetComponent<Text>().text = "捕獲";
			IsExsistUnit = false;
		}

		// 撤退設定
		public void SetRetreat()
		{
			var image = gameObject.GetComponent<Image>();
			image.color = new Color(47 / 255.0f, 47 / 255.0f, 47 / 255.0f);
			HPText.gameObject.SetActive(false);
			mStateText.SetActive(true);
			mStateText.GetComponent<Text>().text = "敗走";
			IsExsistUnit = false;
		}

		// ボタンの名称を設定する
		public void SetButton(string attack, string defense, bool isCapture)
		{
			int pos = 0;
			if (attack != "")
				AttackButton.SetButton(attack, pos++);
			if (defense != "")
				DefenseButton.SetButton(defense, pos++);
			if (isCapture && mCaptureButton != null)
				CaptureButton.SetButton("捕獲する", pos);
		}

		// ボタンを全て非表示にする
		public void HideButton()
		{
			AttackButton.HideButton();
			DefenseButton.HideButton();
			CaptureButton.HideButton();
		}

		void Awake()
		{
			mBaseRectPos = gameObject.GetComponent<RectTransform>().localPosition;
			mBaseRectSize = gameObject.GetComponent<RectTransform>().sizeDelta;
		}

		// Use this for initialization
		void Start()
		{
			BattleObj = GameObject.Find("/BattleObject").GetComponent<BattleObj>();
			mSelectFlame.SetActive(false);
		}

		// Update is called once per frame
		void Update()
		{
			if (!Unit)
				return;
			if (mHP != Unit.DisplayHP)
			{
				mHP = Unit.DisplayHP;
				LeaderHPText.text = Unit.DisplayHP.ToString();
			}
			if (mSoldierNum != Unit.DisplaySoldierNum)
			{
				mSoldierNum = Unit.DisplaySoldierNum;
				SoldierNumText.text = Unit.DisplaySoldierNum.ToString();
			}
			// ステータスアイコン変更
			bool statusChangeFlag = false;
			for (int i = 0; i < mPreStatusDataUpDown.Count; i++)
			{
				if (mPreStatusDataUpDown[i] != Unit.GetStatusPercent(i))
				{
					statusChangeFlag = true;
					break;
				}
			}
			if (!statusChangeFlag)
			{
				for (int i = 0; i < mPreStatusDataOther.Count; i++)
				{
					if(mPreStatusDataOther[i]!= Unit.GetStatusOtherFlag(i))
					{
						statusChangeFlag = true;
						break;
					}
				}
			}
			if (statusChangeFlag)
				SetStatusIcons();
		}

		// マウスオーバーした時
		public void OnPointerEnter(PointerEventData e) {
			if (!BattleObj.IsPlayerSelectTime || !IsExsistUnit || Unit.UnitData.HP == 0)
				return;
			var turnUnit = BattleObj.TurnUnit;
			string atkSkillName = "", defSkillName = "";
			bool isCapture = (!Unit.IsPlayer && Unit.UnitData.Deathable && !Unit.IsExistSoldier);
			if (turnUnit.IsPlayer == Unit.IsPlayer)
			{
				// 味方
				if (turnUnit == Unit && turnUnit.LDefSkill.Target == GameData.SkillDataFormat.SkillTarget.Own)
					defSkillName = turnUnit.LDefSkill.Name;
				else if (turnUnit.LDefSkill.Target == GameData.SkillDataFormat.SkillTarget.Player ||
					turnUnit.LDefSkill.Target == GameData.SkillDataFormat.SkillTarget.EnemyAndPlayer ||
					turnUnit.LDefSkill.Target == GameData.SkillDataFormat.SkillTarget.PlayerLeader)
				{
					defSkillName = turnUnit.LDefSkill.Name;
					if (turnUnit.LDefSkill.Range == GameData.SkillDataFormat.SkillRange.All)
						foreach (var face in (Unit.IsPlayer ? BattleObj.PlayerFaces : BattleObj.EnemyFaces))
							if (face.Unit != Unit && face.IsExsistUnit)
								face.SetButton("", defSkillName, false);
				}
			}
			else
			{
				// 敵
				if (turnUnit.LAtkSkill.Target == GameData.SkillDataFormat.SkillTarget.Enemy ||
					turnUnit.LAtkSkill.Target == GameData.SkillDataFormat.SkillTarget.EnemyAndPlayer ||
					turnUnit.LAtkSkill.Target == GameData.SkillDataFormat.SkillTarget.EnemyLeader)
				{
					atkSkillName = turnUnit.LAtkSkill.Name;
					if (turnUnit.LAtkSkill.Range == GameData.SkillDataFormat.SkillRange.All)
						foreach (var face in (Unit.IsPlayer ? BattleObj.PlayerFaces : BattleObj.EnemyFaces))
							if (face.Unit != Unit && face.IsExsistUnit)
								face.SetButton(atkSkillName, "", false);
				}
				if (turnUnit.LDefSkill.Target == GameData.SkillDataFormat.SkillTarget.Enemy ||
					turnUnit.LDefSkill.Target == GameData.SkillDataFormat.SkillTarget.EnemyAndPlayer ||
					turnUnit.LDefSkill.Target == GameData.SkillDataFormat.SkillTarget.EnemyLeader)
				{
					defSkillName = turnUnit.LDefSkill.Name;
					if (turnUnit.LDefSkill.Range == GameData.SkillDataFormat.SkillRange.All)
						foreach (var face in (Unit.IsPlayer ? BattleObj.PlayerFaces : BattleObj.EnemyFaces))
							if (face.Unit != Unit && face.IsExsistUnit)
								face.SetButton("", defSkillName, false);
				}
			}
			SetButton(atkSkillName, defSkillName, isCapture);
			if (!Unit.IsPlayer)
				Unit.StartCoroutine("SlideIn");
			mSelectFlame.SetActive(true);
		}

		// マウスが離されたとき
		public void OnPointerExit(PointerEventData e) {
			if (!BattleObj.IsPlayerSelectTime || !IsExsistUnit)
				return;
			SetAllFaceHide();
			if (!Unit.IsPlayer)
				Unit.StartCoroutine("SlideOut");
			mSelectFlame.SetActive(false);
		}

		// 選択枠を非表示
		public void SetSelectFlame(bool flag)
		{
			mSelectFlame.SetActive(flag);
		}

		public void SetAllFaceHide()
		{
			foreach (var face in BattleObj.PlayerFaces)
				if (face.IsExsistUnit)
					face.HideButton();
			foreach (var face in BattleObj.EnemyFaces)
				if (face.IsExsistUnit)
					face.HideButton();
		}
	}
}
