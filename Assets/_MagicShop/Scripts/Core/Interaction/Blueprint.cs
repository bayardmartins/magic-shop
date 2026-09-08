using UnityEngine;

namespace MagicShop.Core
{
    public class Blueprint : MonoBehaviour
    {
        [SerializeField] private Transform bottomLeftSnap;
        [SerializeField] private Transform bottomRightSnap;

        /// <summary>
        /// Retorna o snap point esquerdo inferior da blueprint.
        /// </summary>
        public Transform BottomLeftSnap => bottomLeftSnap;

        /// <summary>
        /// Retorna o snap point direito inferior da blueprint.
        /// </summary>
        public Transform BottomRightSnap => bottomRightSnap;

        /// <summary>
        /// Retorna o snap point correto da blueprint baseado no lado do snap point do ambiente.
        /// Quando isLeftSide = true, retorna bottomRightSnap (lado oposto).
        /// Quando isLeftSide = false, retorna bottomLeftSnap (lado oposto).
        /// </summary>
        public Transform GetSnapPointForEnvironmentSide(bool isLeftSide)
        {
            // Lado oposto: left alinha com right
            return isLeftSide ? bottomRightSnap : bottomLeftSnap;
        }
    }
}
