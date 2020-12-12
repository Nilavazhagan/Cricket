using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HelpManager : MonoBehaviour
{
    public static HelpManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
            throw new System.Exception("Attempted to create a second HelpManager Instance");
        }
        else
        {
            Instance = this;
        }
    }

    public Text headerText, contentText;
    public RectTransform arrow, contentPanel;

    public void UpdateHelpContent(string header, string content)
    {
        headerText.text = header;
        contentText.text = content;
    }

    public void ToggleHelpVisibility()
    {
        if (contentPanel.gameObject.activeSelf)
        {
            contentPanel.gameObject.SetActive(false);
            arrow.localRotation = Quaternion.Euler(0f, 0f, 0f);
        }
        else
        {
            contentPanel.gameObject.SetActive(true);
            arrow.localRotation = Quaternion.Euler(0f, 0f, -90f);
        }
    }
}
