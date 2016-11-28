using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Field
{

    public class OwnerPanel : MonoBehaviour {

        [SerializeField]
        private Text mActionCountText = null;

        [SerializeField]
        private float mBackPos = 0.0f;

        [SerializeField]
        private int mTerritoryID = 0;
        public int TerritoryID { get { return mTerritoryID; } private set { } }

        [SerializeField]
        private float mSpeed = 0.0f;

        //状態変数（パネルが前に出ているかどうか）
        public bool IsActive { get; set; }

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
            if (IsActive)
            {
                targetX += mBackPos;
            }
            DeltaMove(targetX);

            //テキスト更新
            var game = Game.GetInstance();
            var ter = game.GameData.Territory[TerritoryID];
            mActionCountText.text = ter.ActionCount.ToString();
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