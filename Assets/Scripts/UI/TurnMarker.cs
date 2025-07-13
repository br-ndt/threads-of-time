using UnityEngine;

public class TurnMarker : MonoBehaviour
{
    public float amplitude = 0.2f;
    public float frequency = 5f;

    public float rotateAmplitude = 60f;
    public float rotateFrequency = 1.5f; 

    private Vector3 startPos;
    private Quaternion startRot;

    private void Start()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation;
    }

    private void Update()
    {
        // Bob up and down
        float offsetY = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPos + new Vector3(0f, offsetY, 0f);

        // Sway rotation around local Y
        float sway = Mathf.Sin(Time.time * rotateFrequency) * rotateAmplitude;
        transform.localRotation = startRot * Quaternion.Euler(0f, sway, 0f);
    }
}
