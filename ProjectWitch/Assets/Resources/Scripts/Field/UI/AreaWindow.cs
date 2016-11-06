using UnityEngine;
using UnityEngine.UI;
using System;
using GameData;

namespace Field
{
    public class AreaWindow : MonoBehaviour
    {

        //背景画像のパス
        private string mBackImagePath = "Textures/Field/Back/";

        //各オブジェクト
        [Space(1)]
        [Header("Parts")]
        [SerializeField]
        private Text mcOwner = null;
        [SerializeField]
        private Text mcLevel = null;
        [SerializeField]
        private Text mcTime = null;
        [SerializeField]
        private Text mcMana = null;
        [SerializeField]
        private Text mcPAtk = null;
        [SerializeField]
        private Text mcMAtk = null;
        [SerializeField]
        private Text mcPDef = null;
        [SerializeField]
        private Text mcMDef = null;
        [SerializeField]
        private Text mcLead = null;
        [SerializeField]
        private Text mcAgi = null;
        [SerializeField]
        private Image mcBack = null;
        [SerializeField]
        private Image mcOwnerIcon = null;
        [SerializeField]
        private GameObject mDominationButton = null;
        [SerializeField]
        private GameObject mManaButton = null;

        [Space(1)]

        //領主画像
        [SerializeField]
        private Sprite[] mcOwnerIconResources = null;


        //エリアID
        private int mAreaID = -1;
        public int AreaID { get { return mAreaID; } set { mAreaID = value; } }

        //コントローラ
        public FieldController FieldController { get; set; }
        public FieldUIController FieldUIController { get; set; }

        //名前ウィンドウの参照
        public GameObject NameWindow { get; set; }

        void Update()
        {
            var game = Game.GetInstance();

            //ダイアログが出ていたら何もしない
            if (game.IsDialogShowd) return;

            if (Input.GetButtonDown("Cancel"))
            {
                //キャンセル音再生
                game.SoundManager.PlaySE(SE.Cancel);

                Close();
            }
        }

        public void Init()
        {
            try
            {
                var game = Game.GetInstance();

                //areaデータ取得
                if (AreaID == -1) throw new ProjectWitchException("エリアIDをセットしてください");
                var area = game.AreaData[AreaID];

                //テキストデータのセット
                mcOwner.text = game.TerritoryData[area.Owner].OwnerName;
                mcLevel.text = area.Level.ToString();
                mcTime.text = area.Time.ToString();
                mcMana.text = area.Mana.ToString();
                mcPAtk.text = area.BattleFactor.PAtk.ToString() + "%";
                mcMAtk.text = area.BattleFactor.MAtk.ToString() + "%";
                mcPDef.text = area.BattleFactor.PDef.ToString() + "%";
                mcMDef.text = area.BattleFactor.MDef.ToString() + "%";
                mcLead.text = area.BattleFactor.Leadership.ToString() + "%";
                mcAgi.text = area.BattleFactor.Agility.ToString() + "%";

                //領主画像のセット
                mcOwnerIcon.sprite = mcOwnerIconResources[area.Owner];
                mcOwnerIcon.SetNativeSize();

                //背景画像のセット
                var resoruce = Resources.Load<Sprite>(mBackImagePath + area.BackgroundName);
                mcBack.sprite = resoruce;

                //ボタンの表示非表示
                ShowButton(area);
            }
            catch (NullReferenceException e)
            {
                Debug.LogError(e.Message + " : Inspectorの参照が切れていないかかくにんしてください");
            }
            catch (ArgumentException e)
            {
                Debug.LogError(e.Message + " : AreaIDかTerritoryIDが不正の可能性があります");
            }
            catch (ProjectWitchException e)
            {
                Debug.LogError(e.Message);
            }



        }

        //戦闘を呼び出す
        public void CallBattle()
        {
            var game = Game.GetInstance();

            //侵攻戦の開始
            FieldController.DominationBattle(AreaID, game.AreaData[AreaID].Owner);

            //メニューを閉じる
            Close();
        }

        //臨時徴収を呼びだす
        public void CallMana()
        {
            var game = Game.GetInstance();

            //二重起動の防止
            if (game.IsDialogShowd) return;

            //取得するマナ量
            var mana = game.AreaData[AreaID].Mana;

            //表示する文字列の構成
            var str = "地点：" + game.AreaData[AreaID].Name + "\n";
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

        //ウィンドウを閉じる
        public void Close()
        {

            FieldController.FlagClickable = true;
            FieldUIController.OwnerPanelLock = false;
            FieldUIController.SelectedTerritory = -1;
            Destroy(NameWindow);
            Destroy(this.gameObject);
        }

        //敵の領地なら侵攻可能か判断して侵攻ボタンを
        //味方の領地なら臨時徴収ボタンを表示
        private void ShowButton(AreaDataFormat area)
        {
            if(area.Owner == 0)
            {
                mManaButton.SetActive(true);
            }
            else
            {
                var game = Game.GetInstance();
                var nextAreas = area.NextArea;

                //隣接領地に自領地があるか判定
                bool isPossible = false;
                foreach(var nextArea in nextAreas)
                {
                    var data = game.AreaData[nextArea];

                    if (data.Owner == 0)
                    {
                        isPossible = true;
                        break;
                    }
                }
                if (!isPossible) return;

                //当該地域の領主が、自領地と交戦できる状態にあるか
                var territory = game.TerritoryData[area.Owner];
                if (territory.State == TerritoryDataFormat.TerritoryState.Ready ||
                    territory.State == TerritoryDataFormat.TerritoryState.Active)
                    mDominationButton.SetActive(true);

            }
        }
    }
}