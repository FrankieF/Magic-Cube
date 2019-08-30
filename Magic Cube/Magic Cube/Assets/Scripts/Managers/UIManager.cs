using UnityEngine;

public class UIManager : MonoBehaviour
{
    private MenuUIController _menu;
    private GameUIController _game;
    
    private void Awake()
    {
        ServiceManager.Register(this, GetComponent<UIManager>());
        GameEventsManager.Register(GameEventConstants.ON_RETURN_TO_MENU, SetupMenuUI);
    }

    private void OnDestroy()
    {
        GameEventsManager.Unregister(GameEventConstants.ON_RETURN_TO_MENU, SetupMenuUI);
    }

    public void RegisterMenuUI(MenuUIController menu)
    {
        _menu = menu;
    }
    
    public void RegisterGameUI(GameUIController game)
    {
        _game = game;
    }
    
    public void SetUpGameUI()
    {
        _menu.TurnOffUI();
        _game.TurnOnUI();
    }

    public void SetupMenuUI()
    {
        _game.TurnOffUI();
        _menu.TurnOnUI();
    }

    public void ShowWinState()
    {
        _game.SetToWinState();
    }

    public void ShowWinMessage()
    {
        _game.ShowWinMessage();
    }
}
