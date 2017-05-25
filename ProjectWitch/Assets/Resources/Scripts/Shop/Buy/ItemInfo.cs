using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Shop
{
    [RequireComponent(typeof(Animator))]
    public class ItemInfo:MonoBehaviour
    {
        //アニメータへの参照
        private Animator mAnimator = null;

        //各コンポーネントへの参照
        [SerializeField]
        private Text mName = null;
        [SerializeField]
        private Text mHP = null;
        [SerializeField]
        private Text mLPAtk = null;
        [SerializeField]
        private Text mLPDef = null;
        [SerializeField]
        private Text mLMAtk = null;
        [SerializeField]
        private Text mLMDef = null;
        [SerializeField]
        private Text mGPAtk = null;
        [SerializeField]
        private Text mGPDef = null;
        [SerializeField]
        private Text mGMAtk = null;
        [SerializeField]
        private Text mGMDef = null;
        [SerializeField]
        private Text mSpeed = null;
        [SerializeField]
        private Text mLeader = null;
        [SerializeField]
        private Text mCur = null;

        [SerializeField]
        private Button mButton = null;

        [SerializeField]
        private NextManaWindow mNextManaWindow = null;

        //メッセージボックス
        [SerializeField]
        private MessageBox mMessageBox = null;

        //購入時に表示するメッセージ
        [SerializeField]
        private string mMesName = "";
        [SerializeField,Multiline]
        private string mMessage = "";

        //リストへの参照
        [SerializeField]
        private BuyList mList = null;

        public int ItemID { get; set; }

        public void Start()
        {
            mAnimator = GetComponent<Animator>();

            ItemID = -1;
            Reset();
        }

        public void Reset()
        {
            if (ItemID != -1)
            {
                var game = Game.GetInstance();
                var item = game.GameData.Equipment[ItemID];

                mName.text = item.Name;
                mHP.text = item.MaxHP.ToString();
                mLPAtk.text = item.LeaderPAtk.ToString();
                mLPDef.text = item.LeaderPDef.ToString();
                mLMAtk.text = item.LeaderMAtk.ToString();
                mLMDef.text = item.LeaderMDef.ToString();
                mGPAtk.text = item.GroupPAtk.ToString();
                mGPDef.text = item.GroupPDef.ToString();
                mGMAtk.text = item.GroupMAtk.ToString();
                mGMDef.text = item.GroupMDef.ToString();
                mSpeed.text = item.Agility.ToString();
                mLeader.text = item.Leadership.ToString();
                mCur.text = item.Curative.ToString();

                var mana = game.GameData.PlayerMana - item.BuyingPrice;
                mNextManaWindow.SetMana(mana);

                if (mana > 0) mButton.interactable = true;
                else mButton.interactable = false;

                mAnimator.SetBool("IsShow", true);

            }
            else
                mAnimator.SetBool("IsShow", false);
        }

        public void Close()
        {
            ItemID = -1;
            Reset();
        }

        public void ClickBuyButton()
        {
            //プレイヤーのデータに装備データを入れる
            var game = Game.GetInstance();
            game.GameData.Territory[0].EquipmentList[ItemID].Add(-1);

            //マナを減らす
            game.GameData.PlayerMana -= game.GameData.Equipment[ItemID].BuyingPrice;

            //メッセージを表示
            mMessageBox.SetText(mMesName, mMessage);

            //データをリセット
            mList.Reset();

            Close();
        }
    }
}
