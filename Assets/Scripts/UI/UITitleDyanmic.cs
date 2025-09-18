using TMPro;
using UnityEngine;

public class UITitleDynamic : MonoBehaviour
{
    public string title;
    private TextMeshPro textMeshPro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponent<TextMeshPro>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
