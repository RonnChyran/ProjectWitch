using UnityEngine;
using System.Collections;
using UnityEngine.UI;

namespace Battle
{
	public class DamageDisplay : MonoBehaviour
	{
        [SerializeField]
        private Font mFontDamage = null;
        [SerializeField]
        private Font mFontHeal = null;
		[SerializeField]
		private GameObject mImDamage = null, mImHeal = null;

		public BattleObj BattleObj { get; private set; }
		public Text Text { get { return gameObject.GetComponent<Text>(); } }
		public Image UseImage { get; private set; }
		public Vector3 DefaultPos { get; private set; }

		public void Setup()
		{
			DefaultPos = transform.localPosition;
		}

		private IEnumerator CoDisplay(float num, bool isDamage)
		{
			Text.text = ((int)num).ToString();
			Text.font = (isDamage ? mFontDamage : mFontHeal);
            Text.color = new Color(1, 1, 1);
			mImDamage.SetActive(false);
			mImHeal.SetActive(false);
			transform.localPosition = DefaultPos;
			if (isDamage)
			{
				mImDamage.SetActive(true);
				UseImage = mImDamage.GetComponent<Image>();
				yield return BattleObj.WaitSeconds(0.025f);
				transform.localPosition += new Vector3(0, 10, 0);
				yield return BattleObj.WaitSeconds(0.005f);
				while (transform.localPosition.y > DefaultPos.y)
				{
					transform.localPosition -= new Vector3(0, 100 * Time.deltaTime, 0);
					yield return BattleObj.WaitSeconds(0.0025f);
				}
			}
			else
			{
				mImHeal.SetActive(true);
				UseImage = mImHeal.GetComponent<Image>();
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
			var targetY = DefaultPos.y - 20;
			while (transform.localPosition.y > targetY)
			{
				transform.localPosition -= new Vector3(0, 100 * Time.deltaTime, 0);
				Text.color -= new Color(0, 0, 0, 5f * Time.deltaTime);
				UseImage.color -= new Color(0, 0, 0, 5f * Time.deltaTime);
				yield return BattleObj.WaitSeconds(0.0025f);
			}

			mImDamage.SetActive(false);
			mImHeal.SetActive(false);
			UseImage.color = new Color(1, 1, 1, 1);
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

