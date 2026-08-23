using System;
using UnityEngine;
using UnityEngine.AdaptivePerformance;

public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    private bool isCursorLocked = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Mantem entre cenas
    }

    /// <summary>
    /// Desbloqueia o cursor para permitir interacao com UI
    /// </summary>
    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isCursorLocked = false;
        Debug.Log("CursorManager - Cursor unlocked");
    }

    /// <summary>
    /// Bloqueia o cursor no centro da tela para gameplay
    /// </summary>
    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isCursorLocked = true;
        Debug.Log("CursorManager - Cursor locked");
    }

    /// <summary>
    /// Alterna entre bloqueado e desbloqueado (util para menus com ESC)
    /// </summary>
    public void ToggleCursor()
    {
        if (isCursorLocked)
            UnlockCursor();
        else
            LockCursor();
    }

    /// <summary>
    /// Verifica se o cursor esta bloqueado
    /// </summary>
    public bool IsCursorLocked()
    {
        return isCursorLocked;
    }
}
