using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class HilightBoxReciever : MonoBehaviour
    {
        //現在選択しているインデックス番号
        private int mSelectedIndex = -1;
        public int SelectedIndex { get { return mSelectedIndex; } set { mSelectedIndex = value; } }

        //インフォテキスト
        [SerializeField]
        protected Text mcText = null;

        //ＯＫボタン
        [SerializeField]
        protected Button mcButton = null;

        //コンテナへの参照
        private SaveLoadContainerBase mContainer = null;
        public SaveLoadContainerBase Container { get { return mContainer; } set { mContainer = value; } }
    }
}