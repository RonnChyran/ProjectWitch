using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System;


namespace ProjectWitch
{
    public class Game : MonoBehaviour
    {
        #region 定数

        //シーン名
        private const string cSceneName_Title = "Title";
        private const string cSceneName_Battle = "Battle";
        private const string cSceneName_PreBattle = "PreBattle";
        private const string cSceneName_Field = "Field";
        private const string cSceneName_Menu = "Menu";
        private const string cSceneName_Save = "Save";
        private const string cSceneName_Load = "Load";
        private const string cSceneName_Talk = "Talk";
        private const string cSceneName_Opening = "Opening";
        private const string cSceneName_Ending = "Ending";

        //読み取り専用プロパティ
        public string SceneName_Title { get { return cSceneName_Title; } private set { } }
        public string SceneName_Battle { get { return cSceneName_Battle; } private set { } }
        public string SceneName_PreBattle { get { return cSceneName_PreBattle; } private set { } }
        public string SceneName_Field { get { return cSceneName_Field; } private set { } }
        public string SceneName_Menu { get { return cSceneName_Menu; } private set { } }
        public string SceneName_Save { get { return cSceneName_Save; } private set { } }
        public string SceneName_Load { get { return cSceneName_Load; } private set { } }
        public string SceneName_Talk { get { return cSceneName_Talk; } private set { } }
        public string SceneName_Opening { get { return cSceneName_Opening; } private set { } }
        public string SceneName_Ending { get { return cSceneName_Ending; } private set { } }

        //毎ターンのHP回復率
        [SerializeField]
        private float mHPRecoveryRate = 0.2f; //20%

        #endregion

        #region 外部システム

        //サウンドマネージャ
        [SerializeField]
        private SoundManager mSoundManager = null;
        public SoundManager SoundManager { get { return mSoundManager; } private set { } }

        //Talkシーンのコマンド
        [SerializeField]
        private TalkCommandHelper mTalkCommand = null;
        public TalkCommandHelper TalkCommand { get { return mTalkCommand; } private set { } }

        //ローディング画面
        [SerializeField]
        private GameObject mNowLoadingPrefab = null;

        //タイトル画面遷移
        [SerializeField]
        private GoTitleWindow mGoTitleWindow = null;
        public GoTitleWindow GoTitle { get { return mGoTitleWindow; } }

        #endregion

        #region ゲームデータ関連

        //実行中のゲーム内データ
        [SerializeField]
        private GameData mGameData = null;
        public GameData GameData { get { return mGameData; } set { mGameData = value; } }
        
        //アプリケーション全体のシステムデータ
        public SystemData SystemData { get; set; }

        #endregion

        #region シーン間データ関連

        public BattleDataIn BattleIn { get; set; }
        public BattleDataOut BattleOut { get; set; }

        public ScenarioDataIn ScenarioIn { get; set; }

        public MenuDataIn MenuDataIn { get; set; }

        public int EndingID { get; set; }

        #endregion

        #region 制御変数

        //ダイアログが開いているか
        public bool IsDialogShowd { get; set; }

        //戦闘中かどうか
        public bool IsBattle { get; set; }

        //スクリプト実行中かどうか
        public bool IsTalk { get; set; }

        //編成画面から戦闘に入るかどうか
        public bool UsePreBattle { get; set; }

        #endregion

        //Singleton
        private static Game mInst;
        public static Game GetInstance()
        {
            if (mInst == null)
            {
                GameObject gameObject = Resources.Load("Prefabs/System/Game") as GameObject;
                mInst = Instantiate(gameObject).GetComponent<Game>();
            }
            return mInst;
        }
        void Awake()
        {
            DontDestroyOnLoad(this.gameObject);

            if (mInst == null)
            {
                this.Setup();
                mInst = this;
            }
            else if (mInst != this)
            {
                Destroy(this.gameObject);
            }
        }

        //初期化処理
        public void Setup()
        {
            //制御変数初期化
            IsDialogShowd = false;
            IsBattle = false;
            IsTalk = false;
            UsePreBattle = true;
            
            //シーン間データの初期化
            BattleIn = new BattleDataIn();
            BattleOut = new BattleDataOut();
            ScenarioIn = new ScenarioDataIn();
            MenuDataIn = new MenuDataIn();
            MenuDataIn.Reset();

            //ゲームデータ初期化
            GameData.Reset();

            //システムデータ初期化
            SystemData = new SystemData();
            SystemData.Reset();
            SystemData.Load();
            
        }

        //ダイアログを表示
        public void ShowDialog(string caption, string message)
        {
            if (IsDialogShowd) return;

            GameObject prefab = (GameObject)Resources.Load("Prefabs/UI/dialog");
            if (!prefab)
                Debug.Log("ダイアログのプレハブが見つかりません");

            //インスタンス化
            var inst = Instantiate(prefab);
            inst.GetComponent<DialogWindow>().Caption = caption;
            inst.GetComponent<DialogWindow>().Text = message;

            //ダイアログ表示
            IsDialogShowd = true;

        }

        //タイトルへ戻る
        public IEnumerator CallTitle()
        {
            ShowNowLoading();
            yield return null;

            yield return SceneManager.LoadSceneAsync(cSceneName_Title);

            HideNowLoading();
        }

        //エンディングを呼び出す
        public IEnumerator CallEnding(int id)
        {
            ShowNowLoading();
            yield return null;

            EndingID = id;
            SceneManager.LoadScene(cSceneName_Ending);
        }

        //フィールドの開始
        public IEnumerator CallField()
        {
            ShowNowLoading();
            yield return null;

            yield return SceneManager.LoadSceneAsync(cSceneName_Field);

            HideNowLoading();
        }

        //メニューの呼び出し
        public IEnumerator CallMenu()
        {
            SceneManager.LoadSceneAsync(cSceneName_Menu, LoadSceneMode.Additive);

            yield return null;
        }

        //戦闘の開始
        public IEnumerator CallPreBattle()
        {

            if (UsePreBattle)
            {
                ShowNowLoading();
                yield return null;

                yield return SceneManager.LoadSceneAsync(cSceneName_PreBattle, LoadSceneMode.Additive);
            }
            else
            {
                //戦闘準備画面を出さず直接戦闘
                UsePreBattle = true;
                yield return StartCoroutine(CallBattle());
            }
        }

        public IEnumerator CallBattle()
        {
            ShowNowLoading();
            yield return null;

            //戦闘情報の格納
            //var time = (CurrentTime <= 2) ? CurrentTime : 2;
            //BattleIn.TimeOfDay = time;

            BattleIn.TimeOfDay = GameData.CurrentTime;

            yield return SceneManager.LoadSceneAsync(cSceneName_Battle, LoadSceneMode.Additive);
            if (SceneManager.GetSceneByName(cSceneName_PreBattle).IsValid())
                yield return SceneManager.UnloadSceneAsync(cSceneName_PreBattle);
        }

        //スクリプトの開始
        public void CallScript(EventDataFormat e)
        {
            ScenarioIn.FileName = e.FileName;

            ScenarioIn.NextA = e.NextA;
            ScenarioIn.NextB = e.NextB;

            IsTalk = true;
            StartCoroutine(_CallScript());

            HideNowLoading();
        }
        private IEnumerator _CallScript()
        {
            yield return SceneManager.LoadSceneAsync(cSceneName_Talk, LoadSceneMode.Additive);
        }

        //ローディング画面を表示
        public void ShowNowLoading()
        {
            if (mNowLoadingPrefab)
                mNowLoadingPrefab.SetActive(true);
        }

        //ローディング画面を非表示
        public void HideNowLoading()
        {
            if (mNowLoadingPrefab)
                mNowLoadingPrefab.SetActive(false);
        }

        //オートセーブする
        public void AutoSave()
        {
            GameData.Save(-1); //0スロットはオートセーブ用スロット
            SystemData.Save();
        }

        //各コマンド

        //地点の領主を変更
        //targetArea:変更する地点ＩＤ
        //newOwner:新しい領主ID
        public void ChangeAreaOwner(int targetArea, int newOwner)
        {
            //領地データのエリア番号を移し替える
            var oldOwner = GameData.Area[targetArea].Owner;
            GameData.Territory[oldOwner].AreaList.Remove(targetArea);
            GameData.Territory[newOwner].AreaList.Add(targetArea);

            //地点の領主番号を更新
            GameData.Area[targetArea].Owner = newOwner;

        }

        //エリアのマナを回復量だけ回復
        public void RecoverMana()
        {
            foreach (var area in GameData.Area)
            {
                //IncrementalMana分だけ、マナを回復
                area.Mana += area.IncrementalMana;
            }
        }

        //ユニットを回復量だけ回復
        public void RecoverUnit()
        {
            foreach (var unit in GameData.Unit)
            {
                //死んでいたらスルー
                if (!unit.IsAlive) continue;

                //ユニットの回復量分兵士数を回復
                //HPは固定比率回復

                //HP回復
                unit.HP += (int)((float)unit.MaxHP * mHPRecoveryRate);
                if (unit.HP > unit.MaxHP) unit.HP = unit.MaxHP;

                //兵数回復
                unit.SoldierNum += unit.Curative;
                if (unit.SoldierNum > unit.MaxSoldierNum) unit.SoldierNum = unit.MaxSoldierNum;

            }
        }



    }
}