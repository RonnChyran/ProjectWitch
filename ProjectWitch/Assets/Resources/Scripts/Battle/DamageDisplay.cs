using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Battle
{
	public class DamageDisplay : MonoBehaviour
	{
		[SerializeField]
		private Color mColorDamage = new Color(1, 0, 0), mColorHeal = new Color(0, 1, 0);

		public BattleObj BattleObj { get; private set; }
		public Text Text { get { return gameObject.GetComponent<Text>(); } }

		private IEnumerator CoDisplay(float num, bool isDamage)
		{
			Text.text = ((int)num).ToString();
			Text.color = (isDamage ? mColorDamage : mColorHeal);
			if (isDamage)
			{
				yield return BattleObj.WaitSeconds(0.025f);
				transform.localPosition += new Vector3(0, 10, 0);
				yield return BattleObj.WaitSeconds(0.005f);
				for (int i = 0; i < 10; i++)
				{
					transform.localPosition -= new Vector3(0, 1, 0);
					yield return BattleObj.WaitSeconds(0.0025f);
				}
			}
			else
			{
				Text.color = new Color(Text.color.r, Text.color.g, Text.color.b, 0.0f);
				for (int i = 0; i < 10; i++)
				{
					Text.color += new Color(0, 0, 0, 0.1f);
					yield return BattleObj.WaitSeconds(0.005f);
				}
			}
			yield return null;
		}

		public void Display(float num, bool isDamage)
		{
			if (!BattleObj)
				BattleObj = GameObject.Find("/BattleObject").GetComponent<BattleObj>();
			gameObject.SetActive(true);
			StartCoroutine(CoDisplay(num, isDamage));
		}

		private IEnumerator CoHide()
		{
			for (int i = 0; i < 20; i++)
			{
				transform.localPosition -= new Vector3(0, 1, 0);
				Text.color -= new Color(0, 0, 0, 0.05f);
				yield return BattleObj.WaitSeconds(0.0025f);
			}
			gameObject.SetActive(false);
			transform.localPosition += new Vector3(0, 10, 0);
		}

		public IEnumerator Hide()
		{
            if(gameObject.activeSelf)
                yield return StartCoroutine("CoHide");
		}

		// Use this for initialization
		void Start()
		{

		}

		// Update is called once per frame
		void Update()
		{

		}
	}
}

