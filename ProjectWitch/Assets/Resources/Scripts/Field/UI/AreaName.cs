using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

namespace ProjectWitch.Field
{
    public class AreaName : MonoBehaviour
    {
        //名前表示用テキストコンポーネント
        [SerializeField]
        private Text mcText = null;

        //地点ID
        public int AreaID { get; set; }

        public AreaName()
        {
            AreaID = -1;
        }
        
        public void Init()
        {
            if (AreaID == -1) return;

            var game = Game.GetInstance();

            try
            {
                var data = game.AreaData[AreaID];

                if (mcText)
                    mcText.text = data.Name;
                else
                    throw new ProjectWitchException("テキストコンポーネントを付けてください");

            }
            catch(ArgumentException)
            {
                Debug.LogError("AreaIDの引数が不正です");
            }
            catch(ProjectWitchException e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
}