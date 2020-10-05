using UnityEngine;
using UnityEditor;

public class ShowPopup : EditorWindow
{
    public string message;
    public string list;
    public System.Action handler;

    static void Init()
    {
        ShowPopup window = CreateInstance<ShowPopup>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);
        window.ShowPopup();
    }

    public void Show(string title, string message, string list, System.Action handler)
    {
        ShowPopup window = CreateInstance<ShowPopup>();
        window.position = new Rect(Screen.width / 2, Screen.height / 2, 500, 500);

        window.titleContent.text = title;
        window.message = message;
        window.list = list;
        window.handler = handler;

        window.ShowUtility();
    }

    string text;

    private Vector2 scroll = new Vector2();

    void OnGUI()
    {
        if (message != null) EditorGUILayout.LabelField(message, EditorStyles.wordWrappedLabel);
        
        scroll = EditorGUILayout.BeginScrollView(scroll);
        if (list != null) EditorGUILayout.LabelField(list, EditorStyles.wordWrappedLabel);
        EditorGUILayout.EndScrollView();

        GUILayout.Space(30);

        GUIContent content = new GUIContent("OK");
        GUIStyle style = new GUIStyle();
        style.fixedHeight = 10;
        if (GUILayout.Button(content))
        {
            if (handler != null)
                handler();

            this.Close();
        }
    }
}