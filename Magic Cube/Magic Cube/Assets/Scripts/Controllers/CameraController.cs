using System;
using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour 
{
    [SerializeField] 
    private Transform _Target;
    
    [SerializeField]
    [Range(0,100)]
    private float _Distance = 20.0f;
    
    [SerializeField]
    [Range(0, 100)] 
    private float _XSpeed = 10.0f;
    
    [SerializeField]
    [Range(0, 100)] 
    private float _YSpeed = 10.0f;
    
    [SerializeField]
    [Range(-90,0)] 
    private float _MinLimitY = -90.0f;
    
    [SerializeField]
    [Range(0, 90)] 
    private float _MaxLimitY = 90.0f;
    
    [SerializeField]
    [Range(0, 100)] 
    private float _MinDistance = 0.0f;
    
    [SerializeField]
    [Range(0, 100)] 
    private float _DistanceMax = 40.0f;
    
    [SerializeField]
    [Range(0, 100)] 
    private float _SmoothTime = 40.0f;

    private float _xRotation = 0.0f;
    private float _yRotation = 0.0f;
    private float _xVelocity = 0.0f;
    private float _yVelocity = 0.0f;
    private bool _allowControl;
    private Vector3 _MenuPosition;
    private Quaternion _MenuRotation;

    private void Awake()
    {
        ServiceManager.Register(this, GetComponent<CameraController>());
        _MenuPosition = transform.position;
        _MenuRotation = transform.rotation;
    }
    
    private void Start()
    {
        var eulerAngles = transform.eulerAngles;
        _yRotation = eulerAngles.y;
        _xRotation = eulerAngles.x;
        _Target = ServiceManager.Get<MagicCubeController>().transform;
    }

    public void SetInGame(bool inGame)
    {
        _allowControl = inGame;
    }
    
    public IEnumerator GameState()
    {
        while (_allowControl)
        {
            var mouseInput = new Vector2(Input.GetAxis(Consts.MOUSE_X_INPUT), Input.GetAxis(Consts.MOUSE_Y_INPUT));
            MoveCamera(Input.GetButton(Consts.FIRE_BUTTON_INPUT), mouseInput);
            yield return null;
        }
    }

    public void GoToMenuPosition()
    {
        _allowControl = false;
        transform.position = _MenuPosition;
        transform.rotation = _MenuRotation;
    }
    
    public void MoveCamera(bool move, Vector3 mouseInput)
    {
        if (_Target == null)
        {
            LogManager.LogError("Target is null!");
            return;
        }
        if (move)
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (!Physics.Raycast(ray, 1000))
            {
                _xVelocity += _XSpeed * mouseInput.x * _Distance * 0.02f;
                _yVelocity += _YSpeed * mouseInput.y * _Distance * 0.02f;
            }
               
        }
        _yRotation += _xVelocity;
        _xRotation -= _yVelocity;
        _xRotation = ClampAngle(_xRotation, _MinLimitY, _MaxLimitY);
        var rotation = Quaternion.Euler(_xRotation, _yRotation, 0);

        _Distance = Mathf.Clamp(_Distance - Input.GetAxis(Consts.MOUSE_SCROLL_INPUT) * 5, _MinDistance, _DistanceMax);

        if (Physics.Linecast(_Target.position, transform.position, out var hit, 8))
        {
            _Distance -= hit.distance;
        }
        // Todo: Just multiple by negative z value
        var negDistance = new Vector3(0.0f, 0.0f, -_Distance);
        var position = rotation * negDistance + _Target.position;
        var myTransform = transform;
        myTransform.rotation = rotation;
        myTransform.position = position;
        _xVelocity = Mathf.Lerp(_xVelocity, 0, Time.deltaTime * _SmoothTime);
        _yVelocity = Mathf.Lerp(_yVelocity, 0, Time.deltaTime * _SmoothTime);
    }

    public IEnumerator WinAnimation(Action onComplete = null)
    {
        _allowControl = false;
        while (_Distance < _DistanceMax + _DistanceMax * 0.5f)
        {
            var rotation = Quaternion.Euler(_xRotation, _yRotation, 0);

            _Distance = _Distance + 10 * Time.deltaTime;
            // Todo: Just multiple by negative z value
            var negDistance = new Vector3(0.0f, 0.0f, -_Distance);
            var position = rotation * negDistance + _Target.position;
            var myTransform = transform;
            myTransform.rotation = rotation;
            myTransform.position = position;
            _xVelocity = Mathf.Lerp(_xVelocity, 0, Time.deltaTime * _SmoothTime);
            _yVelocity = Mathf.Lerp(_yVelocity, 0, Time.deltaTime * _SmoothTime);
            yield return null;
        }

        float time = 0.0f;

        while (time < Consts.CAMERA_ROTATE_TIME)
        {
            time += Time.deltaTime;
            transform.RotateAround(Vector3.zero, Vector3.up, Consts.CAMERA_ROTATE_SPEED * Time.deltaTime);
            yield return null;
        }

        _allowControl = true;
        StartCoroutine(GameState());
        onComplete?.Invoke();
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        angle %= 360;
        return Mathf.Clamp(angle, min, max);
    }
}
