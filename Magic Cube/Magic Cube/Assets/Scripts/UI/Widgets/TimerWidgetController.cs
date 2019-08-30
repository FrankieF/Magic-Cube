using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TimerWidgetController : MonoBehaviour
{
    [SerializeField] private Text _TimerText;

    private bool _timerIsActive = false;
    private MagicCubeLogic _logic;
    
    public void Init()
    {
        if (_logic == null)
        {
            _logic = ServiceManager.Get<MagicCubeLogic>();
        }

        if (_TimerText == null)
        {
            _timerIsActive = false;
            LogManager.LogError("Timer Text is null, not starting update timer routine.");
            return;
        }
        gameObject.SetActive(true);
        _timerIsActive = true;
        StartCoroutine(UpdateTimer());
    }

    public string GetTime()
    {
        return _TimerText.text;
    }
    
    public void TurnOffTimer()
    {
        _timerIsActive = false;
        gameObject.SetActive(false);
    }

    private IEnumerator UpdateTimer()
    {
        while (_timerIsActive)
        {
            var time = _logic._gameTime;
            var minutes = Mathf.FloorToInt(time / 60F);
            var seconds = Mathf.FloorToInt(time - minutes * 60);
            _TimerText.text = string.Format(Consts.TIMER_MM_SS_FORMAT_TEXT, minutes, seconds);
            yield return new WaitForSeconds(1.0f);
        }
    }
}
