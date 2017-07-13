using UnityEngine;
using System.Collections;

namespace ProjectWitch.Field
{
    public class CameraController : MonoBehaviour
    {

        //マップの大きさ
        private Vector3 mMapSize;

        //操作対象のカメラ
        [SerializeField]
        private Camera mMoveCamera = null;

        //全体表示のカメラ
        [SerializeField]
        private Camera mOverLookCamera = null;

        //カメラが操作可能か
        public bool IsPlayable { get; set; }

        //全体表示になっているか
        public bool IsOverlook { get; set; }

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
                        
        }

        void Update()
        {
            DeltaMove();
        }

        //カメラ切り替え true:全体表示にする false:縮小ビューにする
        public void ChangeOverLookCamera(bool enable)
        {
            mMoveCamera.enabled = !enable;
            mOverLookCamera.enabled = enable;
        }

        //変数入力
        private Vector3 mOldMousePos;

        void OnMouseDown()
        {
            var ray = RectTransformUtility.ScreenPointToRay(mMoveCamera, Input.mousePosition);

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
            var ray = RectTransformUtility.ScreenPointToRay(mMoveCamera, Input.mousePosition);

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

            mMoveCamera.transform.position -= tmp;

            //カメラの移動制限
            Vector3 cameraPos = mMoveCamera.transform.position;
            float cameraSizeY = mMoveCamera.orthographicSize;
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

            mMoveCamera.transform.position = cameraPos;

        }

        //カメラ移動（ターゲット指定）
        public IEnumerator MoveTo(Vector2 targetpos)
        {
            mTargetPos = targetpos;
            mCurrentPos = mMoveCamera.transform.position;
            mProgress = mDuration;

            while (mProgress >= 0) yield return null;
            yield return null;
        }

        private void DeltaMove()
        {
            if(mProgress >= 0.0f)
            {

                //線形補間で位置を出す
                mMoveCamera.transform.position = new Vector3(mCurrentPos.x * mProgress + mTargetPos.x * (1 - mProgress),
                                                         mCurrentPos.y * mProgress + mTargetPos.y * (1 - mProgress), mCurrentPos.z);

                mProgress -= Time.deltaTime;
            }
        }
    }

}