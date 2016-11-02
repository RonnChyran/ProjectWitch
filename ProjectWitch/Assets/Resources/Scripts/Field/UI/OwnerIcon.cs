using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Field
{
    public class OwnerIcon : MonoBehaviour
    {
        private Image mcImage;

        //画像のセット
        [SerializeField]
        private Sprite[] mSprites = null;

        // Use this for initialization
        void Start()
        {
            mcImage = GetComponent<Image>();
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();

            //現在行動している領主を求める
            int territory = 0;
            if (game.CurrentTime > 2)
                territory = game.CurrentTime - 2;

            //アウトオブレンジの防止
            if (territory >= game.TerritoryData.Count)
                territory = 0;

            //領主名＋TURNを表示する
            mcImage.sprite = mSprites[territory];
        }
    }
}