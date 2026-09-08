using UnityEngine;
using System.Collections.Generic;

namespace MagicShop.Core
{
    /// <summary>
    /// Gerencia e detecta snap points proximos para posicionamento magnetico de pickables.
    /// Este sistema pode ser um Singleton ou adicionado como componente em um objeto gerenciador.
    /// </summary>
    public class SnapZone : MonoBehaviour
    {
        [Header("Snap Configuration")]
        [SerializeField] private float detectionRadius = 10f;
        [SerializeField] private float maxSnapDistance = 1.5f;
        [SerializeField] private bool visualizeDetection = false;

        private List<SnapPoint> registeredSnapPoints = new List<SnapPoint>();
        private static SnapZone instance;
        private bool isSnapping = false;

        public bool IsSnapping => isSnapping;

        public static SnapZone Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<SnapZone>();
                    if (instance == null)
                    {
                        GameObject go = new GameObject("SnapZone");
                        instance = go.AddComponent<SnapZone>();
                    }
                }
                return instance;
            }
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            RegisterAllSnapPoints();
        }

        private void OnDestroy()
        {
            if (instance == this)
            {
                instance = null;
            }
        }

        /// <summary>
        /// Registra um novo snap point no sistema.
        /// </summary>
        public void RegisterSnapPoint(SnapPoint snapPoint)
        {
            if (!registeredSnapPoints.Contains(snapPoint))
            {
                registeredSnapPoints.Add(snapPoint);
            }
        }

        /// <summary>
        /// Remove um snap point do sistema.
        /// </summary>
        public void UnregisterSnapPoint(SnapPoint snapPoint)
        {
            registeredSnapPoints.Remove(snapPoint);
        }

        /// <summary>
        /// Encontra o snap point mais proximo a uma posicao especificada.
        /// Retorna null se nenhum snap point estiver dentro da distancia maxima.
        /// </summary>
        public SnapPoint FindNearestSnapPoint(Vector3 position)
        {
            SnapPoint nearestSnapPoint = null;
            float nearestDistance = maxSnapDistance;

            foreach (SnapPoint snapPoint in registeredSnapPoints)
            {
                if (snapPoint == null) continue;

                float distance = snapPoint.GetDistanceToPoint(position);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestSnapPoint = snapPoint;
                }
            }

            return nearestSnapPoint;
        }

        /// <summary>
        /// Encontra todos os snap points dentro de um raio especificado de uma posicao.
        /// </summary>
        public List<SnapPoint> FindSnapPointsInRadius(Vector3 position, float radius)
        {
            List<SnapPoint> foundSnapPoints = new List<SnapPoint>();

            foreach (SnapPoint snapPoint in registeredSnapPoints)
            {
                if (snapPoint == null) continue;

                float distance = snapPoint.GetDistanceToPoint(position);
                if (distance <= radius)
                {
                    foundSnapPoints.Add(snapPoint);
                }
            }

            foundSnapPoints.Sort((a, b) => 
                a.GetDistanceToPoint(position).CompareTo(b.GetDistanceToPoint(position))
            );

            return foundSnapPoints;
        }

        /// <summary>
        /// Encontra todos os snap points que pertencem a um objeto especifico (como uma blueprint).
        /// </summary>
        public List<SnapPoint> GetSnapPointsInObject(GameObject gameObject)
        {
            List<SnapPoint> snapPoints = new List<SnapPoint>();
            if (gameObject == null) return snapPoints;

            SnapPoint[] foundSnapPoints = gameObject.GetComponentsInChildren<SnapPoint>();
            foreach (SnapPoint snapPoint in foundSnapPoints)
            {
                snapPoints.Add(snapPoint);
            }

            return snapPoints;
        }

        /// <summary>
        /// Estrutura para armazenar um par de snap points (blueprint + ambiente).
        /// </summary>
        public struct SnapPointPair
        {
            public Transform blueprintSnapTransform;
            public SnapPoint environmentSnap;
            public float distance;

            public SnapPointPair(Transform blueprintSnap, SnapPoint environment, float dist)
            {
                blueprintSnapTransform = blueprintSnap;
                environmentSnap = environment;
                distance = dist;
            }
        }

        /// <summary>
        /// Encontra o snap point do ambiente mais proximo para uma blueprint.
        /// Usa a logica de isLeftSide para determinar qual snap point da blueprint usar.
        /// </summary>
        public SnapPointPair FindBestSnapPointPairForBlueprint(Blueprint blueprint, Vector3 blueprintCenter)
        {
            if (blueprint == null)
            {
                isSnapping = false;
                return new SnapPointPair(null, null, -1);
            }

            // Encontra o snap point do ambiente mais proximo
            SnapPoint nearestEnvironmentSnap = FindNearestSnapPoint(blueprintCenter);

            if (nearestEnvironmentSnap == null)
            {
                isSnapping = false;
                return new SnapPointPair(null, null, -1);
            }

            // Obtém o snap point correto da blueprint baseado no lado do snap point do ambiente
            Transform blueprintSnapPoint = blueprint.GetSnapPointForEnvironmentSide(nearestEnvironmentSnap.AlignToTheLeft);

            if (blueprintSnapPoint == null)
            {
                isSnapping = false;
                return new SnapPointPair(null, null, -1);
            }

            float distance = Vector3.Distance(blueprintSnapPoint.position, nearestEnvironmentSnap.Position);

            // Verifica se a distancia eh valida
            if (distance > maxSnapDistance)
            {
                isSnapping = false;
                return new SnapPointPair(null, null, -1);
            }

            isSnapping = true;
            return new SnapPointPair(blueprintSnapPoint, nearestEnvironmentSnap, distance);
        }

        /// <summary>
        /// Calcula a posicao correta para a blueprint de forma que seu snap point
        /// se alinhe com o snap point do ambiente.
        /// </summary>
        public Vector3 CalculateAlignedPosition(Vector3 currentBlueprintCenter, 
            Transform blueprintSnap, SnapPoint environmentSnap)
        {
            if (blueprintSnap == null || environmentSnap == null)
                return currentBlueprintCenter;

            // Calcula o offset entre o centro da blueprint e seu snap point
            Vector3 offsetFromCenter = blueprintSnap.position - currentBlueprintCenter;

            // Posiciona a blueprint de forma que seu snap point fique no snap point do ambiente
            Vector3 alignedPosition = environmentSnap.Position - offsetFromCenter;

            return alignedPosition;
        }

        /// <summary>
        /// Registra automaticamente todos os SnapPoints na cena.
        /// </summary>
        private void RegisterAllSnapPoints()
        {
            SnapPoint[] snapPoints = FindObjectsByType<SnapPoint>(FindObjectsSortMode.None);
            foreach (SnapPoint snapPoint in snapPoints)
            {
                RegisterSnapPoint(snapPoint);
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (visualizeDetection)
            {
                Gizmos.color = new Color(0, 0, 1, 0.1f);
                Gizmos.DrawWireSphere(transform.position, detectionRadius);
            }
        }
    }
}
