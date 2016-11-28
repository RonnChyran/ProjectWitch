using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.Help
{
    public class TopMenu : MonoBehaviour
    {
        [Header("ボタン")]
        [SerializeField]
        private Button mFieldButton = null;
        [SerializeField]
        private Button mMenuButton = null;
        [SerializeField]
        private Button mBattleButton = null;

        //ヘルプウィンドウコントローラ
        public HelpWindow HelpWindow { get; set; }

        // Use this for initialization
        void Start()
        {
            //ボタンの動作を連結
            mFieldButton.onClick.AddListener(HelpWindow.OnClicked_Field);
            mMenuButton.onClick.AddListener(HelpWindow.OnClicked_Menu);
            mBattleButton.onClick.AddListener(HelpWindow.OnClicked_Battle);
        }
    }
}