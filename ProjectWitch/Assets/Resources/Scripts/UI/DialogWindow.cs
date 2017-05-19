using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace ProjectWitch
{
    [RequireComponent(typeof(Animator))]
    public class DialogWindow : MonoBehaviour
    {
        //ボックスの中身への参照
        [SerializeField]
        private Text mCaption = null;
        [SerializeField]
        private Text mText = null;

        //表示、非表示時の効果音の指定
        [SerializeField]
        private string mOpenSE = "";
        [SerializeField]
        private string mCloseSE = "";

        //アニメータへの参照
        private Animator mAnim = null;

        //ボックス内の内容
        public string Caption { get; set; }
        public string Text { get; set; }

        public void Start()
        {
            mCaption.text = Caption;
            mText.text = Text;

            mAnim = GetComponent<Animator>();

            //効果音再生
            var game = Game.GetInstance();
            game.SoundManager.Play(mOpenSE, SoundType.SE);
        }

        public void Update()
        {
            //Submitボタンが押されたらクローズ
            if(Input.GetButtonDown("Submit"))
            {
                Close();
            }
        }

        public void EndAnimation()
        {
            Destroy(this.gameObject);
        }

        public void Close()
        {
            //効果音再生
            var game = Game.GetInstance();
            game.SoundManager.Play(mCloseSE, SoundType.SE);

            Game.GetInstance().IsDialogShowd = false;
            mAnim.SetTrigger("close");
        }
    }

}