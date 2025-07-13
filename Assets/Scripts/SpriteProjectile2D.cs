using Assets.Scripts.Configs;
using UnityEngine;

public class SpriteProjectile2D : MonoBehaviour
{
    private SpriteBookConfig activeBook;
    private Renderer rend;
    private int index;
    private float frameTime;
    private Material TargetMaterial;

    private Transform target;
    private float speed;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
        TargetMaterial = rend.material;
    }

    public void Initialize(SpriteBookConfig anim, Transform target, float speed, System.Action onImpact)
    {
        this.activeBook = anim;
        this.target = target;
        this.speed = speed;

        if (activeBook != null)
        {
            SetupMaterial(activeBook);
            UpdateFrame();
        }
    }

    private void Update()
    {
        if (target != null)
        {
            Vector3 targetPos = new Vector3(target.position.x, transform.position.y, target.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            {
                Destroy(gameObject);
                return;
            }
        }

        if (activeBook == null) return;

        frameTime += Time.deltaTime;
        if (frameTime >= 1f / activeBook.framesPerSecond)
        {
            int totalFrames = activeBook.columns * activeBook.rows;
            index = (index + 1) % totalFrames;
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

    private void UpdateFrame()
    {
        if (activeBook == null) return;
        int uIndex = index % activeBook.columns;
        int vIndex = index / activeBook.columns;
        TargetMaterial.SetTextureScale("_BaseMap", new Vector2(1f / activeBook.columns, 1f / activeBook.rows));
        TargetMaterial.SetTextureOffset("_BaseMap", new Vector2(
            uIndex / (float)activeBook.columns,
            1 - (vIndex + 1) / (float)activeBook.rows
        ));
    }
}
