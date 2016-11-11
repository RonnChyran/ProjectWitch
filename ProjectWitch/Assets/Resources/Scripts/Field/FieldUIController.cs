using UnityEngine;
using System.Linq; //iOSで問題が起こるかも？
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

using GameData;

namespace Field
{

    public class FieldUIController : MonoBehaviour
    {
        ////自動操作時のカメラスピード
        //[SerializeField]
        //private float mCameraSpeed = 1.0f;
        [SerializeField]
        private float mLineDepth = 90.0f;

        //領地ハイライトのエフェクト
        [SerializeField]
        private GameObject mHiLightEffect=null;
        private bool mEffectEnable = false;

        [SerializeField]
        private GameObject mFlagCanvas = null;

        [SerializeField]
        private GameObject mCameraCanvas = null;
        public GameObject CameraCanvas { get { return mCameraCanvas; } private set { } }

        [SerializeField]
        private GameObject mLine=null;

        //旗のプレハブ
        [SerializeField]
        private GameObject mBasePrefab = null;

        //旗の画像フォルダパス
        [SerializeField]
        private string mFlagTexFolderPath = null;

        //ベースへの参照
        private List<GameObject> mBases = new List<GameObject>();


        //現在行動している領地
        public int ActiveTerritory { get; set; }
        //現在選択している領地
        public int SelectedTerritory { get; set; }
        //オーナーパネルロックフラグ
        public bool OwnerPanelLock { get; set; }


        void Start()
        {
            //リストの初期化
            mBases = Enumerable.Repeat<GameObject>(null, Game.GetInstance().AreaData.Count).ToList();

            //拠点設置
            AreaPointReset();

            //道の描画
            AddRoad();

            //内部変数初期化
            SelectedTerritory = -1;
            OwnerPanelLock = false;
        }

        void Update()
        {
        }

        //エリアのハイライトエフェクト表示
        public IEnumerator ShowHiLightEffect(Vector3 targetPos)
        {
            var inst = Instantiate(mHiLightEffect);
            inst.transform.position = targetPos;
            inst.GetComponent<FXController>().EndEvent.AddListener(EndEffect);
            mEffectEnable = true;

            //エフェクト終了まで待つ
            while (mEffectEnable) yield return null;

            yield return null;
        }

        private void EndEffect()
        {
            mEffectEnable = false;
        }

        //拠点の設置
        public void AreaPointReset()
        {
            var game = Game.GetInstance();
            
            for (int i = 1; i < game.AreaData.Count; i++)
            {
                AddAreaPoint(i, game.AreaData[i].Owner);
            }
        }

        private void AddAreaPoint(int area, int owner)
        {
            var game = Game.GetInstance();

            //旗画像ロード
            var path = mFlagTexFolderPath + game.TerritoryData[owner].FlagTexName;
            var sprite = Resources.Load<Sprite>(path);

            var Base = Instantiate(mBasePrefab);
            Base.transform.SetParent(mFlagCanvas.transform);
            Base.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(game.AreaData[area].Position.x, game.AreaData[area].Position.y, 1.0f);
            Base.GetComponent<Image>().sprite = sprite;
            Base.GetComponent<FlagButton>().AreaID = area;
            Base.GetComponent<FlagButton>().FieldUIController = this;

            mBases[area] = Base;
        }

        //拠点の変更
        public void ChangeAreaOwner(int targetArea, int newOwner)
        {
            //拠点を再配置
            Destroy(mBases[targetArea]);
            AddAreaPoint(targetArea, newOwner);
        }

        //道の描画
        private void AddRoad()
        {
            var Openlist = new List<AreaDataFormat>();
            var Closelist = new List<AreaDataFormat>();
            int current = 1;
            int child = 0;

            var game = Game.GetInstance();

            Openlist.Add(game.AreaData[1]);

            while (Openlist.Count != 0)
            {
                //オープンリストとクローズリストにない隣接地点の検出
                for (int i = 0; i < game.AreaData[current].NextArea.Count; i++)
                {
                    child = game.AreaData[current].NextArea[i];

                    bool a = Openlist.Contains(game.AreaData[child]);
                    bool b = Closelist.Contains(game.AreaData[child]);
                    if (a == false && b == false)
                        break;
                    else
                    {
                        child = 0;
                    }
                }


                //隣接する地点がない時
                if (child == 0)
                {
                    var target = game.AreaData[current].NextArea;
                    //オープンリスト内の親以外の点で隣り合っている点をすべて返す（配列）
                    foreach (int a in target)
                    {
                        if (a != 0)//aが０じゃないなら線を引く
                            DrawLine(new Vector3(game.AreaData[current].Position.x, game.AreaData[current].Position.y, mLineDepth),
                                new Vector3(game.AreaData[a].Position.x, game.AreaData[a].Position.y, mLineDepth));
                    }
                    Closelist.Add(game.AreaData[current]);
                    Openlist.Remove(game.AreaData[current]);

                    if (Openlist.Count == 0)
                        break;

                    current = Openlist[0].ID;

                }
                else
                {  //子供がいれば
                    DrawLine(new Vector3(game.AreaData[current].Position.x, game.AreaData[current].Position.y, mLineDepth),
                        new Vector3(game.AreaData[child].Position.x, game.AreaData[child].Position.y, mLineDepth));
                    bool c = Openlist.Contains(game.AreaData[current]);

                    if (c == false)
                        Openlist.Add(game.AreaData[current]);

                    current = child;
                }
            }
        }

        //線の描画
        private void DrawLine(Vector3 pointA, Vector3 pointB)
        {
            if (!mLine) return;

            var inst = Instantiate(mLine);
            inst.transform.SetParent(mFlagCanvas.transform);
            inst.GetComponent<LineRenderer>().SetPositions(new Vector3[] { pointA, pointB });
        }
    }
}