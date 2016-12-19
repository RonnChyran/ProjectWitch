using UnityEngine;
using System.Collections;

namespace ProjectWitch.Battle
{
    public class TalkCommandHelper : MonoBehaviour
    {
		[SerializeField]
		private BattleObj mBattleObj = null;
		// 顔グラ
		[SerializeField]
		private FaceObj mFaceP0 = null, mFaceP1 = null, mFaceP2 = null, mFaceE0 = null, mFaceE1 = null, mFaceE2 = null;

		// ターゲットの顔グラオブジェクトの取得
		private FaceObj GetFaceObj(int target)
		{
			switch (target)
			{
				case 0:
					return mFaceP0;
				case 1:
					return mFaceP1;
				case 2:
					return mFaceP2;
				case 3:
					return mFaceE0;
				case 4:
					return mFaceE1;
				case 5:
					return mFaceE2;
				default:
					return null;
			}

		}

		//スキルボタンを表示する
		//target:p0~p2に0~2 e0~e2に3~5と割り振る
		//error: エラーメッセージ格納用
		public void ShowSkillButton(int target, out string error)
        {
			FaceObj face = GetFaceObj(target);
            error = null;
			if (face != null)
				face.OnPointerEnterAction();
			else
				error = "インスペクタで割り当てられていません";

		}

		//スキルボタンを非表示にする
		public void HideSkillButton(out string error)
        {
			error = null;
			if (mFaceP0 != null)
				mFaceP0.SetAllFaceHideButton();
			else
				error = "インスペクタで割り当てられていません";
        }

        //スキルを実行する
        //target: ShowSkillButtonと同様の割り振り
        //type: 0.攻撃 1.防御 2.捕獲
        public void ExecuteSkill(int target, int type, out string error)
        {
			FaceObj face = GetFaceObj(target);
			error = null;
			if (face != null)
			{
				switch (type)
				{
					case 0:
						face.AttackButton.PushButton();
						break;
					case 1:
						face.DefenseButton.PushButton();
						break;
					case 2:
						face.CaptureButton.PushButton();
						break;
					default:
						error = "typeの値が対応した値ではありません";
						break;
				}
			}
			else
				error = "インスペクタで割り当てられていません";
		}

		// 一時停止する
		// isPause==trueで一時停止
		public void PauseBattle(bool isPause)
		{
			if (mBattleObj != null)
				mBattleObj.IsPause = isPause;
		}
	}
}