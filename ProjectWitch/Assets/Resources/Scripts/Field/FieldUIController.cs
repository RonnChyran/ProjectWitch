using UnityEngine;
using System.Linq; //iOSで問題が起こるかも？
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

namespace ProjectWitch.Field
{

    public class FieldUIController : MonoBehaviour
    {
        //コントローラ
        [SerializeField]
        private FieldController mFieldController = null;

        //非表示にするUI
        [SerializeField]
        private Canvas mPanel = null;

        //ヘルプウィンドウを表示するボタン
        [SerializeField]
        private Button mHelpButton = null;

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
            mBases = Enumerable.Repeat<GameObject>(null, Game.GetInstance().GameData.Area.Count).ToList();

            //拠点設置
            AreaPointReset();

            //道の描画
            AddRoad();

            //内部変数初期化
            SelectedTerritory = -1;
            OwnerPanelLock = false;
        }

        protected virtual void Update()
        {
            if(mFieldController.MenuClickable)
            {
                mHelpButton.interactable = true;

                if (Input.GetButtonDown("Cancel"))
                {
                    ShowMenu();
                }
            }
            else
            {
                mHelpButton.interactable = false;
            }
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
            
            for (int i = 1; i < game.GameData.Area.Count; i++)
            {
                AddAreaPoint(i, game.GameData.Area[i].Owner);
            }
        }

        private void AddAreaPoint(int area, int owner)
        {
            var game = Game.GetInstance();

            //旗画像ロード
            var path = mFlagTexFolderPath + game.GameData.Territory[owner].FlagTexName;
            var sprite = Resources.Load<Sprite>(path);

            var Base = Instantiate(mBasePrefab);
            Base.transform.SetParent(mFlagCanvas.transform);
            Base.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(game.GameData.Area[area].Position.x, game.GameData.Area[area].Position.y, 1.0f);
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

            Openlist.Add(game.GameData.Area[1]);

            while (Openlist.Count != 0)
            {
                //オープンリストとクローズリストにない隣接地点の検出
                for (int i = 0; i < game.GameData.Area[current].NextArea.Count; i++)
                {
                    child = game.GameData.Area[current].NextArea[i];

                    bool a = Openlist.Contains(game.GameData.Area[child]);
                    bool b = Closelist.Contains(game.GameData.Area[child]);
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
                    var target = game.GameData.Area[current].NextArea;
                    //オープンリスト内の親以外の点で隣り合っている点をすべて返す（配列）
                    foreach (int a in target)
                    {
                        if (a != 0)//aが０じゃないなら線を引く
                            DrawLine(new Vector3(game.GameData.Area[current].Position.x, game.GameData.Area[current].Position.y, mLineDepth),
                                new Vector3(game.GameData.Area[a].Position.x, game.GameData.Area[a].Position.y, mLineDepth));
                    }
                    Closelist.Add(game.GameData.Area[current]);
                    Openlist.Remove(game.GameData.Area[current]);

                    if (Openlist.Count == 0)
                        break;

                    current = Openlist[0].ID;

                }
                else
                {  //子供がいれば
                    DrawLine(new Vector3(game.GameData.Area[current].Position.x, game.GameData.Area[current].Position.y, mLineDepth),
                        new Vector3(game.GameData.Area[child].Position.x, game.GameData.Area[child].Position.y, mLineDepth));
                    bool c = Openlist.Contains(game.GameData.Area[current]);

                    if (c == false)
                        Openlist.Add(game.GameData.Area[current]);

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

        //UIを表示
        public void ShowUI()
        {
            var anim = mPanel.GetComponent<Animator>();
            anim.SetBool("IsShow", true);

            mFieldController.MenuClickable = true;
        }

        //UIを隠す
        public void HideUI()
        {
            var anim = mPanel.GetComponent<Animator>();
            anim.SetBool("IsShow", false);
        }

        //メニューを表示
        public void ShowMenu()
        {
            mFieldController.MenuClickable = false;
            StartCoroutine(_ShowMenu());
        }
        private IEnumerator _ShowMenu()
        {
            HideUI();
            yield return new WaitForSeconds(0.3f);
            yield return StartCoroutine(Game.GetInstance().CallMenu());

        }
    }
}