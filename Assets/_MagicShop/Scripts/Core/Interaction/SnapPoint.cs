using System;
using UnityEngine;

namespace MagicShop.Core
{
    /// <summary>
    /// Define um ponto de encaixe magnetico onde os pickables podem se "grudar".
    /// Use para marcar quinas, bordas de prateleiras ou outras superficies onde objetos devem se encaixar.
    /// </summary>
    public class SnapPoint : MonoBehaviour
    {
        [Header("Snap Configuration")]
        [SerializeField] private float snapRadius = 0.5f;
        [SerializeField] private bool visualizeSnap = true;
        [SerializeField] private bool alignToTheLeft = true;

        public float SnapRadius => snapRadius;
        public Vector3 Position => transform.position;
        public bool AlignToTheLeft => alignToTheLeft;

        private void OnDrawGizmosSelected()
        {
            if (visualizeSnap)
            {
                Gizmos.color = new Color(0, 1, 0, 0.3f);
                Gizmos.DrawWireSphere(transform.position, snapRadius);

                Gizmos.color = Color.green;
                Gizmos.DrawSphere(transform.position, 0.1f);
            }
        }

        /// <summary>
        /// Calcula se um ponto esta dentro da zona de snap deste ponto.
        /// </summary>
        public bool IsPointInSnapZone(Vector3 point)
        {
            return Vector3.Distance(point, Position) <= snapRadius;
        }

        /// <summary>
        /// Retorna a distancia entre este snap point e um ponto no mundo.
        /// </summary>
        public float GetDistanceToPoint(Vector3 point)
        {
            return Vector3.Distance(point, Position);
        }
    }
}
