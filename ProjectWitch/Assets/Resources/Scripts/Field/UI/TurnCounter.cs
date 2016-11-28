using UnityEngine;
using UnityEngine.UI;
using System.Collections;


namespace ProjectWitch
{
    public class TurnCounter : MonoBehaviour
    {

        private Text mcText;

        // Use this for initialization
        void Start()
        {
            mcText = GetComponent<Text>();
        }

        // Update is called once per frame
        void Update()
        {
            var game = Game.GetInstance();
            mcText.text = game.GameData.CurrentTurn.ToString();
        }
    }
}