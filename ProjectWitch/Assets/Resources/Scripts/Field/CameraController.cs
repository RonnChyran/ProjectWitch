using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

    //マップの大きさ
    private Vector3 mMapSize;

    void Start()
    {
        var collider = GetComponent<BoxCollider>();
        mMapSize = collider.size/2;
    }

    //変数入力
    private Vector3 mOldMousePos;

    void OnMouseDown()
    {
        //敵ターンのときは無効
        if (Game.GetInstance().CurrentTime >= 3) return;

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
        //敵ターンのときは無効
        if (Game.GetInstance().CurrentTime >= 3) return;

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
        Camera mainCamera = Camera.main;
        Vector3 cameraPos = mainCamera.transform.position;
        float cameraSizeY = mainCamera.orthographicSize;
        float aspect = (float)Screen.width / (float)Screen.height;
        float cameraSizeX = cameraSizeY * aspect;

        if( cameraPos.x < -mMapSize.x + cameraSizeX)
        {
            cameraPos.x = -mMapSize.x + cameraSizeX;
        }
        if( cameraPos.x > mMapSize.x - cameraSizeX)
        {
            cameraPos.x = mMapSize.x - cameraSizeX;
        }
        if(cameraPos.y < -mMapSize.y + cameraSizeY)
        {
            cameraPos.y = -mMapSize.y + cameraSizeY;
        }
        if(cameraPos.y > mMapSize.y - cameraSizeY)
        {
            cameraPos.y = mMapSize.y - cameraSizeY;
        }

        mainCamera.transform.position = cameraPos;

    }
}
