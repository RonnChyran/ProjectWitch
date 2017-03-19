//author	: masanori.k
//version	: 1.1.0
//summary	: InputFieldに最大値、最小値設定、スクロールで数値変更できるようにするコンポーネント
//HowToUse	: InputFieldに貼り付けるだけ

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MK.Utility
{
	[RequireComponent(typeof(InputField))]
	public class InputNumber : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
	{
		public enum NumberType
		{
			Integer = 0,
			Float,
		}
		[SerializeField]
		private NumberType m_Type = NumberType.Integer;
		public NumberType Type
		{
			get { return m_Type; }
			set
			{
				m_Type = value;
				if (value == NumberType.Integer)
					InputField.contentType = InputField.ContentType.IntegerNumber;
				else if (value == NumberType.Float)
					InputField.contentType = InputField.ContentType.DecimalNumber;
			}
		}
		// 最小値、最大値を使用するかどうか
		[SerializeField]
		private bool m_IsUseMin = false, m_IsUseMax = false;
		public bool IsUseMin { get { return m_IsUseMin; } set { m_IsUseMin = value; } }
		public bool IsUseMax { get { return m_IsUseMax; } set { m_IsUseMax = value; } }
		// 下限、上限、スクロールによる変動値(0にすると無効化)
		[SerializeField]
		private int m_MinI = 0, m_MaxI = 100, m_ScrollValueI = 1;
		[SerializeField]
		private float m_MinF = 0, m_MaxF = 1, m_ScrollValueF = 0.01f;
		public int MinInt
		{
			get { return m_MinI; }
			set
			{
				if (!IsUseMax || value <= MaxInt)
				{
					m_MinI = value;
					if (ValueInt < value)
						InputField.text = m_MinI.ToString();
				}
			}
		}
		public float MinFloat
		{
			get { return m_MinF; }
			set
			{
				if (!IsUseMax || value <= MaxFloat)
				{
					m_MinF = value;
					if (ValueFloat < value)
						InputField.text = m_MinF.ToString();
				}
			}
		}
		public int MaxInt
		{
			get { return m_MaxI; }
			set
			{
				if (!IsUseMin || value >= MinInt)
				{
					m_MaxI = value;
					if (ValueInt > value)
						InputField.text = m_MaxI.ToString();
				}
			}
		}
		public float MaxFloat
		{
			get { return m_MaxF; }
			set
			{
				if (!IsUseMin || value >= MinFloat)
				{
					m_MaxF = value;
					if (ValueFloat > value)
						InputField.text = m_MaxF.ToString();
				}
			}
		}
		public int ScrollValueInt { get { return m_ScrollValueI; } set { m_ScrollValueI = value; } }
		public float ScrollValueFloat { get { return m_ScrollValueF; } set { m_ScrollValueF = value; } }

		// テキストを数値として取得、テキストが数値でなかった場合、数値を設定
		public int ValueInt
		{
			get
			{
				int num;
				if (int.TryParse(InputField.text, out num))
				{
					if (IsUseMin && num < MinInt)
					{
						InputField.text = MinInt.ToString();
						return MinInt;
					}
					else if (IsUseMax && num > MaxInt)
					{
						InputField.text = MaxInt.ToString();
						return MaxInt;
					}
					else
						return num;
				}
				else
				{
					num = (IsUseMin ? MinInt : IsUseMax && MaxInt < 0 ? MaxInt : 0);
					InputField.text = num.ToString();
					return num;
				}
			}
			set
			{
				if (IsUseMin && value < MinInt)
					InputField.text = MinInt.ToString();
				else if (IsUseMax && value > MaxInt)
					InputField.text = MaxInt.ToString();
				else
					InputField.text = value.ToString();
			}
		}
		public float ValueFloat
		{
			get
			{
				float num;
				if (float.TryParse(InputField.text, out num))
				{
					if (IsUseMin && num < MinFloat)
					{
						InputField.text = MinFloat.ToString();
						return MinFloat;
					}
					else if (IsUseMax && num > MaxFloat)
					{
						InputField.text = MaxFloat.ToString();
						return MaxFloat;
					}
					else
						return num;
				}
				else
				{
					num = (IsUseMin ? MinFloat : IsUseMax && MaxFloat < 0 ? MaxFloat : 0);
					InputField.text = num.ToString();
					return num;
				}
			}
			set
			{
				if (IsUseMin && value < MinFloat)
					InputField.text = MinFloat.ToString();
				else if (IsUseMax && value > MaxFloat)
					InputField.text = MaxFloat.ToString();
				else
					InputField.text = value.ToString();
			}
		}

		// InputField参照
		public InputField InputField { get { return gameObject.GetComponent<InputField>(); } }
		// 範囲内にマウスカーソルが入っているかどうか
		public bool IsEnter { get; private set; }

		// マウスオーバ状態を監視する関数群
		public void OnPointerEnter(PointerEventData eventData) { IsEnter = true; }
		public void OnPointerExit(PointerEventData eventData) { IsEnter = false; }

		// 初期化
		void Awake()
		{
			IsEnter = false;
			Type = Type;
		}

		void Update()
		{
			if ((Type == NumberType.Integer ? m_ScrollValueI != 0 : m_ScrollValueF != 0) && InputField.interactable && IsEnter)
			{
				var scroll = Input.GetAxis("Mouse ScrollWheel");
				if (scroll != 0)
				{
					if (Type == NumberType.Integer)
						ValueInt = (scroll < 0 ? ValueInt - ScrollValueInt : ValueInt + ScrollValueInt);
					else if (Type == NumberType.Float)
						ValueFloat = (scroll < 0 ? ValueFloat - ScrollValueFloat : ValueFloat + ScrollValueFloat);
					// OnEndEditに登録されているものを呼び出し
					InputField.onEndEdit.Invoke(InputField.text);
				}
			}
		}
	}
}
