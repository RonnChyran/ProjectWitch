using UnityEngine;
using System.Collections;

namespace ProjectWitch.Field
{
    public class CameraController : MonoBehaviour
    {

        //マップの大きさ
        private Vector3 mMapSize;

        //操作対象のカメラ
        private Camera mCamera;

        //カメラが操作可能か
        public bool IsPlayable { get; set; }

        //目的地に到達するまでの時間
        [SerializeField]
        private float mDuration = 0.5f;

        //カメラの移動用内部変数
        private float mProgress = -1.0f;
        private Vector2 mTargetPos = Vector2.zero;
        private Vector3 mCurrentPos = Vector2.zero;
        
        void Start()
        {
            var collider = GetComponent<BoxCollider>();
            mMapSize = collider.size / 2;

            //デフォルトは操作可能
            IsPlayable = true;

            //カメラを取得
            mCamera = Camera.main;
            
        }

        void Update()
        {
            DeltaMove();
        }

        //変数入力
        private Vector3 mOldMousePos;

        void OnMouseDown()
        {
            var ray = RectTransformUtility.ScreenPointToRay(Camera.main, Input.mousePosition);

            RaycastHit rHit;
            if (Physics.Raycast(ray, out rHit))
            {
                mOldMousePos = rHit.point;
            }
            else
            {
                mOldMousePos = Vector3.zero;
            }
        }

        void OnMouseDrag()
        {
            Vector3 mousePos;
            var ray = RectTransformUtility.ScreenPointToRay(Camera.main, Input.mousePosition);

            RaycastHit rHit;
            if (Physics.Raycast(ray, out rHit))
            {
                mousePos = rHit.point;
            }
            else
            {
                mousePos = Vector3.zero;
            }

            //カメラが操作可能かどうか
            if (!IsPlayable)
            {
                mOldMousePos = mousePos;
                return;
            }

            var tmp = mousePos - mOldMousePos;
            tmp.z = 0.0f;

            Camera.main.transform.position -= tmp;

            //カメラの移動制限
            Vector3 cameraPos = mCamera.transform.position;
            float cameraSizeY = mCamera.orthographicSize;
            float aspect = (float)Screen.width / (float)Screen.height;
            float cameraSizeX = cameraSizeY * aspect;

            if (cameraPos.x < -mMapSize.x + cameraSizeX)
            {
                cameraPos.x = -mMapSize.x + cameraSizeX;
            }
            if (cameraPos.x > mMapSize.x - cameraSizeX)
            {
                cameraPos.x = mMapSize.x - cameraSizeX;
            }
            if (cameraPos.y < -mMapSize.y + cameraSizeY)
            {
                cameraPos.y = -mMapSize.y + cameraSizeY;
            }
            if (cameraPos.y > mMapSize.y - cameraSizeY)
            {
                cameraPos.y = mMapSize.y - cameraSizeY;
            }

            mCamera.transform.position = cameraPos;

        }

        //カメラ移動（ターゲット指定）
        public IEnumerator MoveTo(Vector2 targetpos)
        {
            mTargetPos = targetpos;
            mCurrentPos = mCamera.transform.position;
            mProgress = mDuration;

            while (mProgress >= 0) yield return null;
            yield return null;
        }

        private void DeltaMove()
        {
            if(mProgress >= 0.0f)
            {

                //線形補間で位置を出す
                mCamera.transform.position = new Vector3(mCurrentPos.x * mProgress + mTargetPos.x * (1 - mProgress),
                                                         mCurrentPos.y * mProgress + mTargetPos.y * (1 - mProgress), mCurrentPos.z);

                mProgress -= Time.deltaTime;
            }
        }
    }

}