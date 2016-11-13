using UnityEngine;
using System.Collections;

namespace Menu
{
    public class UnitList : MonoBehaviour
    {

        [SerializeField]
        private GameObject mContentGroup = null;

        [SerializeField]
        private GameObject mUnitPrefab = null;

        [SerializeField]
        private StatusWindow mStatusWindow = null;

        // Use this for initialization
        void Start()
        {
            UnitSet();
        }

        // Update is called once per frame
        void Update()
        {

        }

        //リストにコンテンツをセットする
        void UnitSet()
        {
            var game = Game.GetInstance();

            var territory = game.TerritoryData[0];
            var group = game.GroupData[territory.GroupList[0]];
            foreach(var unitid in group.UnitList)
            {
                //コンテンツを追加
                var inst = Instantiate(mUnitPrefab);
                var cUnit = inst.GetComponent<Unit>();
                cUnit.UnitID = unitid;
                cUnit.StatusWindow = mStatusWindow;
                inst.transform.SetParent(mContentGroup.transform, false);
            }
        }
    }
}