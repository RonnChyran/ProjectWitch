using UnityEngine;
using UnityEditor;
using System.Collections;

public class DebugWindow : EditorWindow {

    private string mTextField = "";
    private Vector2 mScrollPos = Vector2.zero;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	}

    [MenuItem("Window/PWDebugWindow")]
    static void ShowWindow()
    {
        DebugWindow window = (DebugWindow)EditorWindow.GetWindow(typeof(DebugWindow));
        window.Show();
    }

    void OnGUI()
    {
        GUILayout.Label("変数ビューワー", EditorStyles.boldLabel);

        if (!EditorApplication.isPlaying) return;
        var game = Game.GetInstance();
        var memoryList = game.SystemMemory;

        mScrollPos = EditorGUILayout.BeginScrollView(mScrollPos, GUI.skin.box);
        {
            for (int i = 0; i < 1000; i++)
            {
                EditorGUILayout.BeginHorizontal(GUI.skin.box);
                {
                    EditorGUILayout.PrefixLabel(i + " :");
                    EditorGUILayout.TextField(memoryList[i].ToString(), mTextField);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        EditorGUILayout.EndScrollView();

    }
}
