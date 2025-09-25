using UnityEngine;
using RobotCoder.Core;

namespace Core
{
    public class CameraController : MonoBehaviour
    {
        [Header("Target Settings")]
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0, 10, -10);
        [SerializeField] private float smoothSpeed = 0.125f;
        
        [Header("Rotation Settings")]
        [SerializeField] private bool allowRotation = true;
        [SerializeField] private float rotationSpeed = 2f;
        
        [Header("Zoom Settings")]
        [SerializeField] private bool allowZoom = true;
        [SerializeField] private float zoomSpeed = 2f;
        [SerializeField] private float minZoom = 5f;
        [SerializeField] private float maxZoom = 20f;
        
        private Vector3 desiredPosition;
        private float currentZoom = 10f;
        
        private void Start()
        {
            if (target == null)
            {
                // Try to find the robot controller
                RobotController robot = RobotController.Instance;
                if (robot != null)
                {
                    target = robot.transform;
                }
            }
            
            currentZoom = offset.magnitude;
        }
        
        private void LateUpdate()
        {
            if (target == null) return;
            
            HandleInput();
            UpdateCameraPosition();
        }
        
        private void HandleInput()
        {
            if (allowRotation)
            {
                // Rotate camera around target with right mouse button
                if (Input.GetMouseButton(1))
                {
                    float mouseX = Input.GetAxis("Mouse X");
                    transform.RotateAround(target.position, Vector3.up, mouseX * rotationSpeed);
                }
            }
            
            if (allowZoom)
            {
                // Zoom with scroll wheel
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                currentZoom -= scroll * zoomSpeed;
                currentZoom = Mathf.Clamp(currentZoom, minZoom, maxZoom);
            }
        }
        
        private void UpdateCameraPosition()
        {
            // Calculate desired position based on target and offset
            desiredPosition = target.position + offset.normalized * currentZoom;
            
            // Smoothly move camera to desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
            transform.position = smoothedPosition;
            
            // Look at target
            transform.LookAt(target);
        }
        
        public void SetTarget(Transform newTarget)
        {
            target = newTarget;
        }
        
        public void SetOffset(Vector3 newOffset)
        {
            offset = newOffset;
            currentZoom = offset.magnitude;
        }
        
        public void SetZoom(float zoom)
        {
            currentZoom = Mathf.Clamp(zoom, minZoom, maxZoom);
        }
    }
}