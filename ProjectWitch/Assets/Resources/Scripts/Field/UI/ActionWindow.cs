using UnityEngine;
using System.Collections;

namespace Field
{
    public class ActionWindow : MonoBehaviour {

        //コントローラへの参照
        [SerializeField]
        private FieldUIController mcFUICtrl = null;

        //各領主パネルへの参照
        [SerializeField]
        private OwnerPanel[] mOwnerPanels = null;
        

        //攻撃アイコン
        [SerializeField]
        private GameObject mAttackIcon = null;

        //防御アイコン
        [SerializeField]
        private GameObject mDefenseIcon = null;

        //内部変数
        private bool mIsRunnning = false;

        // Use this for initialization
        void Start() {

        }

        // Update is called once per frame
        void Update() {
            if (!mIsRunnning)
                StartCoroutine(_Update());
        }

        //処理軽減のために一定間隔をあけて処理を行う
        private IEnumerator _Update()
        {
            mIsRunnning = true;

            mDefenseIcon.SetActive(false);
            foreach (var panel in mOwnerPanels)
            {
                //領地が防衛になっているとき
                if (mcFUICtrl.SelectedTerritory == panel.TerritoryID &&
                    mcFUICtrl.ActiveTerritory != panel.TerritoryID)
                {
                    panel.IsActive = true;

                    //防衛アイコンを移動
                    mDefenseIcon.SetActive(true);
                    var pos = mDefenseIcon.transform.position;
                    pos.y = panel.transform.position.y;
                    mDefenseIcon.transform.position = pos;
                }
                //領地が攻撃側になっているとき
                else if (mcFUICtrl.ActiveTerritory == panel.TerritoryID)
                {
                    panel.IsActive = true;

                    //攻撃アイコンを移動
                    var pos = mAttackIcon.transform.position;
                    pos.y = panel.transform.position.y;
                    mAttackIcon.transform.position = pos;
                }
                //どっちにもなっていない
                else
                {
                    panel.IsActive = false;
                }
            }

            yield return new WaitForSeconds(0.2f);

            mIsRunnning = false;
        }
    }

}
