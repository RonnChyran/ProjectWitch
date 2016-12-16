using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class MenuController : MonoBehaviour
    {
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

        //各アニメーターへの参照
        [SerializeField]
        private Animator mAnimTop = null;

        [SerializeField]
        private Animator mAnimCommon = null;

        //内部変数
        private Field.FieldController mFController = null;       

        public void Start()
        {
            mFController = GameObject.FindWithTag("FieldController").GetComponent<Field.FieldController>();
        }

        //メニューを閉じる
        public void Close()
        {
            var game = Game.GetInstance();
            game.SoundManager.PlaySE(SE.Cancel);

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
    }
}