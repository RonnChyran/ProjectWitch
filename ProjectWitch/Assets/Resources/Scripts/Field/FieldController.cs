using UnityEngine;
using System;
using System.IO;
using System.Text;
using System.Collections.Generic;

class FieldController : MonoBehaviour
{
    [SerializeField]
    private Canvas mCanvas;
    private Canvas mCanvasInst;

    [SerializeField]
    private GameObject[] mFlag = new GameObject[9];

    [SerializeField]
    private GameObject mLine;

    //メニューが開いているかどうか
    public bool OpeningMenu{ get; set; }

    //構造体制作
    List<Basedata> Data;

    //線生成各種設定
    GameObject mPrehub;
    //変数設定
    struct Basedata
        : IEquatable<Basedata>
    {
        public int number;
        public string name;
        public float positionX;
        public float positionY;
        public int holder;
        public int keepmana;
        public int maxmana;
        public int[] nextregion;

        public bool Equals(Basedata y)
        {
            return name == y.name;
        }

    }






    void Start()
    {
        Basedata box = new Basedata();
        Data = new List<Basedata>();

        //データ読み込み
        string filePath = "Assets\\Resources\\Data\\area_data.csv";
        StreamReader reader = new StreamReader(filePath, Encoding.GetEncoding("Shift_JIS"));
        while (reader.Peek() >= 0)
        {
            string[] cols = reader.ReadLine().Split(',');

            //箱入れ作業
            int Skip = 0;
            int.TryParse(cols[0], out Skip);
            if (Skip > 0)
            {
                box.number = Convert.ToInt32(cols[0]);
                box.name = cols[1];
                box.positionX = float.Parse(cols[2]);
                box.positionY = float.Parse(cols[3]);
                box.holder = int.Parse(cols[4]);
                box.maxmana = int.Parse(cols[5]);
                box.keepmana = int.Parse(cols[6]);
                box.nextregion = new int[7];
                for (int n = 7; n < cols.Length; n++)
                {
                    int existing = 0;
                    int.TryParse(cols[n], out existing);

                    if (existing > 0)
                    {

                        box.nextregion[n - 7] = Convert.ToInt32(cols[n]);
                    }
                }

            }

            Data.Add(box);


            Console.WriteLine(Data);

        }

        reader.Close();

        //拠点設置
        AddAreaPoint();

        //道の描画
        AddRoad();



    }

    //拠点の設置
    void AddAreaPoint()
    {
        mCanvasInst = Instantiate(mCanvas);
        for (int i = 1; i < 105; i++)
        {
            var Base = Instantiate(mFlag[Data[i].holder]);
            Base.transform.SetParent(mCanvasInst.transform);
            Base.GetComponent<RectTransform>().anchoredPosition3D = new Vector3(Data[i].positionX, Data[i].positionY, 1.0f);
        }
    }

    //道の描画
    private void AddRoad()
    {
        List<Basedata> Openlist = new List<Basedata>();
        List<Basedata> Closelist = new List<Basedata>();
        int current=1;
        int child=0;
        
        Openlist.Add(Data[1]);

        while (Openlist.Count != 0)
        {
            //オープンリストとクローズリストにない隣接地点の検出
            for (int i = 0; i < 8; i++)
            {
                if (i < 7)
                {
                    child = Data[current].nextregion[i];
                    //System.Predicate<Basedata> u = FuncA;
                    //Basedata data = Openlist.Find(u);
                    bool a = Openlist.Contains(Data[child]);
                    bool b = Closelist.Contains(Data[child]);
                    if (a == false && b == false)
                        break;
                    else
                    {
                        child = 0;
                    }
                }
            }


            //隣接する地点がない時
            if (child == 0)
            {
                var target = Data[current].nextregion;
                //オープンリスト内の親以外の点で隣り合っている点をすべて返す（配列）
                foreach (int a in target)
                {
                    if (a != 0)//aが０じゃないなら線を引く
                        DrawLine(new Vector3(Data[current].positionX, Data[current].positionY, 2.0f), new Vector3(Data[a].positionX, Data[a].positionY, 2.0f));
                }
                Closelist.Add(Data[current]);
                Openlist.Remove(Data[current]);

                if (Openlist.Count == 0)
                    break;

                current = Openlist[0].number;

            }
            else
            {  //子供がいれば
                DrawLine(new Vector3(Data[current].positionX, Data[current].positionY, 2.0f), new Vector3(Data[child].positionX, Data[child].positionY, 2.0f));
                bool c = Openlist.Contains(Data[current]);

                if (c == false)
                    Openlist.Add(Data[current]);

                current = child;
            }
        }
    }


    //線の描画
    private void DrawLine(Vector3 pointA, Vector3 pointB)
    {
        if (!mLine) return;

        var inst = Instantiate(mLine);
        inst.transform.SetParent(mCanvasInst.transform);
        inst.GetComponent<LineRenderer>().SetPositions(new Vector3[] { pointA, pointB });
    }
}