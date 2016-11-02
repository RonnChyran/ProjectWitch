using UnityEngine;
using UnityEngine.UI;
using System.Collections;

using GameData;

namespace Field
{
    public class InfoBoxIcon : MonoBehaviour
    {
        [SerializeField]
        private int mTerritoryID=0;

        [SerializeField]
        private Color mDisableColor = Color.black;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            //領地が占領済みならそのまま、占領していないなら色を変更
            if(game.TerritoryData[mTerritoryID].State !=
                TerritoryDataFormat.TerritoryState.Dead)
            {
                var cImage = GetComponent<Image>();
                cImage.color = mDisableColor;
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}