using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Assets.Scripts.States;
using Assets.Scripts.Configs;
using Assets.Scripts.Events;
using Assets.Scripts.Combat;
using System.Linq;

public class SpriteCharacter2D : MonoBehaviour
{
    [Header("Defaults")]
    public Texture2D placeholderTex;
    public string characterName = "Unnamed";

    [Header("Animations")]
    public List<SpriteBookConfig> animations;

    [Header("Playback Speed")]
    [Range(0.5f, 4.0f)]
    public float speedMult = 1.6f;

    public bool isFlipped = false;

    private IBattleActor myActor;
    
    public BattleSpriteState currentState { get; private set; }
    private BattleSpriteState loopState;

    private Dictionary<BattleSpriteState, List<SpriteBookConfig>> animationMap;
    private SpriteBookConfig activeBook;

    private Renderer rend;
    private int index;
    private float frameTime;

    // define animation fallbacks here
    private static readonly Dictionary<BattleSpriteState, BattleSpriteState[]> fallbackMap = new()
    {
        { BattleSpriteState.Critical, new[] { BattleSpriteState.Attack } },
        { BattleSpriteState.Run,      new[] { BattleSpriteState.Walk, BattleSpriteState.Idle } }
    };


    private Material TargetMaterial
    {
        get
        {
#if UNITY_EDITOR
            return Application.isPlaying ? rend.material : rend.sharedMaterial;
#else
            return rend.material;
#endif
        }
    }

    private void Awake()
    {
        myActor = GetComponentInParent<IBattleActor>();

    }

    void OnEnable()
    {
        rend = GetComponent<Renderer>();
        BuildAnimationMap();

    }

    private void Start()
    {
        transform.localRotation = Quaternion.Euler(new Vector3(90f, -168f, 0f));
        Play(BattleSpriteState.Idle);
    }

    private void Update()
    {
        if (rend == null)
            return;

        if (activeBook == null)
        {
            // Show placeholder if there's no animation playing
            if (placeholderTex != null)
            {
                if (isFlipped) isFlipped = false;
                TargetMaterial.SetTexture("_BaseMap", placeholderTex);
                TargetMaterial.SetTextureScale("_BaseMap", new Vector2(isFlipped ? -1f : 1f, 1f));
                TargetMaterial.SetTextureOffset("_BaseMap", Vector2.zero);
                TargetMaterial.color = Color.white;
            }
            return;
        }

        float multSpeed = Mathf.Lerp(1.4f, 2.5f, Mathf.Clamp01((activeBook.columns - 1) / 10f));

        frameTime += Time.deltaTime;
        if (frameTime >= 1f / (activeBook.framesPerSecond * speedMult))
        {
            int totalFrames = activeBook.columns * activeBook.rows;

            switch (activeBook.playbackType)
            {
                case AnimPlayback.Loop:
                    index = (index + 1) % totalFrames;
                    break;

                case AnimPlayback.PlayOnce:
                    index++;
                    if (index >= totalFrames)
                    {
                        index = totalFrames - 1;
                        Play(loopState);
                    }
                    break;

                case AnimPlayback.PlayThenStop:
                    index++;
                    if (index >= totalFrames)
                    {
                        index = totalFrames - 1;
                        return;
                    }
                    break;
            }

            UpdateFrame();
            frameTime = 0;
        }
    }

    private void SetupMaterial(SpriteBookConfig book)
    {
        TargetMaterial.SetTexture("_BaseMap", book.spriteSheet);
        TargetMaterial.SetTextureScale("_BaseMap", new Vector2(1f / book.columns, 1f / book.rows));
        TargetMaterial.color = Color.white;
    }

    public void UpdateFrame()
    {
        if (activeBook == null) return;

        int uIndex = index % activeBook.columns;
        int vIndex = index / activeBook.columns;

        float scaleX = isFlipped ? -1f / activeBook.columns : 1f / activeBook.columns;
        float offsetX = isFlipped
            ? (uIndex + 1) / (float)activeBook.columns
            : uIndex / (float)activeBook.columns;

        TargetMaterial.SetTextureScale("_BaseMap", new Vector2(scaleX, 1f / activeBook.rows));
        TargetMaterial.SetTextureOffset("_BaseMap",
            new Vector2(offsetX, 1 - (vIndex + 1) / (float)activeBook.rows));
    }

    private void BuildAnimationMap()
    {
        animationMap = new Dictionary<BattleSpriteState, List<SpriteBookConfig>>();
        if (animations == null) return;

        foreach (var anim in animations)
        {
            if (anim == null) continue;

            if (!animationMap.ContainsKey(anim.animationType))
                animationMap[anim.animationType] = new List<SpriteBookConfig>();

            animationMap[anim.animationType].Add(anim);
        }
    }


    public void LoadFromConfig(GameConfig config)
    {
        if (config == null) return;

        if (config is EnemyConfig enemy)
        {
            characterName = enemy.enemyName;
            animations = new List<SpriteBookConfig>(enemy.animations);
            isFlipped = true;
        }
        else if (config is HeroConfig hero)
        {
            characterName = hero.heroName; // assuming heroName differs
            animations = new List<SpriteBookConfig>(hero.animations);
            isFlipped = false;
        }
        else
        {
            Debug.LogWarning($"Unsupported config type: {config.GetType()}");
            return;
        }

        BuildAnimationMap();
        ResetState();

        if (animationMap.ContainsKey(BattleSpriteState.Idle))
            Play(BattleSpriteState.Idle);
        else if (animationMap.ContainsKey(BattleSpriteState.Walk))
            Play(BattleSpriteState.Walk);
        else
            activeBook = null;
    }


    public void Play(BattleSpriteState state)
    {
        if (animationMap == null) BuildAnimationMap();

        BattleSpriteState currentTry = state;
        bool foundAnimation = false;

        while (true)
        {
            // requested animation is immediately found
            if (animationMap.TryGetValue(currentTry, out var bookList) && bookList != null && bookList.Count > 0)
            {
                foundAnimation = true;
                SpriteBookConfig book = bookList.Count == 1
                    ? bookList[0]
                    : bookList[Random.Range(0, bookList.Count)];

                activeBook = book;
                this.currentState = currentTry;
                index = 0;
                frameTime = 0;

                if (book.playbackType == AnimPlayback.Loop)
                {
                    loopState = currentTry;
                }

                SetupMaterial(book);
                UpdateFrame();
                break;
            }

            // or go through list of fallbacks
            if (fallbackMap.TryGetValue(currentTry, out var fallbacks) && fallbacks.Length > 0)
            {
                currentTry = fallbacks[0];
                fallbackMap[currentTry] = fallbacks.Skip(1).ToArray();
            }
            else
            {
                break; // give up if no fallbacks found
            }
        }

        if (!foundAnimation)
        {
            Debug.Log($"{characterName} tried to play {state}, but no animation was found. Staying in current state.");
        }
    }



    public void ResetCharacter()
    {
        characterName = "Unnamed";
        animations?.Clear();
        activeBook = null;
        animationMap?.Clear();

        ResetState();

        if (rend != null && placeholderTex != null)
        {
            TargetMaterial.SetTexture("_BaseMap", placeholderTex);
            TargetMaterial.SetTextureScale("_BaseMap", new Vector2(isFlipped ? -1f : 1f, 1f));
            TargetMaterial.SetTextureOffset("_BaseMap", Vector2.zero);
            TargetMaterial.color = Color.white;
        }
    }
    public IEnumerable<BattleSpriteState> GetAvailableStates()
    {
        if (animationMap == null) BuildAnimationMap();
        return animationMap.Keys;
    }


    private void ResetState()
    {
        currentState = 0;
        index = 0;
        frameTime = 0;
    }
}

[CustomEditor(typeof(SpriteCharacter2D))]
public class SpriteCharacter2DEditor : Editor
{
    private GameConfig configToLoad;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        SpriteCharacter2D character = (SpriteCharacter2D)target;

        GUILayout.Space(10);
        GUILayout.Label("Animation Controls", EditorStyles.boldLabel);

        // Load from Config
        configToLoad = (GameConfig)EditorGUILayout.ObjectField(
            "Config To Load",
            configToLoad,
            typeof(GameConfig),
            false);

        if (GUILayout.Button("Load From Config") && configToLoad != null)
        {
            Undo.RecordObject(character, "Load From Config");
            character.LoadFromConfig(configToLoad);
            EditorUtility.SetDirty(character);
        }

        // Flip toggle
        EditorGUI.BeginChangeCheck();
        bool newFlipped = EditorGUILayout.Toggle("Flip?", character.isFlipped);
        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(character, "Toggle Flip");
            character.isFlipped = newFlipped;
            character.UpdateFrame();
        }

        GUILayout.Space(10);
        GUILayout.Label("Play Available States", EditorStyles.boldLabel);

        // Dynamically list only the states this character actually has
        if (character != null && character.GetAvailableStates() != null)
        {
            foreach (var state in character.GetAvailableStates())
            {
                if (GUILayout.Button($"Play {state}"))
                {
                    Undo.RecordObject(character, $"Play {state}");
                    character.Play(state);
                    EditorUtility.SetDirty(character);
                }
            }
        }

        GUILayout.Space(10);
        if (GUILayout.Button("Reset"))
        {
            Undo.RecordObject(character, "Reset Character");
            character.ResetCharacter();
            EditorUtility.SetDirty(character);
        }
    }
}
