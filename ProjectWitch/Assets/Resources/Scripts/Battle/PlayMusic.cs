using UnityEngine;
using System.Collections;


namespace Battle
{
	public class PlayMusic : MonoBehaviour
	{
		// 戦闘開始時○○VS○○を表示したとき
		[SerializeField]
		private string mVS = null;
		// スキルボタンを選択したとき
		[SerializeField]
		private string mOverSkillButton = null;
		// スキルボタンをクリックしたとき
		[SerializeField]
		private string mClickSkillButton = null;
		// 順番送り（時間が減ったタイミング）
		[SerializeField]
		private string mNextTurn = null;
		// 戦闘終了（勝利）
		[SerializeField]
		private string mWin = null;
		// 戦闘終了（敗北）
		[SerializeField]
		private string mLose = null;
		// コンフィグのスライダーの移動音
		[SerializeField]
		private string mMoveConfigSlider = null;

		public void PlayAudio(string name)
		{
            var game = Game.GetInstance();
            if (name != "")
                game.SoundManager.Play(name, SoundType.SE);
		}

        public void StopBGM()
        {
            var game = Game.GetInstance();
            game.SoundManager.Stop(SoundType.BGM);
        }

		public void PlayVS()
		{
			PlayAudio(mVS);
		}

		public void PlayOverSkillButton()
		{
			PlayAudio(mOverSkillButton);
		}

		public void PlayClickSkillButton()
		{
			PlayAudio(mClickSkillButton);
		}

		public void PlayNextTurn()
		{
			PlayAudio(mNextTurn);
		}

		public void PlayWin()
		{
            StopBGM();
			PlayAudio(mWin);
		}

		public void PlayLose()
		{
            StopBGM();
			PlayAudio(mLose);
		}

		public void PlayMoveConfigSlider()
		{
			PlayAudio(mMoveConfigSlider);
		}

		// Use this for initialization
		void Start()
        {
            var game = Game.GetInstance();
            game.SoundManager.Play(game.BattleIn.BGM, SoundType.BGM);
        }
		// Update is called once per frame
		void Update() {}
	}
}
