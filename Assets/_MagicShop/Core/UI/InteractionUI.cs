using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InteractionUI : MonoBehaviour
{
    [SerializeField] private TMP_Text label;
    [SerializeField] private Image icon;

    private void Awake()
    {
        HideAll();
    }


    public void ShowPrompt(string prompt)
    {
        label.text = prompt;
        label.gameObject.SetActive(true);
        icon.gameObject.SetActive(true);
    }

    public void HideAll()
    {
        label.gameObject.SetActive(false);
        icon.gameObject.SetActive(false);
    }
}
