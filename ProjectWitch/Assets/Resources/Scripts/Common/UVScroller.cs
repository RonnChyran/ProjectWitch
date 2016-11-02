using UnityEngine;
using System.Collections;

public class UVScroller : MonoBehaviour {

    private MeshRenderer mcRenderer;

    [SerializeField]
    private Vector2 mScrollSpeed = Vector2.zero;

	// Use this for initialization
	void Start () {
        mcRenderer = GetComponent<MeshRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
        var offset = mcRenderer.material.mainTextureOffset;
        offset += mScrollSpeed;

        if (offset.x > 1.0f) offset.x = 0.0f;
        if (offset.y > 1.0f) offset.y = 0.0f;

        mcRenderer.material.mainTextureOffset = offset;
        
	}
}
