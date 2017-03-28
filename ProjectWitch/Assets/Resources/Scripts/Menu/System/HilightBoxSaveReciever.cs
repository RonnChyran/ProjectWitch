﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch.Menu
{
    public class HilightBoxSaveReciever : HilightBoxReciever
    {
        
        // Update is called once per frame
        void Update()
        {
            //
            if(SelectedIndex>=0)
            {
                mcText.text = SelectedIndex.ToString() + "番のファイルにセーブします。\nよろしいですか？";
                mcButton.interactable = true;
            }
            else
            {
                mcText.text = "セーブするファイルを選択して下さい。";
                mcButton.interactable = false;
            }

        }

        //セーブを実行
        public void Save()
        {
            if(Container)
            {
                SaveContainer sContainer = (SaveContainer)Container;
                sContainer.Save();
            }
        }

    }
}