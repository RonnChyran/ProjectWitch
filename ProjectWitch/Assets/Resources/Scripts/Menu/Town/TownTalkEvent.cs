using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class TownTalkEvent : MonoBehaviour
    {
        //イベント実行エリアの指定
        //広場:0 酒場:1 路地裏:2 街はずれ:3
        [SerializeField]
        private int mArea = -1;

        //メニューへの参照
        [SerializeField]
        private TownMenu mMenu = null;

        //自身のボタンへの参照
        private Button mcButton = null;

        //実行するスクリプト
        private EventDataFormat mEvent = null;

        // Use this for initialization
        void Start()
        {
            mcButton = GetComponent<Button>();
            Reset();
        }

        public void Reset()
        {
            //実行するスクリプトを決定

            //すべてのイベントの取得
            var game = Game.GetInstance();
            var events = game.GameData.TownEvent;

            //実行エリアに該当するスクリプトのみ取り出す
            events = events.Where(x => x.Area == mArea).ToList();

            //実行時間に該当するスクリプトのみ取り出す
            events = events.Where(x => ((int)x.Timing == game.GameData.CurrentTime || (int)x.Timing == 3)).ToList();

            //条件判定を行う
            var exeList = new List<EventDataFormat>();
            foreach(var e in events)
            {
                bool result = false;

                //条件式の数だけ条件判定をする
                for (int i=0; i<e.If_Var.Count; i++)
                {
                    if (e.If_Var[i] == -1) continue;
                    var mem_value = game.GameData.Memory[e.If_Var[i]];
                    switch(e.If_Ope[i])
                    {
                        case EventDataFormat.OperationType.Equal:
                            result = (mem_value == e.If_Imm[i]); break;
                        case EventDataFormat.OperationType.Bigger:
                            result = (mem_value < e.If_Imm[i]); break;
                        case EventDataFormat.OperationType.BiggerEqual:
                            result = (mem_value <= e.If_Imm[i]); break;
                        case EventDataFormat.OperationType.NotEqual:
                            result = (mem_value != e.If_Imm[i]); break;
                        case EventDataFormat.OperationType.Smaller:
                            result = (mem_value > e.If_Imm[i]); break;
                        case EventDataFormat.OperationType.SmallerEqual:
                            result = (mem_value >= e.If_Imm[i]); break;
                        default:
                            break;
                    }

                    //条件を満たさなかった場合、直ちに棄却する
                    if (!result) break;
                }
                
                //判定が棄却されなかったらスクリプトを実行リストへ入れる
                if (result)
                {
                    exeList.Add(e);
                }
            }

            //実行するスクリプトをランダムに取り出す
            if(exeList.Count>0 && game.GameData.TownEventEnable)
            {
                mEvent = exeList[UnityEngine.Random.Range(0, exeList.Count - 1)];
                mcButton.interactable = true;
            }
            else
            {
                mcButton.interactable = false;
            }
        }

        //イベント発火
        public void OnClick()
        {
            if(mEvent!=null)
                mMenu.ExecuteEvent(mEvent);
        }
    }
}