using System;
using UnityEngine;
using UnityEngine.UI;

public class ConfirmDialougeWidgetController : MonoBehaviour
{
    public class Config
    {
        public string Text;
        public Action OnConfirm;
        public Action OnClose;
    }

    [SerializeField] private Text _ConfirmText;

    private Config _config;
    private Action _onConfirm;
    private Action _onClose;

    public void Init(Config config)
    {
        if (config == null) { return; }
        _config = config;
        if (_ConfirmText != null)
        {
            _ConfirmText.text = _config.Text;
        }
        _onConfirm = _config.OnConfirm;
        _onClose = _config.OnClose;
    }

    public void OnConfirm()
    {
        _onConfirm?.Invoke();
        gameObject.SetActive(false);
    }

    public void Close()
    {
        _onClose?.Invoke();
        gameObject.SetActive(false);
    }
}
