using TMPro;
using UnityEngine;

public class UITitleDynamic : MonoBehaviour
{
    public string title;
    private string _content;
    public TextMeshProUGUI textMeshPro;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        textMeshPro = GetComponent<TextMeshProUGUI>();
    }
    private void OnGUI()
    {
        WriteContent(_content);
    }
    protected virtual void WriteContent(string content)
    {
        _content = content;
        textMeshPro.SetText(title + _content);
    }
}
