using UnityEngine;

public class MenuUIController : MonoBehaviour 
{
    [SerializeField] private GameObject _MenuButtons;
    [SerializeField] private GameObject _MenuCubes;
    
    private MagicCubeLogic _logicController;

    private void Start()
    {
        _logicController = ServiceManager.Get<MagicCubeLogic>();
        ServiceManager.Get<UIManager>().RegisterMenuUI(this);
    }

    public void TurnOnUI()
    {
        _MenuButtons.SetActive(true);
        _MenuCubes.SetActive(true);
    }

    public void TurnOffUI()
    {
        _MenuButtons.SetActive(false);
        _MenuCubes.SetActive(false);
    }


    public void QuitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}
