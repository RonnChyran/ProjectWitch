using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


namespace ProjectWitch
{
    public class LoadContainer :SaveLoadContainerBase
    {
        public override void Reset()
        {
            base.Reset();

            var path = GamePath.GameSaveFilePath(mFileIndex);
            
            //ロードファイルの存在をチェック
            if (!System.IO.File.Exists(path))
            {
                mcButton.interactable = false;
            }
        }

        //ロードを実行
        public void Load()
        {
            var game = Game.GetInstance();
            game.GameData.Load(mFileIndex);
            
            StartCoroutine(game.CallField());
        }
    }
}