using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectWitch
{
    public class DragAndDropBase : MonoBehaviour
        , IDragHandler, IBeginDragHandler, IEndDragHandler
    {
        //識別用ID
        [SerializeField]
        private int mID = 0;
        public int ID { get { return mID; } set { mID = value; } }

        //components
        protected CanvasGroup mcCanvasGroup;

        //内部変数
        protected GameObject mDragObject;

        //ドラッグ開始
        virtual public void OnBeginDrag(PointerEventData e)
        {
            //ドラッグオブジェクトの作成
            CreateDragObject();

            mDragObject.transform.position = e.position;

            //レイキャストを貫通させる
            mcCanvasGroup.blocksRaycasts = false;
        }

        //ドラッグ中
        virtual public void OnDrag(PointerEventData e)
        {
            if (mDragObject)
                mDragObject.transform.position = e.position;
        }

        //ドラッグ終了
        virtual public void OnEndDrag(PointerEventData e)
        {
            mcCanvasGroup.blocksRaycasts = true;

            Destroy(mDragObject);
        }

        // Use this for initialization
        virtual protected void Start()
        {
            mcCanvasGroup = GetComponent<CanvasGroup>();
        }

        // Update is called once per frame
        virtual protected void Update()
        {
        }

        //ドラッグしている間のオブジェクトを生成
        virtual protected void CreateDragObject()
        {
            //自身を複製
            mDragObject = this.gameObject;
            var inst = Instantiate(mDragObject);

            //親をキャンバスに
            var canvas = GameObject.Find("Canvas");
            inst.transform.SetParent(canvas.transform);

            //表示順を一番手前に
            inst.transform.SetAsLastSibling();

            //本来のサイズに戻す
            var image = inst.GetComponent<Image>();
            image.SetNativeSize();

            //例キャストがブロックされないようにする
            var canvasGroup = inst.GetComponent<CanvasGroup>();
            canvasGroup.blocksRaycasts = false;

            mDragObject = inst;
        }
    }

}