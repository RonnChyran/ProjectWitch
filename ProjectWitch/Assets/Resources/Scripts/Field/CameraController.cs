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

        void Start()
        {
            var collider = GetComponent<BoxCollider>();
            mMapSize = collider.size / 2;

            //デフォルトは操作可能
            IsPlayable = true;

            //カメラを取得
            mCamera = Camera.main;
            
        }

        //変数入力
        private Vector3 mOldMousePos;

        void OnMouseDown()
        {
            //カメラが操作可能かどうか
            if (!IsPlayable) return;

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
            //カメラが操作可能かどうか
            if (!IsPlayable) return;

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
            var pastpos = mCamera.transform.position;

            var progress = mDuration;

            while (progress >= 0.0f)
            {

                //線形補間で位置を出す
                mCamera.transform.position = new Vector3(pastpos.x * progress + targetpos.x * (1 - progress),
                                                         pastpos.y * progress + targetpos.y * (1 - progress),0.0f);
                
                progress -= Time.deltaTime;
                yield return null;
            }

            yield return null;
        }
    }

}