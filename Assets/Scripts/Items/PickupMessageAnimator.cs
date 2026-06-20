using UnityEngine;

public class PickupMessageAnimator : MonoBehaviour
{
    private float timer = 0f;
    private TextMesh textMesh;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
    }

    void Update()
    {
        timer += Time.deltaTime;
        transform.position += Vector3.up * Time.deltaTime * 1.5f;

        if (textMesh != null)
        {
            Color color = textMesh.color;
            color.a = Mathf.Lerp(1f, 0f, timer / 1.5f);
            textMesh.color = color;
        }
    }
}