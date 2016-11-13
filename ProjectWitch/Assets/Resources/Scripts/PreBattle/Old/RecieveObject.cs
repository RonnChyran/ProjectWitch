using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace PreBattleOld
{
    public class RecieveObject : DragAndDropBase,
        IDropHandler
    {
        //0:前陣 1:中陣 2:後陣
        [SerializeField]
        private int mPosition;
        public int Position { get { return mPosition; } set { mPosition = value; } }

        private Image mIconImage;
        private Sprite mDefSprite;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();

            mIconImage = GetComponent<Image>();
            mDefSprite = mIconImage.sprite;
        }

        //ドロップした時の処理
        public virtual void OnDrop(PointerEventData e)
        {
            var dragUnit = e.pointerDrag.GetComponent<DragAndDropBase>();
            if (dragUnit == null) return;

            //ドラッグオブジェクトのIDが一致していることを確認
            var id = dragUnit.ID;
            if (id == ID)
            {
                Image droppedImage = e.pointerDrag.GetComponent<Image>();
                mIconImage.sprite = droppedImage.sprite;
            }
        }

        //ドラッグ開始時の処理
        public override void OnBeginDrag(PointerEventData e)
        {
            if (mIconImage.sprite != mDefSprite)
            {
                base.OnBeginDrag(e);

                mcCanvasGroup.alpha = 0.5f;
            }
        }

        //ドラッグ時の処理
        public override void OnDrag(PointerEventData e)
        {
            if (mIconImage.sprite != mDefSprite)
                base.OnDrag(e);
        }

        //ドラッグ終了時の処理
        public override void OnEndDrag(PointerEventData e)
        {
            if (mIconImage.sprite != mDefSprite)
            {
                base.OnEndDrag(e);
                mIconImage.sprite = mDefSprite;

                mcCanvasGroup.alpha = 1.0f;
            }
        }

        protected override void CreateDragObject()
        {
            base.CreateDragObject();

            //0.8倍に縮小
            mDragObject.transform.localScale = new Vector3(0.8f, 0.8f, 1.0f);

        }

    }

}