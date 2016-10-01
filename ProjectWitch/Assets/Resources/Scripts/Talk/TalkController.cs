using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;


public class TalkController : MonoBehaviour {

    [SerializeField]
    private GameObject mMask;   //画面のトランジション用マスク
    [SerializeField]
    private float mTransSpeed = 0.03f;


	// Use this for initialization
	void Start () {
        var game = Game.GetInstance();
        
        //トークシーンの開始
        game.IsTalk = true;

        //画面を明るくする
        StartCoroutine(FadeIn());
	}

    public IEnumerator EndScript()
    {
        var game = Game.GetInstance();

        //画面を暗くする
        yield return StartCoroutine(FadeOut());
        
        //トークシーンの終了
        game.IsTalk = false;

        //シーンのアンロード
        SceneManager.UnloadScene(game.SceneName_Talk);

        yield return null;
    }

    private IEnumerator FadeIn()
    {

        var image = mMask.GetComponent<Image>();

        while (image.color.a > mTransSpeed)
        {
            var c = image.color;
            image.color = new Color(c.r,c.g,c.b,c.a-mTransSpeed);
            yield return null;
        }

        yield return null;
    }

    private IEnumerator FadeOut()
    {
        var image = mMask.GetComponent<Image>();

        while (image.color.a < 1.0f)
        {
            var c = image.color;
            image.color = new Color(c.r, c.g, c.b, c.a + mTransSpeed);
            yield return null;
        }

        yield return null;
    }
}
