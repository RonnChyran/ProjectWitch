using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class TownMenu : BaseMenu
    {

        //トークイベントへの参照
        [SerializeField]
        private TownTalkEvent[] mTalkEvents = null;

        protected override IEnumerator _Close()
        {
            var game = Game.GetInstance();

            //キャンセル音再生
            game.SoundManager.PlaySE(SE.Cancel);

            //BGMを戻す
            game.SoundManager.Play(game.GameData.FieldBGM, SoundType.BGM);

            mcAnim.SetBool("IsShow", false);
            yield return new WaitForSeconds(0.2f);
            mTopMenu.SetBool("IsShow", true);
            yield return new WaitForSeconds(0.2f);

        }

        public void Click_ToolShop()
        {
            //BGMを変更
            var game = Game.GetInstance();
            game.SoundManager.Play(game.GameData.ShopBGM, SoundType.BGM);

            Closable = false;
            SceneManager.LoadScene("ToolShop",LoadSceneMode.Additive);
        }

        public void Click_MagicShop()
        {
            //BGMを変更
            var game = Game.GetInstance();
            game.SoundManager.Play(game.GameData.ShopBGM, SoundType.BGM);

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
            game.SoundManager.Play(game.GameData.TownBGM, SoundType.BGM);

            //終了
            Closable = true;

            yield return null;
        }
    }
}