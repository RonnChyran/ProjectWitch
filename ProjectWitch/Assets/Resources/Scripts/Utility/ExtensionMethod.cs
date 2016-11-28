using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ProjectWitch.Extention
{
    public static class RandomEx
    {
        static Random _Rand = new Random();

        //リストからランダムに要素を取得
        public static T Random<T>(this List<T> list)
        {
            return list.ElementAt(_Rand.Next(list.Count()));
        }

        //あるリストからn個分ランダムに取り出す
        //リストの要素数<nの場合はリストの要素個分ランダムに取り出す
        public static List<T> RandomN<T>(this List<T> list, int n)
        {
            //ランダム配列を作成
            var dummy = Enumerable.Range(0,list.Count);
            dummy = dummy.OrderBy(i => Guid.NewGuid());
            var indexList = dummy.ToList();

            //リスト要素数 < nの場合、出力のリストサイズを縮小
            int outListSize = Math.Min(list.Count, n);

            //ランダム配列の要素をインデックスとして、n個分取り出す
            var outList = new List<T>();
            for (int i=0; i<outListSize; i++)
            {
                outList.Add(list[indexList[i]]);
            }
            return outList;
        }

        //リストをシャッフルする
        public static List<T> Shuffle<T>(this List<T> list)
        {
            return list.OrderBy(i => Guid.NewGuid()).ToList();
        }

        //順番にN個の要素を抜き出す
        //リストの要素数<nとなるときは、リストの要素数に合わせる
        public static List<T> GetOrderN<T>(this List<T> list, int n)
        {
            var outListSize = Math.Min(list.Count, n);
            return list.GetRange(0, outListSize);
        }

        //インデックスリストを引数に渡し、そのインデックスに対応する要素のリストを作成する
        public static List<T> GetFromIndex<T>(this List<T> list, List<int> indexList)
        {
            var outList = new List<T>();
            foreach(var index in indexList)
            {
                outList.Add(list[index]);
            }

            return outList;
        }
    }
}
