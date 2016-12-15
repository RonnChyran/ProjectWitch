using UnityEngine;
using System.Collections;

namespace ProjectWitch.Battle
{
    public class TalkCommandHelper : MonoBehaviour
    {
        //スキルボタンを表示する
        //target:p0~p2に0~2 e0~e2に3~5と割り振る
        //error: エラーメッセージ格納用
        public void ShowSkillButton(int target, out string error)
        {
            error = null;
        }

        //スキルボタンを非表示にする
        public void HideSkillButton(out string error)
        {
            error = null;
        }

        //スキルを実行する
        //target: ShowSkillButtonと同様の割り振り
        //type: 0.攻撃 1.防御 2.捕獲
        public void ExecuteSkill(int target, int type, out string error)
        {
            error = null;
        }
    }
}