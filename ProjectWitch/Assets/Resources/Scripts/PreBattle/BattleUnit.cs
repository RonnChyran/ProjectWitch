using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace ProjectWitch.PreBattle
{
    public class BattleUnit : MonoBehaviour,
        IPointerEnterHandler, IPointerExitHandler
    {
        //こんとろーら
        [SerializeField]
        private PreBattleController mController = null;

        //ID
        [SerializeField]
        private int mID = 0;

        //パス
        [SerializeField]
        private string mFaceImagePath = "Textures/Face/";

        //パネル
        [SerializeField]
        private GameObject mPanel = null;

        //顔画像
        [SerializeField]
        private Image mFace = null;

        //表示名
        [SerializeField]
        private Text mName = null;

        //外すボタン
        [SerializeField]
        private GameObject mDetouchButton = null;

        //内部変数
        private int mUnitID = -1;

        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            //unitIDが変化したら更新
            if (mController.UnitList[mID] != mUnitID)
            {
                mUnitID = mController.UnitList[mID];
                Reset();
            }

        }

        //表示を更新
        void Reset()
        {
            if (mController.UnitList[mID] != -1)
            {
                mPanel.SetActive(true);

                var game = Game.GetInstance();
                var unit = game.GameData.Unit[mController.UnitList[mID]];

                //表示の更新
                mName.text = unit.Name;
                var sprite = Resources.Load<Sprite>(mFaceImagePath + unit.FaceIamgePath);
                mFace.sprite = sprite;

                mDetouchButton.SetActive(false);
            }
            else
            {
                mPanel.SetActive(false);
            }
        }

        
        public void OnPointerEnter(PointerEventData e)
        {
            if(mController.UnitList[mID] != -1)
            {
                mDetouchButton.SetActive(true);
            }
        }

        public void OnPointerExit(PointerEventData e)
        {
            if(mController.UnitList[mID] != -1)
            {
                mDetouchButton.SetActive(false);
            }
        }

        public void OnClickedDetouchButton()
        {
            mController.UnitList[mID] = -1;
            mController.UnitSetHistory.HistoryRemove(mID);
        }

    }
}