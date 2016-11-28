using UnityEngine;

namespace ProjectWitch.Field
{
    public class MapColor : MonoBehaviour
    {
        //コンポーネント
        private SpriteRenderer mcSpriteRenderer;

        //基準の色
        [SerializeField]
        private Color[] mColor = new Color[4];

        //現在の時間
        private int mCurrentTime;

        //変化のための線形補完係数
        private float mLeapFactor=0.0f;

        // Use this for initialization
        void Start()
        {
            var game = Game.GetInstance();

            mCurrentTime = game.GameData.CurrentTime;

            //コンポーネントの取得
            mcSpriteRenderer = GetComponent<SpriteRenderer>();

            //最初の色を時間から決定
            var targetColor = (mCurrentTime < 3) ?
                mColor[mCurrentTime] : mColor[3];
            mcSpriteRenderer.color = targetColor;
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();

            //現在の色を保持
            var currentColor = mcSpriteRenderer.color;
            var targetColor = (mCurrentTime < 3) ?
                mColor[mCurrentTime] : mColor[3];

            //色を変化
            currentColor = Color.Lerp(currentColor, targetColor, mLeapFactor += 0.003f);
            mcSpriteRenderer.color = currentColor;

            if (mLeapFactor > 1.0f) mLeapFactor = 1.0f;

            if (mCurrentTime != game.GameData.CurrentTime)
            {
                mCurrentTime = game.GameData.CurrentTime;
                mLeapFactor = 0.0f;
            }
        }
    }
}