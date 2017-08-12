using UnityEngine;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class ArmyMenu : BaseMenu
    {
        //ユニットリストへの参照
        [SerializeField]
        private UnitList mUnitList = null;
        public UnitList UnitList { get { return mUnitList; }  private set { } }

        //ステータスウィンドウへの参照
        [SerializeField]
        private StatusWindow mStatusWindow = null;

        protected override IEnumerator _Close()
        {
            //キャンセル音再生
            Game.GetInstance().SoundManager.PlaySE(SE.Cancel);

            //ステータスウィンドウを閉じる
            mStatusWindow.UnitID = -1;
            mStatusWindow.Init();

            mcAnim.SetBool("IsShow", false);
            yield return new WaitForSeconds(0.2f);
            mTopMenu.SetBool("IsShow", true);
            yield return new WaitForSeconds(0.2f);

        }
    }
}