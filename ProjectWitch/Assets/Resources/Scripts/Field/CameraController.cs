using UnityEngine;
using System.Collections;

namespace Field
{
    public class CameraController : MonoBehaviour
    {

        //マップの大きさ
        private Vector3 mMapSize;

        //操作対象のカメラ
        private Camera mCamera;

        //カメラが操作可能か
        public bool IsPlayable { get; set; }

        //自動操縦カメラのスピード
        [SerializeField]
        private float mAutoCameraSpeed = 1.0f;

        void Start()
        {
            var collider = GetComponent<BoxCollider>();
            mMapSize = collider.size / 2;

            //デフォルトは操作可能
            IsPlayable = true;

            //カメラを取得
            mCamera = Camera.main;

            //フィールドコントローラに自身をセット
            var fieldCtrl = GameObject.FindWithTag("FieldController");
            fieldCtrl.GetComponent<FieldController>().CameraController = this;
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
            var dir = targetpos - (Vector2)mCamera.transform.position;
            var moveVec = dir.normalized;
            while (dir.magnitude > mAutoCameraSpeed/50.0f)
            {
                mCamera.transform.position += new Vector3(moveVec.x, moveVec.y, 0.0f) / 100.0f * mAutoCameraSpeed;
                dir = targetpos - (Vector2)mCamera.transform.position;
                moveVec = dir.normalized;
                yield return new WaitForSeconds(0.01f);
            }

            yield return null;
        }
    }

}