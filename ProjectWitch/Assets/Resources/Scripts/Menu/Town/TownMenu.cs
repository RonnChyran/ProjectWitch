using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class TownMenu : MonoBehaviour
    {
        //コントローラへの参照
        [SerializeField]
        private MenuController mController = null;

        //トップメニューへの参照
        [SerializeField]
        private Animator mTopMenu = null;

        //トークイベントへの参照
        [SerializeField]
        private TownTalkEvent[] mTalkEvents = null;

        //component 
        private Animator mcAnim = null;

        //閉じれるかどうか
        public bool Closable { get; set; }

        // Use this for initialization
        void Start()
        {
            mcAnim = GetComponent<Animator>();
            Closable = true;
        }

        // Update is called once per frame
        void Update()
        {
            if (mcAnim.GetBool("IsShow") && Closable && mController.InputEnable)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    Close();
                }
            }
        }

        public void Close()
        {
            StartCoroutine(_Close());
        }

        private IEnumerator _Close()
        {
            //キャンセル音再生
            Game.GetInstance().SoundManager.PlaySE(SE.Cancel);

            mcAnim.SetBool("IsShow", false);
            yield return new WaitForSeconds(0.2f);
            mTopMenu.SetBool("IsShow", true);
            yield return new WaitForSeconds(0.2f);

        }

        public void Click_ToolShop()
        {
            Closable = false;
            SceneManager.LoadScene("ToolShop",LoadSceneMode.Additive);
        }

        public void Click_MagicShop()
        {
            Closable = false;
            SceneManager.LoadScene("MagicShop", LoadSceneMode.Additive);
        }

        public void ExecuteEvent(EventDataFormat e)
        {
            Closable = false;
            StartCoroutine(_EvecuteEvent(e));
        }

        private IEnumerator _EvecuteEvent(EventDataFormat e)
        {
            var game = Game.GetInstance();

            game.CallScript(e);
            yield return null;

            //会話の終了まち
            while (game.IsTalk) yield return null;
            
            //イベント実行済みフラグを折る
            game.GameData.TownEventEnable = false;

            //リセットをかける
            foreach (var te in mTalkEvents)
            {
                te.Reset();
            }
            yield return null;

            //イベントが終わったら少し待つ
            yield return new WaitForSeconds(0.1f);

            //BGM再生
            game.SoundManager.Play(game.GameData.FieldBGM, SoundType.BGM);

            //終了
            Closable = true;

            yield return null;
        }
    }
}