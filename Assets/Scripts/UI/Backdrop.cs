using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[ExecuteInEditMode]
public class Backdrop : MonoBehaviour
{
    [SerializeField] private List<Sprite> backgroundSprites;
    [SerializeField] private SpriteRenderer targetRenderer;
    [SerializeField] private Camera targetCamera;

    private int currentIndex = 0;

    private void Awake()
    {
        if (backgroundSprites.Count > 0)
        {
            currentIndex = Random.Range(0, backgroundSprites.Count);
            SetSprite(backgroundSprites[currentIndex]);
        }
    }

    public void Next()
    {
        if (backgroundSprites.Count == 0) return;
        currentIndex = (currentIndex + 1) % backgroundSprites.Count;
        SetSprite(backgroundSprites[currentIndex]);
    }

    public void Prev()
    {
        if (backgroundSprites.Count == 0) return;
        currentIndex = (currentIndex - 1 + backgroundSprites.Count) % backgroundSprites.Count;
        SetSprite(backgroundSprites[currentIndex]);
    }

    public void Randomize()
    {
        if (backgroundSprites.Count == 0) return;

        int newIndex;
        do
        {
            newIndex = Random.Range(0, backgroundSprites.Count);
        } while (newIndex == currentIndex && backgroundSprites.Count > 1);

        currentIndex = newIndex;
        SetSprite(backgroundSprites[currentIndex]);
    }

    private void SetSprite(Sprite sprite)
    {
        targetRenderer.sprite = sprite;
        FitSpriteToCamera(targetRenderer, targetCamera);
    }

    private void FitSpriteToCamera(SpriteRenderer renderer, Camera cam)
    {
        if (renderer.sprite == null || cam == null) return;


        float targetWidth = 2f * cam.orthographicSize * cam.aspect;
        float targetHeight = cam.orthographicSize;

        float spriteWidth = renderer.sprite.bounds.size.x;
        float spriteHeight = renderer.sprite.bounds.size.y;

        float scaleX = targetWidth / spriteWidth;
        float scaleY = targetHeight / spriteHeight;

        renderer.transform.localScale = new Vector3(scaleX, scaleY, 1f);

        renderer.transform.position = new Vector3(0f, 6.2f, 15f);
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
        if (GUILayout.Button("Prev")) backdrop.Prev();
        if (GUILayout.Button("Next")) backdrop.Next();
        if (GUILayout.Button("Random")) backdrop.Randomize();
        EditorGUILayout.EndHorizontal();
    }
}
