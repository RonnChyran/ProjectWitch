//author	: masanori.k
//version	: 1.0.0
//summary	: InputNumberのEditor拡張
//HowToUse	: 直下フォルダ「Editor」の中に入れる

using UnityEditor;

namespace MK.Utility
{
	[CustomEditor(typeof(InputNumber))]
	public class InputNumberEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			InputNumber input = target as InputNumber;
			input.Type = (InputNumber.NumberType)EditorGUILayout.EnumPopup("Type", input.Type);
			EditorGUILayout.HelpBox("ScrollValueを0にするとスクロール時の変動が無効化されます", MessageType.Info, true);
			input.IsUseMin = EditorGUILayout.Toggle("IsUseMin", input.IsUseMin);
			if (input.IsUseMin)
			{
				if (input.Type == InputNumber.NumberType.Integer)
					input.MinInt = EditorGUILayout.IntField("Min", input.MinInt);
				else if (input.Type == InputNumber.NumberType.Float)
					input.MinFloat = EditorGUILayout.FloatField("Min", input.MinFloat);
			}
			input.IsUseMax = EditorGUILayout.Toggle("IsUseMax", input.IsUseMax);
			if (input.IsUseMax)
			{
				if (input.Type == InputNumber.NumberType.Integer)
					input.MaxInt = EditorGUILayout.IntField("Max", input.MaxInt);
				else if (input.Type == InputNumber.NumberType.Float)
					input.MaxFloat = EditorGUILayout.FloatField("Max", input.MaxFloat);
			}
			if (input.Type == InputNumber.NumberType.Integer)
				input.ScrollValueInt = EditorGUILayout.IntField("ScrollValue", input.ScrollValueInt);
			else if (input.Type == InputNumber.NumberType.Float)
				input.ScrollValueFloat = EditorGUILayout.FloatField("ScrollValue", input.ScrollValueFloat);
			EditorUtility.SetDirty(target);
		}
	}
}