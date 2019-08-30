using System.Collections;
using UnityEngine;
using Random = UnityEngine.Random;

public class MagicCubeController : MonoBehaviour
{
    [SerializeField] private int _MinimumDragThreshold = 5;
    
    private int _targetCubeLayer;
    private float _mouseDistance;
    private bool _isMouseMoving;
    private bool _canControl = false;
    private Vector3 _mousePosition = Vector3.zero;
    private Vector3 _mouseDirection = Vector3.zero;
    private CubeInfo _selectedCube;

    private MagicCubeUI _magicCubeUi => ServiceManager.Get<MagicCubeUI>();
    private ICommandManager _commandManager => ServiceManager.Get<CommandManager>();

    private void Awake()
    {
        ServiceManager.Register(this, GetComponent<MagicCubeController>());
    }

    public void StartShuffle( int size = 3)
    {
        _canControl = false;
        StartCoroutine(Shuffle(size));
    }

    private IEnumerator Shuffle(int size)
    {
        _canControl = false;
        var max = size <= 4 ? Consts.MAX_SHUFFLES_SMALL : Consts.MAX_SHULLES_LARGE;
        var total = Random.Range(size * 2, max);
        int count = 0;
        while (count < total)
        {
            ShuffleCube();
            count++;
            while (!_commandManager.AcceptInput())
            {
                yield return null;
            }
        }
        GameEventsManager.BroadcastMessage(GameEventConstants.ON_SHUFFLE_COMPLETE);
        _canControl = true;
        StartCoroutine(CheckMouseInput());
    }

    public void StartCheckingInput()
    {
        _canControl = true;
        StartCoroutine(CheckMouseInput());
    }

    private IEnumerator CheckMouseInput()
    {
        while (_canControl)
        {
            HandleMouseInput();
            yield return null;
        }
    }

    private void ShuffleCube()
    {
        var random = Random.Range(Consts.RIGHT_OF_CUBE_LAYER, Consts.BACK_OF_CUBE_LAYER);
        var clockwise =  (random % 2) == 0;
        var xAxis = (Random.Range(1, 2) % 2) == 1;
        SelectionTriggerController trigger = TriggerManager.GetEmptyTrigger();
        Vector3 direction = Vector3.zero;
        if (xAxis)
        {
            if (random != Consts.TOP_OF_CUBE_LAYER && random != Consts.BOTTOM_OF_CUBE_LAYER)
            {
                trigger = TriggerManager.GetRandomTrigger(Consts.X_TRIGGER_AXIS);
                direction = Vector3.up;
            }
            else
            {
                if ((Random.Range(1, 2) % 2) == 1)
                {
                    trigger = TriggerManager.GetRandomTrigger(Consts.Y_TRIGGER_AXIS);
                    direction = Vector3.right;
                }
                else
                {
                    trigger = TriggerManager.GetRandomTrigger(Consts.Z_TRIGGER_AXIS);
                    direction = Vector3.forward;
                }
            }
        }
        else 
        {
            switch (random)
            {
                case Consts.FRONT_OF_CUBE_LAYER:
                    trigger = TriggerManager.GetRandomTrigger(Consts.Y_TRIGGER_AXIS);
                    direction = Vector3.right;
                    break;
                case Consts.RIGHT_OF_CUBE_LAYER:
                    trigger = TriggerManager.GetRandomTrigger(Consts.Z_TRIGGER_AXIS);
                    direction = Vector3.back;
                    break;
                case Consts.TOP_OF_CUBE_LAYER:
                case Consts.BOTTOM_OF_CUBE_LAYER:
                    var angle = Random.Range(1, 3);
                    // Rotating from the front
                    if (angle == 1)
                    {
                        trigger = TriggerManager.GetRandomTrigger(Consts.Y_TRIGGER_AXIS);
                        direction = Vector3.right;
                    }
                    // Rotating from the side
                    else if (angle == 2)
                    {
                        trigger = TriggerManager.GetRandomTrigger(Consts.Z_TRIGGER_AXIS);
                        direction = (Random.Range(1, 2) % 2) == 0 ? Vector3.forward : Vector3.back;
                    }
                    // Rotating from the back
                    else
                    {
                        trigger = TriggerManager.GetRandomTrigger(Consts.Y_TRIGGER_AXIS);
                        direction = Vector3.left;
                    }
                    break;
                case Consts.LEFT_OF_CUBE_LAYER:
                    trigger = TriggerManager.GetRandomTrigger(Consts.Z_TRIGGER_AXIS);
                    direction = Vector3.forward;
                    break;
                case Consts.BACK_OF_CUBE_LAYER:
                    trigger = TriggerManager.GetRandomTrigger(Consts.Y_TRIGGER_AXIS);
                    direction = Vector3.left;
                    break;
                default:
                    break;
            }
        }
        if (clockwise)
        {
            direction *= -1;
        }
        _commandManager.Move(new MoveCubeCommand(trigger, direction,true));
    }

    private void HandleMouseInput()
    {
        if (_commandManager.AcceptInput() && Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 1000))
            {
                if (hit.collider.gameObject.layer >= 10 && hit.collider.gameObject.layer <= 15)
                {
                    var distance = Camera.main.transform.position - Vector3.zero;
                    var com = hit.collider.gameObject.GetComponent<SelectionTriggerController>();
                    _selectedCube = hit.collider.gameObject.GetComponent<CubeSelectionReference>().Cube;
                    _isMouseMoving = true;
                    _mousePosition = Input.mousePosition;
                    _targetCubeLayer = hit.collider.gameObject.layer;
                    return;
                }
            }
        }

        if (Input.GetMouseButtonUp(0) && _isMouseMoving)
        {
            _isMouseMoving = false;
        }

        if (_isMouseMoving)
        {
            UpdateMouseVariables();
            if (_mouseDistance > _MinimumDragThreshold)
            {
                Move(false);
            }
        }
    }

    private void Move(bool shuffling)
    {
        MoveCubeCommand command = null;
        if (Mathf.Abs(_mouseDirection.x) > Mathf.Abs(_mouseDirection.y))
        {
            // X is the same for all the sides
            if (_targetCubeLayer != Consts.TOP_OF_CUBE_LAYER && _targetCubeLayer != Consts.BOTTOM_OF_CUBE_LAYER)
            {
                var axis = _mouseDirection.x < 0 ? Vector3.up : Vector3.down;
                command = new MoveCubeCommand(_selectedCube.XSelection, axis, shuffling);
            }
            // Top and bottom have different X
            else
            {
                command = HandleTopAndBottomXRotation(shuffling);
            }
        }
        else
        {
            // Y needs to be handled on a side by side basis
            Vector3 axis;
            switch (_targetCubeLayer)
            {
                case Consts.FRONT_OF_CUBE_LAYER:
                    axis = _mouseDirection.y < 0 ? Vector3.right : Vector3.left;
                    command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
                    break;
                case Consts.RIGHT_OF_CUBE_LAYER:
                    axis = _mouseDirection.y < 0 ? Vector3.back : Vector3.forward;
                    command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
                    break;
                case Consts.TOP_OF_CUBE_LAYER:
                case Consts.BOTTOM_OF_CUBE_LAYER:
                        command = HandleTopAndBottomYRotation(shuffling);
                        break;
                case Consts.LEFT_OF_CUBE_LAYER:
                    axis = _mouseDirection.y < 0 ? Vector3.forward : Vector3.back;
                    command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
                    break;
                case Consts.BACK_OF_CUBE_LAYER:
                    axis = _mouseDirection.y < 0 ? Vector3.left : Vector3.right;
                    command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
                    break;
                default:
                    break;
            }
        }
        if (command != null)
        {
            _commandManager.Move(command);
        }
    }
    
    private MoveCubeCommand HandleTopAndBottomXRotation(bool shuffling)
    {
        MoveCubeCommand command = null;
        if (_targetCubeLayer != Consts.TOP_OF_CUBE_LAYER && _targetCubeLayer != Consts.BOTTOM_OF_CUBE_LAYER) { return command; }
        var cameraPosition = Camera.main.transform.position;
        var cubePosition = Vector3.forward;
        var angle = Vector3.Angle(cubePosition, cameraPosition.normalized);
        
        if (_targetCubeLayer == Consts.TOP_OF_CUBE_LAYER)
        {
            // Rotating from the front
                if (angle < Consts.FRONT_OF_CUBE_ANGLE)
                {
                    Vector3 axis = _mouseDirection.x < 0 ? Vector3.back : Vector3.forward;
                    command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
                }
                // Rotating from the side
                else if (angle < Consts.SIDE_OF_CUBE_ANGLE)
                {
                    // Left side
                    if ((cubePosition.x - cameraPosition.x) > 0)
                    {
                        Vector3 axis = _mouseDirection.x < 0 ? Vector3.right : Vector3.left;
                        command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
                    }
                    // Right side
                    else
                    {
                        Vector3 axis = _mouseDirection.x < 0 ? Vector3.left : Vector3.right;
                        command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
                    }
                }
                // Rotating from the back
                else
                {
                    Vector3 axis = _mouseDirection.x < 0 ? Vector3.forward : Vector3.back;
                    command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
                }
        }
        else if (_targetCubeLayer == Consts.BOTTOM_OF_CUBE_LAYER)
        {
            // Rotating from the front
            if (angle < Consts.FRONT_OF_CUBE_ANGLE)
            {
                Vector3 axis = _mouseDirection.x < 0 ? Vector3.forward : Vector3.back;
                command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
            }
            // Rotating from the side
            else if (angle < Consts.SIDE_OF_CUBE_ANGLE)
            {
                // Left side
                if ((cubePosition.x - cameraPosition.x) < 0)
                {
                    Vector3 axis = _mouseDirection.x < 0 ? Vector3.right : Vector3.left;
                    command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
                }
                // Right side
                else
                {
                    Vector3 axis = _mouseDirection.x < 0 ? Vector3.left : Vector3.right;
                    command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
                }
            }
            // Rotating from the back
            else
            {
                Vector3 axis = _mouseDirection.x < 0 ? Vector3.back : Vector3.forward;
                command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
            }
        }
        return command;
    }

    private MoveCubeCommand HandleTopAndBottomYRotation(bool shuffling)
    {
        
        MoveCubeCommand command = null;
        if (_targetCubeLayer != Consts.TOP_OF_CUBE_LAYER && _targetCubeLayer != Consts.BOTTOM_OF_CUBE_LAYER) { return command; }
        var cameraPosition = Camera.main.transform.position;
        var cubePosition = Vector3.forward;
        var angle = Vector3.Angle(cubePosition, cameraPosition.normalized);
        Vector3 axis;
        
        if (_targetCubeLayer == Consts.TOP_OF_CUBE_LAYER)
        {
            // Rotating from the front
            if (angle < Consts.FRONT_OF_CUBE_ANGLE)
            {
                axis = _mouseDirection.y < 0 ? Vector3.right : Vector3.left;
                command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
            }
            // Rotating from the side
            else if (angle < Consts.SIDE_OF_CUBE_ANGLE)
            {
                // Left side
                if ((cubePosition.x - cameraPosition.x) > 0)
                {
                    axis = _mouseDirection.y < 0 ? Vector3.forward : Vector3.back;
                    command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
                }
                // Right side
                else
                {
                    axis = _mouseDirection.y < 0 ? Vector3.back : Vector3.forward;
                    command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
                }
            }
            // Rotating from the back
            else
            {
                axis = _mouseDirection.y < 0 ? Vector3.left : Vector3.right;
                command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
            }
        }
        else if (_targetCubeLayer == Consts.BOTTOM_OF_CUBE_LAYER)
        {
            // Rotating from the front
            if (angle < Consts.FRONT_OF_CUBE_ANGLE)
            {
                axis = _mouseDirection.y < 0 ? Vector3.right : Vector3.left;
                command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
            }
            // Rotating from the side
            else if (angle < Consts.SIDE_OF_CUBE_ANGLE)
            {
                // Left side
                if ((cubePosition.x - cameraPosition.x) > 0)
                {
                    axis = _mouseDirection.y < 0 ? Vector3.forward : Vector3.back;
                    command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
                }
                // Right side
                else
                {
                    axis = _mouseDirection.y < 0 ? Vector3.back : Vector3.forward;
                    command = new MoveCubeCommand(_selectedCube.ZSelection, axis, shuffling);
                }
            }
            // Rotating from the back
            else
            {
                axis = _mouseDirection.y < 0 ? Vector3.left : Vector3.right;
                command = new MoveCubeCommand(_selectedCube.YSelection, axis, shuffling);
            }
        }
        return command;
    }
    
    private void UpdateMouseVariables()
    {
        _mouseDistance = Vector3.Distance(Input.mousePosition, _mousePosition);
        _mouseDirection = (Input.mousePosition - _mousePosition);
    }
}