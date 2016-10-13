using UnityEngine;
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
			mOrderDiplayObj.Sort((a, b) => a.BattleUnit.OrderValue - b.BattleUnit.OrderValue);

			if (todo == mOrderDiplayObj[mOrderDiplayObj.Count - 1])
				todo.SlideIn(mOrderDiplayObj.Count - 1);
			else
			{
				int pos = 0;
				for (int i = 0; i < mOrderDiplayObj.Count; ++i)
				{
					if (todo != mOrderDiplayObj[i])
						mOrderDiplayObj[i].SlideToPos(i);
					else
						pos = i;
				}
				while (IsAnimation)
					yield return null;
				todo.SlideUpIn(pos);
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
			MakeDislayObject(unit);
			// 追加ユニットの保持＆行動値計算
			var plusUnitObj = mOrderDiplayObj[mOrderDiplayObj.Count - 1];
			plusUnitObj.BattleUnit.OrderValue = plusUnitObj.BattleUnit.GetActionOrderValue() - 100;
			// ターンユニットオブジェクトの保持
			var turnObj = mOrderDiplayObj[0];
			// ソート
			mOrderDiplayObj.Sort((a, b) => a.BattleUnit.OrderValue - b.BattleUnit.OrderValue);
			// ターンユニットオブジェクトが先頭になっていない場合、先頭に置き直す
			if (turnObj != mOrderDiplayObj[0])
			{
				mOrderDiplayObj.Remove(turnObj);
				mOrderDiplayObj.Insert(0, turnObj);
			}
			// 移動
			if(plusUnitObj == mOrderDiplayObj[mOrderDiplayObj.Count - 1])
			{
				// 追加ユニットが最後尾の場合、追加ユニットを右端からスライドインさせる
				plusUnitObj.SlideIn(mOrderDiplayObj.Count - 1);
			}
			else
			{
				// 追加ユニットが途中の場合
				int pos = 0;
				// 追加ユニットオブジェクト以外を移動位置にスライド
				for (int i = 0; i < mOrderDiplayObj.Count; i++)
				{
					if (mOrderDiplayObj[i] != plusUnitObj)
						mOrderDiplayObj[i].SlideToPos(i);
					else
						pos = i;
				}
				// スライドが終わるまで待機
				while (IsAnimation)
					yield return null;
				// 追加ユニットを上部からスライドインさせる
				plusUnitObj.SlideUpIn(pos);
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
		private IEnumerator CoDownOrder(BattleUnit bu)
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
			if (mOrderDiplayObj[mOrderDiplayObj.Count - 1].BattleUnit != bu)
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
				}
			}
			else if (bu.OrderValue <= NextOrderValue)
			{
				var tmp = NextOrderValue;
				NextOrderValue = bu.OrderValue;
				bu.OrderValue = tmp;
			}
			odo.SlideUpOut();
			while (IsAnimation)
				yield return null;
			mOrderDiplayObj.Sort((a, b) => a.BattleUnit.OrderValue - b.BattleUnit.OrderValue);
			int pos = 0;
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
			{
				if (odo != mOrderDiplayObj[i])
					mOrderDiplayObj[i].SlideToPos(i);
				else
					pos = i;
			}
			while (IsAnimation)
				yield return null;
			odo.SlideUpIn(pos);
			while (IsAnimation)
				yield return null;
		}

		public IEnumerator DownOrder(BattleUnit bu)
		{
			yield return StartCoroutine("CoDownOrder", bu);
		}

		// 順番下げ(陣営)のコルーチン
		private IEnumerator CoDownOrder(bool isPlayer)
		{
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
				if (mOrderDiplayObj[i].BattleUnit.IsPlayer == isPlayer)
					mOrderDiplayObj[i].SlideUpOut();
			while (IsAnimation)
				yield return null;
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
			{
				var odo = mOrderDiplayObj[mOrderDiplayObj.Count - i - 1];
				if (odo.BattleUnit.IsPlayer == isPlayer)
				{
					if (i == 0)
					{
						if (odo.BattleUnit.OrderValue < NextOrderValue && TurnUnit.IsPlayer != isPlayer)
						{
							var tmp = NextOrderValue;
							NextOrderValue = odo.BattleUnit.OrderValue;
							odo.BattleUnit.OrderValue = tmp;
						}
					}
					else
					{
						var odo2 = mOrderDiplayObj[mOrderDiplayObj.Count - i - 2];
						if (odo.BattleUnit.OrderValue < NextOrderValue && TurnUnit.IsPlayer != isPlayer && (odo2.BattleUnit.IsPlayer == isPlayer || odo2.BattleUnit.OrderValue >= NextOrderValue))
						{
							var tmp = NextOrderValue;
							NextOrderValue = odo.BattleUnit.OrderValue;
							odo.BattleUnit.OrderValue = tmp;
						}
						else if (odo2.BattleUnit.IsPlayer != isPlayer)
						{
							var tmp = odo2.BattleUnit.OrderValue;
							odo2.BattleUnit.OrderValue = odo.BattleUnit.OrderValue;
							odo.BattleUnit.OrderValue = tmp;
						}
					}
				}
			}
			var turnODO = mOrderDiplayObj[0];
			mOrderDiplayObj.Sort((a, b) => a.BattleUnit.OrderValue - b.BattleUnit.OrderValue);
			if (mOrderDiplayObj[0] != turnODO)
			{
				mOrderDiplayObj.Remove(turnODO);
				mOrderDiplayObj.Insert(0, turnODO);
			}
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
			{
				if (mOrderDiplayObj[i].BattleUnit.IsPlayer != isPlayer)
					mOrderDiplayObj[i].SlideToPos(i);
			}
			while (IsAnimation)
				yield return null;
			for (int i = 0; i < mOrderDiplayObj.Count; ++i)
			{
				if (mOrderDiplayObj[i].BattleUnit.IsPlayer == isPlayer)
					mOrderDiplayObj[i].SlideUpIn(i);
			}
			while (IsAnimation)
				yield return null;
		}

		public IEnumerator DownOrder(bool isPlayer)
		{
			yield return StartCoroutine("CoDownOrder", isPlayer);
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
