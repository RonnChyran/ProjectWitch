using UnityEngine;
using System.Collections;

namespace PreBattle
{
    public class UnitList : MonoBehaviour
    {
        //ユニット単体のベース
        [SerializeField]
        private GameObject mUnit;

        //ユニットを縦に二つセットする
        [SerializeField]
        private GameObject mUnitBox;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            var playerUnits = game.TerritoryData[0].UnitList;
            for(int i=0; i<playerUnits.Count; i+=2)
            {
                var unit1 = playerUnits[i];
                var unit2 = ((i + 1) < playerUnits.Count) ? playerUnits[i + 1] : -1;

                //箱の生成
                var box = Instantiate(mUnitBox);
                box.transform.SetParent(transform);
                box.transform.localScale = Vector3.one;

                //要素の生成
                var unitInst1 = Instantiate(mUnit);
                var unitInst2 = (unit2 == -1) ? null : Instantiate(mUnit);

                //Unit1の格納
                unitInst1.GetComponent<DragUnit>().UnitID = unit1;
                unitInst1.transform.SetParent(box.transform);
                unitInst1.transform.localScale = Vector3.one;

                //Unit2の格納
                if (unitInst2)
                {
                    unitInst2.GetComponent<DragUnit>().UnitID = unit2;
                    unitInst2.transform.SetParent(box.transform);
                    unitInst2.transform.localScale = Vector3.one;
                }
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}