using UnityEngine;
using System.Collections;

namespace Field
{

    public class OwnerPanel : MonoBehaviour {

        [SerializeField]
        private float mBackPos = 0.0f;

        [SerializeField]
        private int mTerritoryID = 0;

        [SerializeField]
        private float mSpeed = 0.0f;

        [SerializeField]
        private FieldUIController mcFUICtrl = null;

        //内部変数
        private RectTransform mcRect;
        private float basePosX;

        // Use this for initialization
        void Start() {
            mcRect = GetComponent<RectTransform>();
            basePosX = mcRect.localPosition.x;
        }

        // Update is called once per frame
        void Update() {

            var targetX = basePosX;

            //選択されている領地が自分のとき
            if (mcFUICtrl.SelectedTerritory == mTerritoryID ||
                mcFUICtrl.ActiveTerritory == mTerritoryID)
            {
                targetX += mBackPos;
            }
            DeltaMove(targetX);
        }

        //ほんのちょっとtargetPosに向かって進む
        void DeltaMove(float targetX)
        {
            var pos = transform.localPosition;

            var delta = targetX - transform.localPosition.x;
            if (Mathf.Abs(delta) <= mSpeed * Time.deltaTime)
            {
                pos.x = targetX;
                mcRect.localPosition = pos;
                return;
            }

            delta = Mathf.Abs(delta) / delta;

            pos.x += delta * mSpeed * Time.deltaTime;
            mcRect.localPosition = pos;
        }
    }
}