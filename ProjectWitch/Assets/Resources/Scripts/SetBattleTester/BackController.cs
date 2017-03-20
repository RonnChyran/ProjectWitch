using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using ProjectWitch.Battle;


namespace ProjectWitch.BattleTester
{
	public class BackController : MonoBehaviour
	{
		public Game Game { get; set; }

		[SerializeField]
		private GameObject m_Canvas, m_BackBattle;

		// Use this for initialization
		void Start()
		{
			Game = Game.GetInstance();
			m_Canvas.SetActive(true);
			m_Canvas.GetComponent<SetBattleTester>().Load();
		}

		// Update is called once per frame
		void Update()
		{
			if (!m_Canvas.activeSelf && !SceneManager.GetSceneByName(Game.SceneName_Battle).IsValid())
			{
				m_Canvas.SetActive(true);
				m_BackBattle.SetActive(false);
				var units = m_Canvas.GetComponent<SetBattleTester>().Units;
				foreach (var unit in units)
					unit.SetHPs();
			}
		}

		public void CancelBattle()
		{
			StartCoroutine(CoCancelBattle());
		}

		public IEnumerator CoCancelBattle()
		{
			var battleObj = GameObject.Find("BattleObject").GetComponent<BattleObj>();
			yield return SceneManager.UnloadSceneAsync(battleObj.BackGroundSceneName);
			yield return SceneManager.UnloadSceneAsync(Game.SceneName_Battle);
		}
	}
}
