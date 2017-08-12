using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace ProjectWitch.Field
{
    public class MenuBar : MonoBehaviour
    {
        [SerializeField]
        private Button mMenu = null;

        [SerializeField]
        private Button mArmy = null;

        [SerializeField]
        private Button mItem = null;
        
        [SerializeField]
        private Button mTown = null;

        [SerializeField]
        private Button mSystem = null;

        [SerializeField]
        private Button mTIPS = null;

        [SerializeField]
        private Button mOverlookOn = null;

        [SerializeField]
        private Button mOverlookOff = null;

        [SerializeField]
        private FieldController mFieldController = null;

        [SerializeField]
        private FieldUIController mFieldUIController = null;

        [SerializeField]
        private CameraController mCameraController = null;


        // Update is called once per frame
        void Update()
        {
            //メニュー表示可能かでボタンの有効無効を決定
            if(mFieldController.MenuClickable)
            {
                mMenu.interactable = true;
                mArmy.interactable = true;
                mItem.interactable = true;
                mTown.interactable = true;
                mSystem.interactable = true;
                mTIPS.interactable = true;
                mOverlookOn.interactable = true;
                mOverlookOff.interactable = true;
            }
            else
            {
                mMenu.interactable = false;
                mArmy.interactable = false;
                mItem.interactable = false;
                mTown.interactable = false;
                mSystem.interactable = false;
                mTIPS.interactable = false;
                mOverlookOn.interactable = false;
                mOverlookOff.interactable = false;
            }
        }

        //ボタンのイベントコールバック

        public void OnClick_Menu()
        {
            mMenu.OnPointerExit(null);
            EventSystem.current.SetSelectedGameObject(null);
            Game.GetInstance().MenuDataIn.Top = MenuDataIn.TopMenu.Default;
            mFieldUIController.ShowMenu();
        }

        public void OnClick_Army()
        {
            mArmy.OnPointerExit(null);
            EventSystem.current.SetSelectedGameObject(null);
            Game.GetInstance().MenuDataIn.Top = MenuDataIn.TopMenu.Army;
            mFieldUIController.ShowMenu();
        }

        public void OnClick_Item()
        {
            mItem.OnPointerExit(null);
            EventSystem.current.SetSelectedGameObject(null);
            Game.GetInstance().MenuDataIn.Top = MenuDataIn.TopMenu.Item;
            mFieldUIController.ShowMenu();
        }

        public void OnClick_Town()
        {
            mTown.OnPointerExit(null);
            EventSystem.current.SetSelectedGameObject(null);
            Game.GetInstance().MenuDataIn.Top = MenuDataIn.TopMenu.Town;
            mFieldUIController.ShowMenu();
        }

        public void OnClick_System()
        {
            mSystem.OnPointerExit(null);
            EventSystem.current.SetSelectedGameObject(null);
            Game.GetInstance().MenuDataIn.Top = MenuDataIn.TopMenu.System;
            mFieldUIController.ShowMenu();
        }

        public void OnClick_Tips()
        {
            mTIPS.OnPointerExit(null);
            EventSystem.current.SetSelectedGameObject(null);
            Game.GetInstance().MenuDataIn.Top = MenuDataIn.TopMenu.Tips;
            mFieldUIController.ShowMenu();
        }

        public void OnClick_GotoTitle()
        {
            EventSystem.current.SetSelectedGameObject(null);
            Game.GetInstance().GoTitle.Show();
        }

        public void OnClick_Overlook(bool enable)
        {
            mOverlookOff.gameObject.SetActive(enable);
            mOverlookOn.gameObject.SetActive(!enable);
            mCameraController.ChangeOverLookCamera(enable);
        }
    }
}