using UnityEngine;

public class StartGameWidgetController : MonoBehaviour
{
    [SerializeField] private int CubeSize;
    [SerializeField] private GameObject _ContinueButton;

    private void OnEnable()
    {
        if (_ContinueButton != null)
        {
            _ContinueButton.SetActive(GameDataManager.CheckIfSaveDataExists(CubeSize));
        }
    }
    
    public void OnClickStartGame()
    {
        ServiceManager.Get<CubesManager>().LoadCube(CubeSize);
        ServiceManager.Get<UIManager>().SetUpGameUI();
        ServiceManager.Get<MagicCubeLogic>().SwitchToShuffleState();
    }

    public void OnClickContinueGame()
    {
        ServiceManager.Get<CubesManager>().LoadCube(CubeSize, false);
        ServiceManager.Get<UIManager>().SetUpGameUI();
        ServiceManager.Get<CommandManager>().LoadCommands(CubeSize);
        ServiceManager.Get<MagicCubeLogic>().SwitchToLoadState();
    }
}
