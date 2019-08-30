using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public interface ICommandManager
{
    bool AcceptInput();
    void Move(ICommand command);
    void MoveWithoutSaving(ICommand command);
    void Undo();
    void SaveCommands();
    void LoadCommands(int cubeSize = 3);
}

public interface ICommand
{
    void Run(Action onComplete);
    bool AllowUndo();
    string WriteDataToString();
    ICommand GetReverseMove();
    
}

public class CommandManager : MonoBehaviour, ICommandManager
{
    private bool _acceptInput = true;
    private LinkedList<ICommand> _commands = new LinkedList<ICommand>();

    private void Awake()
    {
        ServiceManager.Register(this, GetComponent<CommandManager>());
    }

    private void OnDestroy()
    {
        ServiceManager.Unregister(this);
    }

    public LinkedList<ICommand> GetCommands()
    {
        return _commands;
    }

    public bool AcceptInput()
    {
        return _acceptInput;
    }

    public void Move(ICommand command)
    {
        if (!_acceptInput)
        {
            return;
        }
        if (command == null)
        {
            LogManager.LogError("Tried to move with a null command.");
            return;
        }
        _acceptInput = false;
        _commands.AddFirst(command);
        command.Run(() => _acceptInput = true);
    }

    public void MoveWithoutSaving(ICommand command)
    {
        if (!_acceptInput)
        {
            return;
        }
        if (command == null)
        {
            LogManager.LogError("Tried to move with a null command.");
            return;
        }

        _acceptInput = false;
        command.Run(() => _acceptInput = true);
    }
    
    public void Undo()
    {
        if (!_acceptInput)
        {
            return;
        }
        if (_commands.Count > 0 && _commands.First.Value.AllowUndo())
        {
            _acceptInput = false;
            _commands.First.Value.GetReverseMove().Run(() => _acceptInput = true);
            _commands.RemoveFirst();
        }
        else
        {
            LogManager.LogError("Tried to undo command with empty command list!");
        }
    }

    public void SaveCommands()
    {
        GameDataManager.SaveCommands(_commands, ServiceManager.Get<CubesManager>().GetCurrentCubeSize());
    }

    public void LoadCommands(int cubeSize = 3)
    {
        _commands.Clear();
        _commands = GameDataManager.LoadCommands(cubeSize);
    }

    public void ClearCommandList()
    {
        _commands.Clear();
    }
}

public class MoveCubeCommand : ICommand
{
    private static char Delimter = '_';
    private static Dictionary<string, Vector3> _stringToDirection = new Dictionary<string, Vector3>()
    {
        {"up", Vector3.up},
        {"down", Vector3.down},
        {"left", Vector3.left},
        {"right", Vector3.right},
        {"forward", Vector3.forward},
        {"back", Vector3.back}
    };

    private static Dictionary<Vector3, string> _directionToString = new Dictionary<Vector3, string>()
    {
        {Vector3.up, "up"},
        {Vector3.down, "down"},
        {Vector3.left, "left"},
        {Vector3.right, "right"},
        {Vector3.forward, "forward"},
        {Vector3.back, "back"},
    };

    public bool Shuffling;
    public Vector3 Direction;

    public SelectionTriggerController SelectionTrigger;

    public MoveCubeCommand(SelectionTriggerController selection, Vector3 direction, bool shuffling)
    {
        SelectionTrigger = selection;
        Direction = direction;
        Shuffling = shuffling;
    }

    public ICommand GetReverseMove()
    {
        return new MoveCubeCommand(SelectionTrigger, Direction * -1, Shuffling);
    }

    public string WriteDataToString()
    {
        var sb = new StringBuilder();
        if (SelectionTrigger != null)
        {
            sb.Append(SelectionTrigger.name).Append(Delimter);
        }
        sb.Append(Shuffling ? "1" : "0").Append(Delimter);
        if (_directionToString.ContainsKey(Direction))
        {
            sb.Append(_directionToString[Direction]);
        }
        return sb.ToString();
    }


    public bool AllowUndo()
    {
        return !Shuffling;
    }
    public void Run(Action onComplete)
    {
        SelectionTrigger.StartCoroutine(SelectionTrigger.Rotate(SelectionTrigger.GetListOfCubesToRotate(), Direction, 
            Shuffling ? Consts.SHUFFLE_SPEED : Consts.ROTATE_SPEED, onComplete));
    }

    public static MoveCubeCommand BuildFromString(string command)
    {
        MoveCubeCommand temp = null;
        string[] vars = command.Split(Delimter);
        if (vars.Length == 3)
        {
            var trigger = TriggerManager.StringToTrigger.ContainsKey(vars[0]) ? 
                TriggerManager.StringToTrigger[vars[0]] : 
                TriggerManager.GetEmptyTrigger();
            var shuffling = vars[1].Equals("1");
            var direction = _stringToDirection.ContainsKey(vars[2]) ? _stringToDirection[vars[2]] : Vector3.zero;
            temp = new MoveCubeCommand(trigger, direction, shuffling);
        }
        return temp;
    }
}
