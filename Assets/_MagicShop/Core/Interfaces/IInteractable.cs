using System.Collections.Generic;
using UnityEngine;
namespace MagicShop.Core
{
    public interface IInteractable
    {
        InteractionUI UI { get; }
        string GetPrompt();
        InteractionAction GetAvailableAction(Interactor interactor);
        void OnInteractionStarted(Interactor interactor);
        void OnInteractionEnded(Interactor interactor);
    }
}