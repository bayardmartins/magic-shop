using System;
namespace MagicShop.Core
{
    public class InteractionAction
    {
        public string Name;
        public string UnavailableHint;
        public Func<Interactor, bool> IsAvailable;
        public Action<Interactor> Execute;

        public InteractionAction(string name, Func<Interactor, bool> isAvailable, Action<Interactor> execute, string unavailableHint = null)
        {
            Name = name;
            IsAvailable = isAvailable;
            Execute = execute;
            UnavailableHint = unavailableHint;
        }

        public bool CanExecute(Interactor interactor)
        {
            return IsAvailable == null || IsAvailable(interactor);
        }
    }
}