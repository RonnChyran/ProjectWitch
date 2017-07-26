using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch.Field.Mana
{
    public class ManaController : MonoBehaviour
    {

        //スポーナー
        [SerializeField]
        private SoldierSpawner mSpawnerA = null;
        [SerializeField]
        private SoldierSpawner mSpawnerB = null;
        [SerializeField]
        private MessageSpawner mMesSpawner = null;

        //取得結果表示ウィンドウ
        [SerializeField]
        private GameObject mResultMesPrefab = null;

        //アニメーション
        [SerializeField]
        private Animator mcAnimator = null;

        //取得マナ
        public int Mana { get; set; }

        //取得アイテム
        public bool IsEquipment { get; set; }   //true:装備 false:カード
        public int ItemID { get; set; }

        //終了フラグ（FieldUIController側で検知）
        public bool IsEnd { get; set; }

        // Use this for initialization
        void Start()
        {
            mcAnimator = GetComponent<Animator>();

        }

        // Update is called once per frame
        public void Begin()
        {
            StartCoroutine(_Process());
        }

        private IEnumerator _Process()
        {
            var game = Game.GetInstance();

            yield return new WaitForSeconds(0.5f);

            mSpawnerA.SpawnStart();
            game.SoundManager.Play("114_fx_akariA", SoundType.SE);
            yield return new WaitForSeconds(1.0f);

            mMesSpawner.StartSpawn();
            yield return new WaitForSeconds(1.0f);

            mSpawnerA.SpawnStop();
            yield return new WaitForSeconds(0.5f);

            game.SoundManager.Play("165", SoundType.SE);
            yield return new WaitForSeconds(1.5f);

            mMesSpawner.StopSpawn();
            yield return new WaitForSeconds(2.0f);

            mSpawnerB.SpawnStart();
            ShowResultMessage(Mana.ToString() + "マナを手に入れた！");
            game.SoundManager.Play("114_fx_akariA", SoundType.SE);
            yield return new WaitForSeconds(3.0f);

            if (ItemID != -1)
            {
                mSpawnerB.SpawnStop();
                yield return null;

                //アイテムデータ
                string itemName = (IsEquipment) ? game.GameData.Equipment[ItemID].Name : game.GameData.Card[ItemID].Name;

                //アイテムを取得した場合に取得を示すノベルシーンを開始
                var e = new EventDataFormat();
                e.FileName = "sMana";
                game.CallScript(e);
                while (game.IsTalk) yield return null;

                ShowResultMessage(itemName + "を　手に入れた！");

                //手に入れたアイテムを領地データに追加
                if (IsEquipment)
                    game.GameData.Territory[0].EquipmentList[ItemID].Add(-1);
                else
                    game.GameData.Group[game.GameData.Territory[0].GroupList[0]].CardList.Add(ItemID);

                yield return new WaitForSeconds(3.0f);
            }

            //ウィンドウを閉じる
            mcAnimator.SetBool("IsShow", false);
            yield return new WaitForSeconds(1.0f);

            IsEnd = true;
            yield return new WaitForSeconds(5.0f);
            
            Destroy(this.gameObject);
            yield return null;
        }

        //結果ウィンドウの表示
        private void ShowResultMessage(string message)
        {
            var inst = Instantiate(mResultMesPrefab);
            var cMes = inst.GetComponent<Message>();
            cMes.transform.SetParent(transform, false);
            cMes.Text = message;
        }
    }
}