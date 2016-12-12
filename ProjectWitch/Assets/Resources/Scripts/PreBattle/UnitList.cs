using UnityEngine;
using System.Collections.Generic;

namespace ProjectWitch.PreBattle
{

    public class UnitList : MonoBehaviour
    {

        [SerializeField]
        private GameObject mContentGroup = null;

        [SerializeField]
        private GameObject mUnitPrefab = null;

        [SerializeField]
        private PreBattleController mController = null;

        //プロパティ
        public List<Unit> UnitComponentList { get; set; }

        // Use this for initialization
        void Start()
        {
            UnitComponentList = new List<Unit>();
            UnitSet();
        }

        //リストにコンテンツをセットする
        void UnitSet()
        {
            var game = Game.GetInstance();

            var territory = game.GameData.Territory[0];
            var group = game.GameData.Group[territory.GroupList[0]];
            foreach (var unitid in group.UnitList)
            {
                //コンテンツを追加
                var inst = Instantiate(mUnitPrefab);
                var cUnit = inst.GetComponent<Unit>();
                cUnit.UnitID = unitid;
                cUnit.Controller = mController;
                inst.transform.SetParent(mContentGroup.transform, false);

                UnitComponentList.Add(cUnit);
            }
        }
    }
}