using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class MenuController : MonoBehaviour
    {
        [Header("各メニューへの参照")]
        //各メニューへの参照
        [SerializeField]
        private TopMenu mTopMenu = null;
        public TopMenu TopMenu { get { return mTopMenu; } private set { } }

        [SerializeField]
        private ArmyMenu mArmyMenu = null;
        public ArmyMenu ArmyMenu { get { return mArmyMenu; } private set { } }

        [SerializeField]
        private TalkCommandHelper mTalkCommandHelper = null;
        public TalkCommandHelper TalkCommandHelper { get { return mTalkCommandHelper; } private set { } }

        [Header("Animator")]
        //各アニメーターへの参照
        [SerializeField]
        private Animator mAnimTop = null;

        [SerializeField]
        private Animator mAnimCommon = null;

        [Header("Other")]
        //チュートリアルのシナリオ名
        [SerializeField]
        private string mTutorialName = "s9806";

        //内部変数
        private Field.FieldController mFController = null;       

        public void Start()
        {
            mFController = GameObject.FindWithTag("FieldController").GetComponent<Field.FieldController>();

            //チュートリアルの開始
            if(Game.GetInstance().MenuDataIn.TutorialMode)
                StartTutorial();
        }

        //メニューを閉じる
        public void Close()
        {
            var game = Game.GetInstance();
            game.SoundManager.PlaySE(SE.Cancel);
            game.MenuDataIn.Reset();

            StartCoroutine(_Close());
        }
        private IEnumerator _Close()
        {
            mAnimTop.SetBool("IsShow", false);
            mAnimCommon.SetBool("IsShow", false);

            yield return new WaitForSeconds(0.3f);

            mFController.FieldUIController.ShowUI();
            SceneManager.UnloadScene(Game.GetInstance().SceneName_Menu);

        }

        public void Update()
        {
            mFController.MenuClickable = false;

        }

        private void StartTutorial()
        {
            var game = Game.GetInstance();
            var e = new EventDataFormat();
            e.FileName = mTutorialName;

            game.CallScript(e);
        }
    }
}