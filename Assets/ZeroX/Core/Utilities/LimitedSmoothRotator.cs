using UnityEngine;

namespace ZeroX
{
    [System.Serializable]
    public class LimitedSmoothRotator
    {
        [SerializeField] private float minHorizontalLimit = -45;
        [SerializeField] private float maxHorizontalLimit = 45;
        [SerializeField] private float minVerticalLimit = -30;
        [SerializeField] private float maxVerticalLimit = 30;

        private Transform _target;
        private Quaternion _baseRotation;

        private float _targetYaw;
        private float _targetPitch;

        private float _currentYaw;
        private float _currentPitch;

        private float _yawVelocity;
        private float _pitchVelocity;


        public bool IsInitialized => _target != null;


        public void Init(Transform target)
        {
            _target = target;

            if (_target == null)
                return;

            _baseRotation = _target.localRotation;

            _targetYaw = 0f;
            _targetPitch = 0f;

            _currentYaw = 0f;
            _currentPitch = 0f;

            _yawVelocity = 0f;
            _pitchVelocity = 0f;
        }

        private void ApplyRotateDelta(Vector2 delta)
        {
            // Góc đích
            _targetYaw -= delta.x;
            _targetPitch += delta.y;

            // Giới hạn góc
            _targetYaw = Mathf.Clamp(_targetYaw, minHorizontalLimit, maxHorizontalLimit);
            _targetPitch = Mathf.Clamp(_targetPitch, minVerticalLimit, maxVerticalLimit);
        }

        public void RotateSmoothDamp(Vector2 delta, float smoothTime, float deltaTime)
        {
            if (_target == null)
                return;

            ApplyRotateDelta(delta);
            

            // Smooth góc hiện tại tới góc đích
            _currentYaw = Mathf.SmoothDamp(
                _currentYaw,
                _targetYaw,
                ref _yawVelocity,
                smoothTime,
                Mathf.Infinity,
                deltaTime
            );

            _currentPitch = Mathf.SmoothDamp(
                _currentPitch,
                _targetPitch,
                ref _pitchVelocity,
                smoothTime,
                Mathf.Infinity,
                deltaTime
            );

            // Apply rotation
            _target.localRotation = _baseRotation * Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }

        public void RotateSmoothDamp(Vector2 delta, float smoothTime)
        {
            RotateSmoothDamp(delta, smoothTime, Time.deltaTime);
        }
        
        public void RotateLerp(Vector2 delta, float lerpDelta)
        {
            if (_target == null)
                return;

            ApplyRotateDelta(delta);
            

            // Smooth góc hiện tại tới góc đích
            _currentYaw = Mathf.Lerp(_currentYaw, _targetYaw, lerpDelta);
            _currentPitch = Mathf.Lerp(_currentPitch, _targetPitch, lerpDelta);

            // Apply rotation
            _target.localRotation = _baseRotation * Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }
        
        public void RotateToward(Vector2 delta, float maxDelta)
        {
            if (_target == null)
                return;

            ApplyRotateDelta(delta);
            

            // Smooth góc hiện tại tới góc đích
            _currentYaw = Mathf.MoveTowards(_currentYaw, _targetYaw, maxDelta);
            _currentPitch = Mathf.MoveTowards(_currentPitch, _targetPitch, maxDelta);

            // Apply rotation
            _target.localRotation = _baseRotation * Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        }
    }
}