using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Backdrop : MonoBehaviour
{
    [SerializeField] private List<GameObject> backgrounds;

    private int currentIndex = 0;

    private void Awake()
    {
        HideAll();
        if (backgrounds.Count > 0)
        {
            backgrounds[0].SetActive(true);
        }
    }

    public void Next()
    {
        backgrounds[currentIndex].SetActive(false);
        currentIndex++;
        if (currentIndex >= backgrounds.Count)
            currentIndex = 0;
        backgrounds[currentIndex].SetActive(true);
    }

    public void Prev()
    {
        backgrounds[currentIndex].SetActive(false);
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = backgrounds.Count - 1;
        backgrounds[currentIndex].SetActive(true);
    }

    private void HideAll()
    {
        foreach (var bg in backgrounds)
        {
            if (bg != null)
                bg.SetActive(false);
        }
    }
}

[CustomEditor(typeof(Backdrop))]
public class BackdropEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        Backdrop backdrop = (Backdrop)target;

        EditorGUILayout.Space();
        EditorGUILayout.LabelField("Backdrop Controls", EditorStyles.boldLabel);

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Prev"))
        {
            backdrop.Prev();
        }
        if (GUILayout.Button("Next"))
        {
            backdrop.Next();
        }
        EditorGUILayout.EndHorizontal();
    }
}
