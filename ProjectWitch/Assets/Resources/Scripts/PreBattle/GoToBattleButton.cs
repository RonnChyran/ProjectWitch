using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace ProjectWitch.PreBattle
{
    public class GoToBattleButton : MonoBehaviour {

        [SerializeField]
        private PreBattleController mController = null;

        //コンポーネント
        private Button mcButton;

        // Use this for initialization
        void Start() {
            mcButton = GetComponent<Button>();
        }

        // Update is called once per frame
        void Update() {
            if(mController.UnitList[0] == -1 &&
                mController.UnitList[1] == -1 &&
                mController.UnitList[2] == -1)
            {
                mcButton.interactable = false;
            }
            else
            {
                mcButton.interactable = true;
            }
        }
    }
}