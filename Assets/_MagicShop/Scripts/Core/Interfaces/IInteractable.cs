using System.Collections.Generic;
using UnityEngine;
namespace MagicShop.Core
{
    public interface IInteractable
    {
        string GetPrompt();
        InteractionAction GetAvailableAction(Interactor interactor);
        void OnInteractionStarted(Interactor interactor);
        void OnInteractionEnded(Interactor interactor);
        Sprite GetIcon();
    }
}