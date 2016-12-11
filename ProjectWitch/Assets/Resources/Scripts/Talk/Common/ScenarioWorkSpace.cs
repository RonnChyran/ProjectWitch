//==========================================================
//	author	:shotta
//	summary	:コマンドを走らせて、操作を受け取るまでする部分
//==========================================================

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

using ProjectWitch.Talk.Compiler;
using ProjectWitch.Talk.Command;
using ProjectWitch.Talk.WorkSpace;

namespace ProjectWitch.Talk.WorkSpace
{
	//シナリオ操作時のワークスペース
	public class ScenarioWorkSpace : MonoBehaviour
	{
		//スクリプトフォルダのパス
		[SerializeField]
		private string mScenarioPath = "";

		//コマンドを読み込む
		void Start (){
			VirtualMachine vm = null;

			string name = Game.GetInstance ().ScenarioIn.FileName;
			string path = mScenarioPath + name;
			ScriptCompiler compiler = new ScriptCompiler();
			vm = compiler.CompileScript (path);

			if (vm == null) return;

			this.SetCommandDelegaters (vm);
			mTextWorkSpace.SetCommandDelegaters (vm);
			mGraphicsWorkSpace.SetCommandDelegaters (vm);
			mSoundsWorkSpace.SetCommandDelegaters (vm);
			mGameWorkSpace.SetCommandDelegaters (vm);
            mFieldWorkSpace.SetCommandDelegaters(vm);

            vm.AddNotification("scriptBegin", new NotifyMethod(mGameWorkSpace.ScriptBegin));
			vm.AddNotification ("scriptEnd", new NotifyMethod(mGameWorkSpace.ScriptEnd));
			mVirtualMachine = vm;
		}

		// Update is called once per frame
		void Update ()
		{
			if (mVirtualMachine == null) return;

			bool stepNextFlag = false;
			float mouseWheel = Input.GetAxis("TalkNext");
			bool isNext = Input.GetButtonDown("TalkNext");
			stepNextFlag |= (mouseWheel<0 || isNext);

			bool skipFlag = Input.GetButton("TalkSkip");
			if (Updater is WaitUpdater)
			{
				if ((Updater as WaitUpdater).Time < mSkipWaitDuration)
					skipFlag = false;
			}
			stepNextFlag |= skipFlag;

			if (Input.GetButtonDown ("TalkAuto"))
				mAutoMode ^= true; 
			bool autoFlag = false;
			if (Updater is WaitUpdater)
			{
				bool isFinishAllExp = true;
				isFinishAllExp &= (Updater as WaitUpdater).Time >= mAutoWaitDuration;
				isFinishAllExp &= !mSoundsWorkSpace.IsPlayingSEAndVoice ();
				if (isFinishAllExp)
					autoFlag = mAutoMode;
			}
			stepNextFlag |= autoFlag;

			bool isWaiting = Updater is WaitUpdater;

			//ここで次ステップへの処理を行う
			if (stepNextFlag&&isWaiting)
			{
				mSoundsWorkSpace.StopVoice ();
				SetUpdater(null);
				stepNextFlag = false;
			}

			//ここで次のUpdaterまでプログラムを進める
			//Updaterが来たらそこでループを抜ける
			while (true) {
				//ここを適切に入れ替えればSkipできるはず
				if (!stepNextFlag) {
					if (Updater != null)
						break;
				} else {
					if (Updater is WaitUpdater)
						break;
				}

				SetUpdater(null);
				if (!mVirtualMachine.RunCommand ())
					break;
			}

			//Updaterの更新
			if (Updater != null)
			{
				Updater.Update (Time.deltaTime);
				if (!Updater.IsActive)
					SetUpdater(null);
			}
		}

		//アップデータコマンドはここに入る
		private UpdaterFormat mUpdater;
		public UpdaterFormat Updater{
			get{ return mUpdater; }
		}
		public void SetUpdater(UpdaterFormat updater)
		{
			if (mUpdater != null)
				mUpdater.Finish ();
			mUpdater = updater;
			if (mUpdater != null)
				mUpdater.Setup ();
		}

		//コマンドを実行する仮想フィールド
		private VirtualMachine mVirtualMachine;

		//テキスト関連のフィールド
		[SerializeField]
		private TextWorkSpace mTextWorkSpace = null;

		//グラフィック関係のフィールド
		[SerializeField]
		private GraphicsWorkSpace mGraphicsWorkSpace = null;

        //サウンド関係のフィールド
        [SerializeField]
        private SoundsWorkSpace mSoundsWorkSpace = null;

        //ゲームシステム関係のフィールド
        [SerializeField]
        private GameWorkSpace mGameWorkSpace = null;

        //フィールドとの連携関係
        [SerializeField]
        private FieldWorkSpace mFieldWorkSpace = null;

        [SerializeField]
        //次の会話をスキップする前にこの時間分だけ待つ
        private float mSkipWaitDuration = 0.01f;
        [SerializeField]
        //自動で次の会話に遷移する前にこの時間分だけ待つ
        private float mAutoWaitDuration = 0.01f;
		[SerializeField]
		//自動遷移モードフラグ
		private bool mAutoMode;

		public void SetCommandDelegaters(VirtualMachine vm)
		{
			//新しいアップデータを取り込む
			vm.AddCommandDelegater (
				"SetUpdater",
				new CommandDelegater(false, 1, delegate(object[] arguments){
					SetUpdater(arguments[0] as UpdaterFormat);
					return null;
				}));
			
			//シーン関係
			//新しいシーンと入れ替える
			vm.AddCommandDelegater (
				"ReplaceScene",
				new CommandDelegater(false, 1, delegate(object[] arguments) {
					string error;
					string sceneName = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error; 
					SceneManager.LoadScene (sceneName, LoadSceneMode.Single);
					return null;
				}));
			//新しいシーンを挿入
			vm.AddCommandDelegater (
				"AddScene",
				new CommandDelegater(false, 1, delegate(object[] arguments) {
					string error;
					string sceneName = Converter.ObjectToString(arguments[0], out error);
					if (error != null) return error; 
					SceneManager.LoadScene (sceneName, LoadSceneMode.Additive);
					return null;
				}));
			//自分のシーンを引き剥がす
			vm.AddCommandDelegater (
				"RemoveScene",
				new CommandDelegater (false, 0, delegate(object[] arguments) {
					string name = SceneManager.GetActiveScene ().name;
					SceneManager.UnloadScene (name);
					return null;
				}));
		}
	}

	//アップデータの中に組み込めるコマンド
	abstract public class UpdaterFormat
	{
		public bool IsActive{get{ return mIsActive; } }
		private bool mIsActive = true;
		protected void SetActive (bool isActive)
		{
			mIsActive = isActive;
		}
		abstract public void Setup ();
		abstract public void Update(float deltaTime);
		abstract public void Finish();
	}

	//待機コマンド(ユーザーの操作を待機)
	public class WaitUpdater : UpdaterFormat{
		public float Time{ get; private set; }
		public override void Setup ()
		{
			Time = 0.0f;
		}
		public override void Update(float deltaTime)
		{
			Time += deltaTime;
		}
		public override void Finish(){}
	}
	//待機コマンド(一定の処理が完了するまで待機)
	public class PauseUpdater : UpdaterFormat{
		public override void Setup (){}
		public override void Update(float deltaTime)
		{
			SetActive (false);
		}
		public override void Finish(){}
	}
}