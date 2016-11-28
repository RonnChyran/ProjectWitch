using System.Reflection;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectWitch
{
    public class InGameConsole : MonoBehaviour
    {
        //表示部分
        [SerializeField]
        private GameObject mcUI = null;

        //表示するテキストの配列
        private Queue<string> text = new Queue<string>();


        // Use this for initialization
        void Start()
        {
        }

        // Update is called once per frame
        void Update()
        {
            if (mcUI.activeSelf)
            {
                if (Input.GetButtonDown("Console"))
                    Hide();
            }
            else
            {
                if (Input.GetButtonDown("Console"))
                    Show();
            }
        }

        public void Show()
        {
            mcUI.SetActive(true);
        }

        public void Hide()
        {
            mcUI.SetActive(false);
        }
    }

    public class InGameConsoleCommand
    {
        //指定のリストの要素をすべて出力する
        [uREPL.Command(name = "printlistall")]
        public static void PrintListAll<T>(List<T> list)
        {
            string outStr = "";

            Type t = typeof(T);
            MemberInfo[] members = t.GetMembers
                (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            //メンバ名の列挙
            if (members.Length == 0) return;
            foreach (var m in members)
            {
                if (m.MemberType.ToString().Equals("Property"))
                {
                    outStr += "\t" + m.Name;
                }
            }
            uREPL.Log.Output(outStr);
            outStr = "";

            //データの列挙
            for (int i = 0; i < list.Count; i++)
            {
                outStr += i.ToString();
                foreach (var m in members)
                {
                    if (m.MemberType.ToString().Equals("Property"))
                    {
                        var pr = t.GetProperty(m.Name);
                        var obj = pr.GetValue(list[i], null);
                        outStr += "\t" + obj.ToString();
                    }
                }
                uREPL.Log.Output(outStr);
                outStr = "";
            }
        }

        //指定のリストのインデックスで指定した要素を出力する
        [uREPL.Command(name = "printlist")]
        public static void PrintList<T>(List<T> list, int index)
        {
            string outStr = "";

            Type t = typeof(T);
            MemberInfo[] members = t.GetMembers
                (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var m in members)
            {
                if (m.MemberType.ToString().Equals("Property"))
                {
                    var pr = t.GetProperty(m.Name);
                    outStr += String.Format("{0,20}", m.Name) + " : ";
                    outStr += pr.GetValue(list[index], null);
                    outStr += "\n";
                }

            }
            uREPL.Log.Output(outStr);
        }

        //指定のリストのインデックスで指定した要素のメンバを書き換える
        [uREPL.Command(name = "setlist")]
        public static void SetList<T>(List<T> list, int index, string member, object value)
        {
            Type t = typeof(T);
            MemberInfo[] members = t.GetMembers
                (BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var m in members)
            {
                if (m.Name.Equals(member))
                {
                    if (m.MemberType.ToString().Equals("Property"))
                    {
                        var pr = t.GetProperty(m.Name);
                        pr.SetValue(list[index], value, null);
                    }
                }
            }
        }

        #region GameData
        //index番目の領地データを表示
        [uREPL.Command(name = "printterritory")]
        public static void PrintTerritory(int index) { PrintList(Game.GetInstance().TerritoryData, index); }
        //index番目の領地データの指定のメンバを変更
        [uREPL.Command(name = "setterritory")]
        public static void SetTerritory(int index, string member, object value) { SetList(Game.GetInstance().TerritoryData, index, member, value); }

        //index番目のグループデータを表示
        [uREPL.Command(name = "printgroup")]
        public static void PrintGroup(int index) { PrintList(Game.GetInstance().GroupData, index); }
        //index番目のグループデータの指定のメンバを変更
        [uREPL.Command(name = "setgroup")]
        public static void SetGroup(int index, string member, object value) { SetList(Game.GetInstance().GroupData, index, member, value); }

        //index番目のユニットデータを表示
        [uREPL.Command(name = "printunit")]
        public static void PrintUnit(int index) { PrintList(Game.GetInstance().UnitData, index); }
        //index番目のユニットデータの指定のメンバを変更
        [uREPL.Command(name = "setunit")]
        public static void SetUnit(int index, string member, object value) { SetList(Game.GetInstance().UnitData, index, member, value); }

        //index番目の地域データを表示
        [uREPL.Command(name = "printarea")]
        public static void PrintArea(int index) { PrintList(Game.GetInstance().AreaData, index); }
        //index番目の地域データの指定のメンバを変更
        [uREPL.Command(name = "setarea")]
        public static void SetArea(int index, string member, object value) { SetList(Game.GetInstance().AreaData, index, member, value); }

        //index番目のスキルデータを表示
        [uREPL.Command(name = "printskill")]
        public static void PrintSkill(int index) { PrintList(Game.GetInstance().SkillData, index); }
        //index番目のスキルデータの指定のメンバを変更
        [uREPL.Command(name = "setskill")]
        public static void SetSkill(int index, string member, object value) { SetList(Game.GetInstance().SkillData, index, member, value); }

        //index番目のカードデータを表示
        [uREPL.Command(name = "printcard")]
        public static void PrintCard(int index) { PrintList(Game.GetInstance().CardData, index); }
        //index番目のカードデータの指定のメンバを変更
        [uREPL.Command(name = "setcard")]
        public static void SetCard(int index, string member, object value) { SetList(Game.GetInstance().CardData, index, member, value); }

        //index番目の装備データを表示
        [uREPL.Command(name = "printequipment")]
        public static void PrintEquipment(int index) { PrintList(Game.GetInstance().EquipmentData, index); }
        //index番目の装備データの指定のメンバを変更
        [uREPL.Command(name = "setequipment")]
        public static void SetEquipment(int index, string member, object value) { SetList(Game.GetInstance().EquipmentData, index, member, value); }

        //index番目のAIデータを表示
        [uREPL.Command(name = "printai")]
        public static void PrintAI(int index) { PrintList(Game.GetInstance().AIData, index); }
        //index番目のAIデータの指定のメンバを変更
        [uREPL.Command(name = "setai")]
        public static void SetAI(int index, string member, object value) { SetList(Game.GetInstance().AIData, index, member, value); }

        //index番目のシステムメモリを表示
        [uREPL.Command(name = "printmemory")]
        public static void PrintMemory(int index) { uREPL.Log.Output(Game.GetInstance().SystemMemory[index].ToString()); }
        //index番目のシステムメモリを書き換え
        [uREPL.Command(name = "setmemory")]
        public static void SetMemory(int index, object value) { Game.GetInstance().SystemMemory[index] = value.ToString(); }

        #endregion
    }
}