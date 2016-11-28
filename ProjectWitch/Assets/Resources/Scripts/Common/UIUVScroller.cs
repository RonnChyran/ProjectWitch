using UnityEngine;
using UnityEngine.UI;

namespace ProjectWitch
{
    public class UIUVScroller : MonoBehaviour
    {

        private RawImage mcRenderer;

        [SerializeField]
        private Vector2 mScrollSpeed = Vector2.zero;

        // Use this for initialization
        void Start()
        {
            mcRenderer = GetComponent<RawImage>();
        }

        // Update is called once per frame
        void Update()
        {

            var size = mcRenderer.uvRect.size;
            var offset = mcRenderer.uvRect.position;
            offset += mScrollSpeed * Time.deltaTime;

            if (offset.x > 1.0f) offset.x = 0.0f;
            if (offset.y > 1.0f) offset.y = 0.0f;

            mcRenderer.uvRect = new Rect(offset, size);
        }
    }
}