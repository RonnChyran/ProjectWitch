using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler {

    public void OnPointerEnter(PointerEventData e)
    {
        var game = Game.GetInstance();

        game.SoundManager.PlaySE(SE.Hover);
    }

    public void OnClicked()
    {
        var game = Game.GetInstance();

        game.SoundManager.PlaySE(SE.Click);
    }
}
