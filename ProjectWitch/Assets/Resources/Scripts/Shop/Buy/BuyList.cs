using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Shop
{
    public class BuyList : MonoBehaviour
    {
        //リストのコンテンツへの参照
        [SerializeField]
        private GameObject mListContent = null;

        //コンテンツの親
        [SerializeField]
        private GameObject mListContentParent = null;

        //情報ウィンドウへの参照
        [SerializeField]
        private ItemInfo mInfoWindow = null;

        //メッセージボックスへの参照
        [SerializeField]
        private MessageBox mMessageBox = null;

        // Use this for initialization
        void Start()
        {
            Reset();
        }

        // Update is called once per frame
        void Update()
        {
        }

        public void Reset()
        {
            var game = Game.GetInstance();

            //子供を全削除
            foreach(Transform child in mListContentParent.transform)
            {
                Destroy(child.gameObject);
            }

            //子供を追加
            for(int i=0; i< game.GameData.Equipment.Count; i++)
            {
                var inst = Instantiate(mListContent);
                var cp = inst.GetComponent<BuyItem>();
                cp.ItemID = i;
                cp.InfoWindow = mInfoWindow;
                cp.MesBox = mMessageBox;
                cp.Reset();

                inst.transform.SetParent(mListContentParent.transform,false);
            }
        }
    }
}