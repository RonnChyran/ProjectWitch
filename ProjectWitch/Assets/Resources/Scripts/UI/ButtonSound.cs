using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

namespace ProjectWitch
{

    [RequireComponent(typeof(Button))]
    public class ButtonSound : MonoBehaviour, IPointerEnterHandler
    {
        Button mcButton = null;

        public void Start()
        {
            mcButton = GetComponent<Button>();
        }

        public void OnPointerEnter(PointerEventData e)
        {
            var game = Game.GetInstance();

            if(mcButton.interactable)
                game.SoundManager.PlaySE(SE.Hover);
        }

        public void OnClicked()
        {
            var game = Game.GetInstance();

            if(mcButton.interactable)
                game.SoundManager.PlaySE(SE.Click);
        }

        public void OnClicked_Cancel()
        {
            var game = Game.GetInstance();

            if(mcButton.interactable)
                game.SoundManager.PlaySE(SE.Cancel);
        }
    }
}