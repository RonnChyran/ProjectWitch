﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Battle
{
	// 行動順、およびその表示を管理するクラス
	public class OrderController : MonoBehaviour
	{
		// 表示させるオブジェクトのプレハブ
		[SerializeField]
		private GameObject mDisplayObject = null;
		// 表示オブジェクト
		private List<OrderDiplayObj> mOrderDiplayObj;
		private BattleObj mBattleObj;
		public static int DisplayOutCount { get; private set; }
		public static int NextOrderValue { get; set; }
		public BattleUnit TurnUnit { get { return (mOrderDiplayObj.Count != 0 ? mOrderDiplayObj[0].BattleUnit : null); } }
		public List<OrderDiplayObj> OrderDiplayObj { get { return mOrderDiplayObj; } }

		// アニメーション中かどうか
		public bool IsAnimation
		{
			get
			{
				if (mOrderDiplayObj != null)
					foreach (var order in mOrderDiplayObj)
						if (order.IsAnimation)
							return true;
				return false;
			}
		}

		// 表示オブジェクト生成
		private void MakeDislayObject(BattleUnit buttleUnit)
		{
			var dispObj = Instantiate(mDisplayObject);
			var rect = dispObj.GetComponent<RectTransform>();
			rect.SetParent(transform);
			var orderDiplayObj = dispObj.GetComponent<OrderDiplayObj>();
			orderDiplayObj.Setup(buttleUnit, mDisplayObject.GetComponent<RectTransform>());
			mOrderDiplayObj.Add(orderDiplayObj);

			var back = dispObj.transform.FindChild("Back");
			if (back)
			{
				var image = back.GetComponent<Image>();
				if (image)
				{
//					image.color = (orderDiplayObj.BattleUnit.IsPlayer ? new Color(0, 0, 1) : new Color(1, 0, 0));
				}
			}
		}

		// 各種セットアップ
		public void Setup(BattleObj battleObj)
		{
			var mBaseRect = mDisplayObject.GetComponent<RectTransform>();
			DisplayOutCount = 0;
			DisplayOutCount = (int)transform.parent.GetComponent<CanvasScaler>().referenceResolution.x;
			DisplayOutCount -= DisplayOutCount / 2 + (int)mBaseRect.localPosition.x;
			DisplayOutCount /= (int)mBaseRect.sizeDelta.x;

			mOrderDiplayObj = new List<OrderDiplayObj>();
			mBattleObj = battleObj;
			foreach (var unit in mBattleObj.PlayerUnits)
				MakeDislayObject(unit);
			foreach (var unit in mBattleObj.EnemyUnits)
				MakeDislayObject(unit);
			foreach (var obo in mOrderDiplayObj)
				obo.BattleUnit.OrderValue = obo.BattleUnit.GetActionOrderValue() - 100;
			mOrderDiplayObj.Sort((a, b) => a.BattleUnit.OrderValue - b.BattleUnit.OrderValue);
			NextOrderValue = TurnUnit.GetActionOrderValue();
			StartCoroutine("CoStartMove");
		}

		// スタート時に移動させるコルーチン
		private IEnumerator CoStartMove()
		{
			var baseRect = mDisplayObject.GetComponent<RectTransform>();
			var baseWidth = baseRect.sizeDelta.x;
			var basePosX = baseRect.localPosition.x;
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
			{
				mOrderDiplayObj[i].SlideIn(i);
				while (mOrderDiplayObj[i].Rect.localPosition.x > basePosX + (DisplayOutCount - 2) * baseWidth)
					yield return null;
			}
			while (IsAnimation)
				yield return null;
		}

		// 戦闘不能時の移動コルーチン
		private IEnumerator CoDeadOut(List<BattleUnit> units)
		{
			List<OrderDiplayObj> removeObjs = new List<OrderDiplayObj>();
			foreach (var unit in units)
			{
				foreach (var odb in mOrderDiplayObj)
				{
					if (odb.BattleUnit == unit)
					{
						odb.SlideUpOut();
						removeObjs.Add(odb);
						break;
					}
				}
			}
			while (IsAnimation)
				yield return null;

			foreach (var odb in removeObjs)
			{
				mOrderDiplayObj.Remove(odb);
			}
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
				mOrderDiplayObj[i].SlideToPos(i);
			while (IsAnimation)
				yield return null;
		}

		public IEnumerator DeadOut(List<BattleUnit> units)
		{
			yield return StartCoroutine("CoDeadOut", units);
		}

		// ターン終了時のコルーチン
		private IEnumerator CoTurnEnd()
		{
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
			{
				if (i == 0)
					mOrderDiplayObj[0].SlideLeftOut();
				else
					mOrderDiplayObj[i].SlideToPos(mOrderDiplayObj[i].Pos - 1);
			}
			while (IsAnimation)
				yield return null;

			var todo = mOrderDiplayObj[0];
			todo.BattleUnit.OrderValue = NextOrderValue;
			mOrderDiplayObj.Remove(todo);
			for (int i = 0; i < mOrderDiplayObj.Count; i++)
			{
				if (mOrderDiplayObj[i].BattleUnit.OrderValue > todo.BattleUnit.OrderValue)
				{
					mOrderDiplayObj.Insert(i, todo);
					for (int j = i + 1; j < mOrderDiplayObj.Count; j++)
					{
						if (todo != mOrderDiplayObj[j])
							mOrderDiplayObj[j].SlideToPos(j);
					}
					while (IsAnimation)
						yield return null;
					todo.SlideUpIn(i);
					break;
				}
				else if (i == mOrderDiplayObj.Count - 1)
				{
					mOrderDiplayObj.Add(todo);
					todo.SlideIn(mOrderDiplayObj.Count - 1);
					break;
				}
			}
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
				mOrderDiplayObj[i].BattleUnit.OrderValue -= 100;
			NextOrderValue = TurnUnit.GetActionOrderValue();
			while (IsAnimation)
				yield return null;
		}

		public IEnumerator TurnEnd()
		{
			yield return StartCoroutine("CoTurnEnd");
		}

		// 召喚ユニット追加時のコルーチン
		private IEnumerator CoPlusUnit(BattleUnit unit)
		{
			// 追加ユニットの生成
			var dispObj = Instantiate(mDisplayObject);
			var rect = dispObj.GetComponent<RectTransform>();
			rect.SetParent(transform);
			var plusUnitObj = dispObj.GetComponent<OrderDiplayObj>();
			plusUnitObj.Setup(unit, mDisplayObject.GetComponent<RectTransform>());
			plusUnitObj.BattleUnit.OrderValue = plusUnitObj.BattleUnit.GetActionOrderValue() - 100;
			// 挿入
			for (int i = 1; i < mOrderDiplayObj.Count; i++)
			{
				if (mOrderDiplayObj[i].BattleUnit.OrderValue > plusUnitObj.BattleUnit.OrderValue)
				{
					mOrderDiplayObj.Insert(i, plusUnitObj);
					for (int j = i + 1; j < mOrderDiplayObj.Count; j++)
					{
						if (plusUnitObj != mOrderDiplayObj[j])
							mOrderDiplayObj[j].SlideToPos(j);
					}
					while (IsAnimation)
						yield return null;
					plusUnitObj.SlideUpIn(i);
					break;
				}
				else if (i == mOrderDiplayObj.Count - 1)
				{
					mOrderDiplayObj.Add(plusUnitObj);
					plusUnitObj.SlideIn(mOrderDiplayObj.Count - 1);
					break;
				}
			}
			// スライドが終わるまで待機
			while (IsAnimation)
				yield return null;
		}

		public IEnumerator PlusUnit(BattleUnit unit)
		{
			yield return StartCoroutine("CoPlusUnit", unit);
		}

		// 順番下げ(単体)のコルーチン
		private IEnumerator CoDownOrderSingle(BattleUnit bu)
		{
			OrderDiplayObj odo = null;
			BattleUnit nextBU = null;
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
			{
				if (bu == mOrderDiplayObj[i].BattleUnit)
				{
					odo = mOrderDiplayObj[i];
					if (mOrderDiplayObj[mOrderDiplayObj.Count - 1].BattleUnit != bu)
						nextBU = mOrderDiplayObj[i + 1].BattleUnit;
					break;
				}
			}
			if (nextBU != null)
			{
				if (bu.OrderValue <= NextOrderValue && nextBU.OrderValue >= NextOrderValue)
				{
					var tmp = NextOrderValue;
					NextOrderValue = bu.OrderValue;
					bu.OrderValue = tmp;
				}
				else
				{
					var tmp = nextBU.OrderValue;
					nextBU.OrderValue = bu.OrderValue;
					bu.OrderValue = tmp;
					// 入れ替え
					odo.SlideUpOut();
					while (IsAnimation)
						yield return null;
					int pos = 0;
					for (; pos < mOrderDiplayObj.Count; pos++)
					{
						if (mOrderDiplayObj[pos].BattleUnit == nextBU)
						{
							mOrderDiplayObj[pos].SlideToPos(pos - 1);
							break;
						}
					}
					while (IsAnimation)
						yield return null;
					odo.SlideUpIn(pos);
					while (IsAnimation)
						yield return null;
					mOrderDiplayObj.Remove(odo);
					mOrderDiplayObj.Insert(pos, odo);
				}
			}
			else if (bu.OrderValue <= NextOrderValue)
			{
				var tmp = NextOrderValue;
				NextOrderValue = bu.OrderValue;
				bu.OrderValue = tmp;
			}
		}

		public IEnumerator DownOrderSingle(BattleUnit bu)
		{
			yield return StartCoroutine("CoDownOrderSingle", bu);
		}

		// 順番下げ(陣営)のコルーチン
		private IEnumerator CoDownOrderAll(bool isPlayer)
		{
			// まだNextOrderを下回る数字が出ていないフラグ
			bool flag = true;
			for (int i = mOrderDiplayObj.Count - 1; i > 0; i--)
			{
				if(i == mOrderDiplayObj.Count - 1)
				{
					if (mOrderDiplayObj[i].BattleUnit.OrderValue <= NextOrderValue)
						flag = false;
					// 最終
					// ターンユニットと最終ユニットが別陣営
					if ((TurnUnit.IsPlayer != mOrderDiplayObj[i].BattleUnit.IsPlayer) &&
						(mOrderDiplayObj[i].BattleUnit.OrderValue <= NextOrderValue) ?
						(mOrderDiplayObj[i].BattleUnit.IsPlayer == isPlayer) :
						(TurnUnit.IsPlayer == isPlayer && mOrderDiplayObj[i - 1].BattleUnit.OrderValue <= NextOrderValue))
					{
						// (NextOrderのほうが大きい&&最終ユニットが対象)||
						//(最終ユニットOrderのほうが大きい&&NextOrderは最終ユニットの1つ前のOrderより大きい&&ターンユニットが対象)
						int order = NextOrderValue;
						NextOrderValue = mOrderDiplayObj[i].BattleUnit.OrderValue;
						mOrderDiplayObj[i].BattleUnit.OrderValue = order;
					}
				}
				else
				{
					// 途中
					if (flag && mOrderDiplayObj[i].BattleUnit.OrderValue <= NextOrderValue)
					{
						// 初めてNextOrderを下回る数字が出た
						flag = false;
						if (TurnUnit.IsPlayer == isPlayer && TurnUnit.IsPlayer != mOrderDiplayObj[i + 1].BattleUnit.IsPlayer)
						{
							// ターンユニットが対象であり、一つ上の対象がターンユニットと別陣営であるならば入れ替える
							int order = NextOrderValue;
							NextOrderValue = mOrderDiplayObj[i + 1].BattleUnit.OrderValue;
							mOrderDiplayObj[i + 1].BattleUnit.OrderValue = order;
						}
					}
					if (mOrderDiplayObj[i].BattleUnit.OrderValue <= NextOrderValue && mOrderDiplayObj[i + 1].BattleUnit.OrderValue > NextOrderValue)
					{
						// NextOrderが途中に挟まっている
						if (mOrderDiplayObj[i].BattleUnit.IsPlayer == isPlayer && TurnUnit.IsPlayer != mOrderDiplayObj[i].BattleUnit.IsPlayer)
						{
							// 対象であり、ターンユニットと陣営が違うなら入れ替える
							int order = NextOrderValue;
							NextOrderValue = mOrderDiplayObj[i].BattleUnit.OrderValue;
							mOrderDiplayObj[i].BattleUnit.OrderValue = order;
						}
					}
					else
					{
						if (mOrderDiplayObj[i].BattleUnit.IsPlayer == isPlayer && mOrderDiplayObj[i].BattleUnit.IsPlayer != mOrderDiplayObj[i + 1].BattleUnit.IsPlayer)
						{
							// 対象であり、一つ上のユニットと陣営が違うなら入れ替える
							int order = mOrderDiplayObj[i + 1].BattleUnit.OrderValue;
							mOrderDiplayObj[i + 1].BattleUnit.OrderValue = mOrderDiplayObj[i].BattleUnit.OrderValue;
							mOrderDiplayObj[i].BattleUnit.OrderValue = order;
						}
					}
				}
			}
			for (int i = mOrderDiplayObj.Count - 1; i >= 0; i--)
				if (mOrderDiplayObj[i].BattleUnit.IsPlayer == isPlayer)
					mOrderDiplayObj[i].SlideUpOut();
			while (IsAnimation)
				yield return null;
			var turnODO = mOrderDiplayObj[0];
			List<OrderDiplayObj> lodo = new List<OrderDiplayObj>();
			for (int i = mOrderDiplayObj.Count - 1; i >= 0; i--)
			{
				if (mOrderDiplayObj[i].BattleUnit.IsPlayer == isPlayer)
				{
					lodo.Insert(0, mOrderDiplayObj[i]);
					mOrderDiplayObj.Remove(mOrderDiplayObj[i]);
				}
			}
			foreach (var odo in lodo)
			{
				for (int i = 1; i < mOrderDiplayObj.Count; i++)
				{
					if (odo.BattleUnit.OrderValue < mOrderDiplayObj[i].BattleUnit.OrderValue)
					{
						mOrderDiplayObj.Insert(i, odo);
						break;
					}
					else if (i == mOrderDiplayObj.Count - 1)
					{
						mOrderDiplayObj.Add(odo);
						break;
					}
				}
			}
			for (int i = 0; i < mOrderDiplayObj.Count; i++)
			{
				if(mOrderDiplayObj[i].BattleUnit.IsPlayer != isPlayer)
				{
					mOrderDiplayObj[i].SlideToPos(i);
				}
			}
			while (IsAnimation)
				yield return null;
			for (int i = 0; i < mOrderDiplayObj.Count; i++)
			{
				if (mOrderDiplayObj[i].BattleUnit.IsPlayer == isPlayer)
				{
					mOrderDiplayObj[i].SlideUpIn(i);
				}
			}
			while (IsAnimation)
				yield return null;
		}

		public IEnumerator DownOrderAll(bool isPlayer)
		{
			yield return StartCoroutine("CoDownOrderAll", isPlayer);
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
