using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    public static InteractionUI Instance { get; private set; }

    [SerializeField] private TMP_Text label;
    [SerializeField] private Image icon;
    [SerializeField] private CanvasGroup canvasGroup;

    private float currentAlpha;
    private float targetAlpha;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
            canvasGroup = gameObject.AddComponent<CanvasGroup>();

        currentAlpha = 0f;
        targetAlpha = 0f;
        if (canvasGroup != null)
            canvasGroup.alpha = currentAlpha;
    }

    void Update()
    {
        if (canvasGroup == null)
            return;
        currentAlpha = Mathf.MoveTowards(currentAlpha, targetAlpha, Time.deltaTime * 5f);
        canvasGroup.alpha = currentAlpha;
    }

    public void ShowPrompt(string prompt, Sprite sprite)
    {
        if (prompt != null)
            label.text = prompt;
        if (sprite != null)
            icon.sprite = sprite;
        targetAlpha = 1;
    }

    public void HideAll()
    {
        targetAlpha = 0;
    }
}
