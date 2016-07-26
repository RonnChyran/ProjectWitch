using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DialogWindow : MonoBehaviour {

    //ボックス内の内容
    public string Caption { get; set; }
    public string Text { get; set; }

    public void Start()
    {
        var caption = transform.FindChild("Caption");
        var text = transform.FindChild("Text");

        caption.GetComponent<Text>().text = Caption;
        text.GetComponent<Text>().text = Text;
    }

	public void PushButton_OK()
    {
        Game.GetInstance().IsDialogShowd = false;
        Destroy(this.gameObject);
    }
}
