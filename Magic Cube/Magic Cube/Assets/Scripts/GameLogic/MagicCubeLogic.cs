using System.Collections;
using UnityEngine;

public enum GameState { Menu, Shuffle, Load, Play, GameMenu, Win }

public class MagicCubeLogic : MonoBehaviour
{
    public GameState GameState { get; set; } = GameState.Menu;
    
    private float _startTime = 0.0f;
    public float _gameTime = 0.0f;
    private bool _playerWon = false;
    private CameraController _camera;
    private CubeInfo[] _cubes;
    private CommandManager _commandManager;
    private CubesManager _cubesManager;

    private void Awake()
    {
        ServiceManager.Register(this, GetComponent<MagicCubeLogic>());
        GameEventsManager.Register(GameEventConstants.ON_SHUFFLE_COMPLETE, SwitchToPlayState);
        GameEventsManager.Register(GameEventConstants.ON_RETURN_TO_MENU, ResetLogic);
    }

    private void OnDestroy()
    {
        GameEventsManager.Unregister(GameEventConstants.ON_SHUFFLE_COMPLETE, SwitchToPlayState);
        GameEventsManager.Unregister(GameEventConstants.ON_RETURN_TO_MENU, ResetLogic);
    }

    private void Start()
    {
        _commandManager = ServiceManager.Get<CommandManager>();
        _cubesManager =  ServiceManager.Get<CubesManager>();
        _camera = Camera.main.GetComponent<CameraController>();
    }

    public void Init(bool isNewGame)
    {
        if (isNewGame)
        {
            _commandManager.ClearCommandList();
        }
    }
    
    private void ResetLogic()
    {
        StopAllCoroutines();
        _camera.GoToMenuPosition();
        GameState = GameState.Menu;
        _gameTime = 0.0f;
        _playerWon = false;
    }

    public void SwitchToShuffleState()
    {
        GameState = GameState.Shuffle;
        _cubes = _cubesManager.GetCubes();
        ServiceManager.Get<MagicCubeController>().StartShuffle();
    }

    public void SwitchToLoadState()
    {
        GameState = GameState.Load;
        _cubes = _cubesManager.GetCubes();
        if (_cubesManager.IsCurrentCubeNew())
        {
            // If we are loading a new cube we have to move it, but if it is in memory it is moved already
            StartCoroutine(PerformPreviousMoves());
        }
        ServiceManager.Get<MagicCubeController>().StartCheckingInput();
        _camera.SetInGame(true);
        StartCoroutine(_camera.GameState());
    }

    public bool DidPlayerWin()
    {
        return _playerWon;
    }

    private IEnumerator PerformPreviousMoves()
    {
        var commands = ServiceManager.Get<CommandManager>().GetCommands();
        if (commands.Count > 0)
        {
            var move = commands.Last;
            while (move.Previous != null)
            {
                _commandManager.MoveWithoutSaving(move.Value);
                move = move.Previous;
                yield return new WaitForSeconds(0.6f);
            }

            _commandManager.MoveWithoutSaving(move.Value);
        }
        SwitchToPlayState();
    }
    
    public void SwitchToPlayState()
    {
        GameState = GameState.Play;
        StartCoroutine(PlayState());
        _camera.SetInGame(true);
        StartCoroutine(_camera.GameState());
    }

    public void SwitchToGameMenuState()
    {
        GameState = GameState.GameMenu;
    }
    
    private IEnumerator MenuState()
    {
        while (true)
        {
            yield return null;
        }
    }

    private IEnumerator ShuffleState()
    {
        while (true)
        {
            yield return null;
        }
    }
    
    private IEnumerator PlayState()
    {
        _gameTime = 0.0f;
        while (true)
        {
            _startTime = Time.time;
            _gameTime += Time.deltaTime;
            if (CheckIfCubeIsSolved())
            {
                GameState = GameState.Win;
                GameEventsManager.BroadcastMessage(GameEventConstants.ON_GAME_WON);
                StartCoroutine(WinState());
                yield break;
            }
            yield return null;
        }
    }

    private IEnumerator WinState()
    {
        _playerWon = true;
        ServiceManager.Get<UIManager>().ShowWinState();
        StartCoroutine(_camera.WinAnimation(() => ServiceManager.Get<UIManager>().ShowWinMessage()));
        while (true)
        {
            yield return null;
        }
    }
    
    private bool CheckIfCubeIsSolved()
    {
        foreach (var cube in _cubes)
        {
            if (!cube.IsSolved())
            {
                return false;
            }
        }
        return true;
    }
}
