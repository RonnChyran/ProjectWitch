using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch
{
    public class SaveLoadContainerBase :MonoBehaviour
    {
        //title
        [SerializeField]
        private Text mcTitle = null;

        //セーブ番号
        [SerializeField]
        protected int mFileIndex = 0;

        //セーブした時間
        [SerializeField]
        private Text mcTimeStamp = null;

        //占領領地
        [SerializeField]
        private Text mcDominatedArea = null;

        //レベル
        [SerializeField]
        private Text mcLevel = null;

        //経過ターン数
        [SerializeField]
        private Text mcTurn = null;

        //ボタンコンポーネント
        [SerializeField]
        protected Button mcButton = null;

        public virtual void Reset()
        {
            var path = GamePath.GameSaveFilePath(mFileIndex);

            //ファイル番号
            mcTitle.text = "File" + mFileIndex.ToString();

            //ロードファイルの存在をチェック
            if (System.IO.File.Exists(path))
            {
                //存在する場合は、メタデータを読み出し、値を更新
                GameMetaData meta = new GameMetaData();
                FileIO.LoadMetaData(path, meta);

                //time
                var y = meta.Year.ToString();
                var m = meta.Month.ToString();
                var d = meta.Day.ToString();
                var h = meta.Hour.ToString();
                var min = meta.Minute.ToString();
                mcTimeStamp.text = y + "/" + m + "/" + d + "  " + h + ":" + min;

                //占領領地
                mcDominatedArea.text = "占領済み領地：" + meta.DominatedTerritory.ToString();

                //レベル
                mcLevel.text = "Lv." + meta.Level.ToString();

                //経過ターン数
                mcTurn.text = meta.Turn.ToString() + "ターン目";
            }
        }

        // Use this for initialization
        public virtual void Start()
        {
            Reset();
        }
    }
}
