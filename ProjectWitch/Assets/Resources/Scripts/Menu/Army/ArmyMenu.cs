using UnityEngine;
using System.Collections;

public class ArmyMenu : MonoBehaviour {

    //トップメニューへの参照
    [SerializeField]
    private Canvas mTopMenu = null;

    //component 
    private Canvas mcCanvas = null;

    //閉じれるかどうか
    public bool Closable { get; set; }

    // Use this for initialization
    void Start () {
        mcCanvas = GetComponent<Canvas>();
        Closable = true;
	}
	
	// Update is called once per frame
	void Update () {
	    if(mcCanvas.enabled && Closable)
        {
            if(Input.GetButtonDown("Cancel"))
            {
                StartCoroutine(Close());
            }
        }
	}

    private IEnumerator Close()
    {
        //キャンセル音再生
        Game.GetInstance().SoundManager.PlaySE(SE.Cancel);

        yield return new WaitForSeconds(0.1f);
        mcCanvas.enabled = false;
        mTopMenu.enabled = true;
    }
}
