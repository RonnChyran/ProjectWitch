using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;

namespace ProjectWitch.PreBattle
{
    public class Display : MonoBehaviour
    {
        //コントローラ
        [SerializeField]
        private PreBattleController mController = null;

        //ユニットの戦闘プレハブのパス
        [SerializeField]
        private string mBattlePrefabPath = "Prefabs/Battle/";

        [Header("テキスト")]
        [SerializeField]
        private DisplayText[] mTexts = null;

        [Header("イメージ")]
        [SerializeField]
        private Image[] mImages = null;

        //表示しているユニットID
        private int[] mUnitIDs = null;

        // Use this for initialization
        void Start()
        {
            mUnitIDs = Enumerable.Repeat<int>(-1, 3).ToArray();
            foreach (var image in mImages)
                image.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {
            for(int i=0; i<mUnitIDs.Length; i++)
            {
                //IDが異なっていたら更新
                if(mUnitIDs[i] != mController.UnitList[i])
                {
                    mUnitIDs[i] = mController.UnitList[i];
                    mTexts[i].UnitID = mUnitIDs[i];
                    LoadImage(mUnitIDs[i], i);
                }
            }
        }

        private void LoadImage(int unitID, int target)
        {
            if(unitID == -1)
            {
                mImages[target].enabled = false;
                return;
            }

            var game = Game.GetInstance();
            var unit = game.UnitData[unitID];

            //ユニットのプレハブのデフォルト画像から抜き出す
            var unitprefab = Resources.Load<GameObject>(mBattlePrefabPath + unit.BattleLeaderPrefabPath);
            if (unitprefab == null) return;
            var sprite = unitprefab.GetComponentInChildren<SpriteRenderer>().sprite;

            //イメージをセットしてサイズを調整
            mImages[target].enabled = true;
            mImages[target].sprite = sprite;
            mImages[target].SetNativeSize();

            //位置を調整
            var height = mImages[target].rectTransform.sizeDelta.y;
            var pos = mImages[target].rectTransform.localPosition;
            pos.y = height;
            mImages[target].rectTransform.localPosition = pos;
        }
    }
}