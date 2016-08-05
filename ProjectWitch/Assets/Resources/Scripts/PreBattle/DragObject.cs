using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

namespace PreBattle
{
    public class DragObject : DragAndDropBase
    {

        //ドラッグ済みかどうか
        public bool IsDragged { get; set; }
        public bool IsDragging { get; set; }

        protected override void Start()
        {
            base.Start();

            IsDragged = false;
            IsDragging = false;

      }

        protected override void Update()
        {
            base.Update();

            //無効判定
            if (IsDragged || IsDragging)
            {
                mcCanvasGroup.alpha = 0.5f;
            }
            else
            {
                mcCanvasGroup.alpha = 1.0f;
            }
        }

        //ドラッグ開始
        public override void OnBeginDrag(PointerEventData e)
        {
            //ドラッグ後は無効にする
            if (!IsDragged)
            {
                base.OnBeginDrag(e);

                //ドラッグオブジェクトを透明にする
                IsDragging = true;
            }
        }

        //ドラッグ終了
        public override void OnEndDrag(PointerEventData e)
        {
            base.OnEndDrag(e);

            IsDragging = false;
        }

        //ドラッグしている間のオブジェクトを生成
        protected override void CreateDragObject()
        {
            base.CreateDragObject();

            //0.5倍に縮小
            mDragObject.transform.localScale = new Vector3(0.5f, 0.5f, 1.0f);
        }
        
    }
}