using UnityEngine;

namespace Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Camera Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private float distance = 10f;
        [SerializeField] private float height = 5f;
        [SerializeField] private float rotationSpeed = 2f;
        [SerializeField] private float zoomSpeed = 5f;
        
        [Header("Zoom Settings")]
        [SerializeField] private float minDistance = 5f;
        [SerializeField] private float maxDistance = 20f;
        
        [Header("Rotation Settings")]
        [SerializeField] private float minYAngle = 10f;
        [SerializeField] private float maxYAngle = 80f;
        
        private float currentX = 0f;
        private float currentY = 0f;
        private float currentDistance;
        
        private void Start()
        {
            currentDistance = distance;
            currentY = 45f; // Начальный угол обзора сверху
            
            // Получаем цель по умолчанию
            if (target == null)
            {
                target = GameObject.FindGameObjectWithTag("Player")?.transform;
            }
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            // Обработка вращения камеры
            HandleRotation();
            
            // Обработка зума
            HandleZoom();
            
            // Позиционирование камеры
            UpdateCameraPosition();
        }
        
        private void HandleRotation()
        {
            // Вращение с помощью мыши
            if (Input.GetMouseButton(1)) // Правая кнопка мыши
            {
                currentX += Input.GetAxis("Mouse X") * rotationSpeed;
                currentY -= Input.GetAxis("Mouse Y") * rotationSpeed;
                
                // Ограничиваем вертикальный угол
                currentY = Mathf.Clamp(currentY, minYAngle, maxYAngle);
            }
        }
        
        private void HandleZoom()
        {
            // Зум с помощью колесика мыши
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            currentDistance -= scroll * zoomSpeed;
            currentDistance = Mathf.Clamp(currentDistance, minDistance, maxDistance);
        }
        
        private void UpdateCameraPosition()
        {
            // Вычисляем направление камеры
            Vector3 direction = new Vector3(0, 0, -currentDistance);
            Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
            
            // Позиционируем камеру
            transform.position = target.position + rotation * direction;
            transform.LookAt(target.position);
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        public void ResetCamera()
        {
            currentX = 0f;
            currentY = 45f;
            currentDistance = distance;
        }
        
        public void SetDistance(float newDistance)
        {
            currentDistance = Mathf.Clamp(newDistance, minDistance, maxDistance);
        }
        
        public void SetHeight(float newHeight)
        {
            height = newHeight;
        }
    }
}