using UnityEngine;

public class GameUIController : MonoBehaviour
{
    [SerializeField] private GameObject _MenuButton;
    [SerializeField] private GameObject _UndoButton;
    [SerializeField] private GameObject _TimerButton;
    [SerializeField] private TimerWidgetController _TimerWidget;
    [SerializeField] private ConfirmDialougeWidgetController _DialougeController;

    private MagicCubeLogic _logicController;

    public void Start()
    {
        _logicController = ServiceManager.Get<MagicCubeLogic>();
        ServiceManager.Get<UIManager>().RegisterGameUI(this);
    }

    public void TurnOnUI()
    {
        _TimerWidget.gameObject.SetActive(true);
        _TimerWidget.Init();
        _DialougeController.gameObject.SetActive(false);
        _MenuButton.SetActive(true);
        _UndoButton.SetActive(true);
        _TimerButton.SetActive(true);
        _MenuButton.SetActive(true);
    }

    public void TurnOffUI()
    {
        _TimerWidget.gameObject.SetActive(false);
        _DialougeController.gameObject.SetActive(false);
        _MenuButton.SetActive(false);
        _UndoButton.SetActive(false);
        _TimerButton.SetActive(false);
    }

    public void SetToWinState()
    {
        _UndoButton.SetActive(false);
    }

    public void ShowWinMessage()
    {
        _DialougeController.gameObject.SetActive(true);
        var config = new ConfirmDialougeWidgetController.Config
        {
            Text = string.Format(Consts.PLAYER_WON_TEXT, _TimerWidget.GetTime()),
            OnConfirm = () =>
            {
                _DialougeController.gameObject.SetActive(true);
                var cubeSize = ServiceManager.Get<CubesManager>().GetCurrentCubeSize();
                if (GameDataManager.CheckIfSaveDataExists(cubeSize))
                {
                    GameDataManager.DeleteCurrentSave(cubeSize);
                }
                GameEventsManager.BroadcastMessage(GameEventConstants.ON_RETURN_TO_MENU);
            },
            OnClose = () =>
            {
                
            }
        };
        _DialougeController.Init(config);
    }
    
    public void ShowTimer()
    {
        if (_TimerWidget != null)
        {
            if (_TimerWidget.gameObject.activeSelf)
            {
                _TimerWidget.TurnOffTimer();
            }
            else
            {
                _TimerWidget.gameObject.SetActive(true);
                _TimerWidget.Init();
            }
        }
    }
    
    public void UndoMove()
    {
        ServiceManager.Get<CommandManager>().Undo();
    }

    public void OnClickMenu()
    {
        if (_DialougeController == null) { return; }
        _logicController.SwitchToGameMenuState();
        _DialougeController.gameObject.SetActive(true);
        var config = new ConfirmDialougeWidgetController.Config
        {
            Text = Consts.RETURN_TO_MENU_TEXT,
            OnConfirm = () =>
            {
                _DialougeController.gameObject.SetActive(true);
                if (!_logicController.DidPlayerWin())
                {
                    GameDataManager.SaveCommands(ServiceManager.Get<CommandManager>().GetCommands(), ServiceManager.Get<CubesManager>().GetCurrentCubeSize());
                }
                GameEventsManager.BroadcastMessage(GameEventConstants.ON_RETURN_TO_MENU);
            },
            OnClose = () =>
            {
                _logicController.SwitchToPlayState();
            }
        };
        _DialougeController.Init(config);
    }
}
