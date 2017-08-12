using UnityEngine;
using System.Collections;

namespace ProjectWitch.Menu
{
    public class BaseMenu:MonoBehaviour
    {
        //コントローラへの参照
        [SerializeField]
        protected MenuController mController = null;

        //トップメニューへの参照
        [SerializeField]
        protected Animator mTopMenu = null;

        //component 
        protected Animator mcAnim = null;

        //閉じれるかどうか
        public bool Closable { get; set; }

        // Use this for initialization
        protected virtual void Start()
        {
            mcAnim = GetComponent<Animator>();
            Closable = true;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (mcAnim.GetBool("IsShow") && Closable && mController.InputEnable)
            {
                if (Input.GetButtonDown("Cancel"))
                {
                    Close();
                }
            }
        }

        public virtual void Close()
        {
            StartCoroutine(_Close());
        }

        protected virtual IEnumerator _Close()
        {
            //キャンセル音再生
            Game.GetInstance().SoundManager.PlaySE(SE.Cancel);

            mcAnim.SetBool("IsShow", false);
            yield return new WaitForSeconds(0.2f);
            mTopMenu.SetBool("IsShow", true);
            yield return new WaitForSeconds(0.2f);

        }
    }
}
