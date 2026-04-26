using UnityEngine;

public class GlitchText : MonoBehaviour
{
    public float intensity = 2f;

    private Vector3 originalPos;

    void Start()
    {
        originalPos = transform.localPosition;
    }

    void Update()
    {
        float offsetX = Random.Range(-intensity, intensity);
        float offsetY = Random.Range(-intensity, intensity);

        transform.localPosition = originalPos + new Vector3(offsetX, offsetY, 0);
    }
}

