using UnityEngine;
using System.Collections;

namespace Field
{
    //フィールドの旗ボタン、メニューボタンの親となるクラス
    public class FieldButtonBase : MonoBehaviour
    {

        //field UI controller
        protected FieldController mFieldController;

        //生成したメニューのインスタンス
        protected GameObject mInst;

        //Start
        protected virtual void Start()
        {
            //フィールドコントローラを取得
            var obj = GameObject.FindWithTag("FieldController");
            mFieldController = obj.GetComponent<FieldController>();
        }
        
        //メニューを表示する
        protected virtual void ShowMenu(GameObject menu)
        {
            //メニュープレハブを生成
            mInst = Instantiate(menu);
        }

        //メニューを消す
        protected virtual void HideMenu()
        {
            if(mInst)
            {
                mInst.GetComponent<MenuBase>().Close();
            }
        }
    }
}