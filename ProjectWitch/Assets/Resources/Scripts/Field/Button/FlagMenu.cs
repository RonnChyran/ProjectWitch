﻿using UnityEngine;
using System.Collections;

namespace Field
{
    public class FlagMenu : MenuBase
    {
        //地点番号
        public int AreaID { get; set; }

        //field controller
        private FieldController mFieldController;

        void Start()
        {
            //フィールドUIコントローラを取得
            var obj = GameObject.FindWithTag("FieldController");
            mFieldController = obj.GetComponent<FieldController>();
        }

        //Update
        void Update()
        {
            var game = Game.GetInstance();

            //ダイアログが出ていたら何もしない
            if (game.IsDialogShowd) return;

            if (Input.GetMouseButton(1))
            {
                Close();
            }
        }

        //地点情報を表示
        public void CallAreaInfo()
        {
            var game = Game.GetInstance();

            //二重起動の防止
            if (game.IsDialogShowd) return;

            //地点情報の読み出し
            string areaInfo = "";
            areaInfo += game.AreaData[AreaID].Name + "\n";
            areaInfo += game.TerritoryData[game.AreaData[AreaID].Owner].OwnerName + "領\n";
            areaInfo += "地点レベル：　" + game.AreaData[AreaID].Level.ToString() + "\n";
            areaInfo += "所持マナ：　" + game.AreaData[AreaID].Mana.ToString() + "\n";

            game.ShowDialog("地点情報", areaInfo);

            //メニューを閉じる
            Close();
        }

        //戦闘開始
        public void CallBattle()
        {
            var game = Game.GetInstance();

            //二重起動の防止
            if (game.IsDialogShowd) return;

            //侵攻戦の開始
            mFieldController.DominationBattle(AreaID, game.AreaData[AreaID].Owner);

            //メニューを閉じる
            Close();
        }

        //マナ集め
        public void CallManaGathering()
        {
            var game = Game.GetInstance();

            //二重起動の防止
            if (game.IsDialogShowd) return;

            //取得するマナ量
            var mana = game.AreaData[AreaID].Mana;

            //表示する文字列の構成
            var str = "地点：" + game.AreaData[AreaID].Name +"\n";
            str += "現在の所持マナ：" + game.PlayerMana + "M\n";
            str += "取得マナ：" + mana.ToString() + "M\n";
            str += "取得後の所持マナ：" + (game.PlayerMana + mana).ToString() + "M\n";

            game.ShowDialog("マナ収集", str);

            //マナの増加処理
            game.PlayerMana += mana;
            game.AreaData[AreaID].Mana = 0;

            //時間を進める
            game.CurrentTime++;

            //メニューを閉じる
            Close();
        }

        //メニューを閉じる
        public override void Close()
        {
            mFieldController.FlagClickable = true;
            Destroy(this.gameObject);
        }

    }
    
}