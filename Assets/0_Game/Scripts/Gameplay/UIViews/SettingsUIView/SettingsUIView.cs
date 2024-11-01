using DG.Tweening;
using JSAM;
using Redcode.Extensions;
using Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUIView : UIView
{
    [Header("CONTENT PANEL")]
    [SerializeField] private RectTransform _contentPanel;
    [SerializeField] private Button _closeButton;

    [Header("SETTINGS PANEL")]
    [SerializeField] private SwitchButton _soundSwitch;
    [SerializeField] private SwitchButton _musicSwitch;
    [SerializeField] private Button _quitButton;

    #region UNITY METHODS

    private void OnEnable()
    {
        _closeButton.onClick.AddListener(OnCloseButtonClick);
        _soundSwitch.onClick.AddListener(OnSoundSwitchClick);
        _musicSwitch.onClick.AddListener(OnMusicSwitchClick);
        _quitButton.onClick.AddListener(OnQuitButtonClick);
    }

    private void OnDisable()
    {
        _closeButton.onClick.RemoveListener(OnCloseButtonClick);
        _soundSwitch.onClick.RemoveListener(OnSoundSwitchClick);
        _musicSwitch.onClick.RemoveListener(OnMusicSwitchClick);
        _quitButton.onClick.RemoveListener(OnQuitButtonClick);
    }

    #endregion

    #region OVERRIDES

    public void Show(bool isShowQuitButton)
    {
        base.Show();
        _canvasGroup.interactable = false;
        _contentPanel.DOKill();
        _contentPanel.SetAnchoredPositionY(_contentPanel.anchoredPosition.y + 1000);
        _contentPanel.DOAnchorPosY(_contentPanel.anchoredPosition.y - 1000, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => _canvasGroup.interactable = true);

        _soundSwitch.IsOn = !AudioManager.SoundMuted;
        _musicSwitch.IsOn = !AudioManager.MusicMuted;
        
        _quitButton.gameObject.SetActive(isShowQuitButton);
    }

    public override void Hide()
    {
        base.Hide();
    }

    #endregion

    #region EVENT LISTENERS

    private void OnCloseButtonClick()
    {
        AudioManager.PlaySound(ESound.ClickSoundSO);
        Hide();
    }

    private void OnSoundSwitchClick()
    {
        AudioManager.SoundMuted = !AudioManager.SoundMuted;
        _soundSwitch.IsOn = !AudioManager.SoundMuted;
    }

    private void OnMusicSwitchClick()
    {
        AudioManager.MusicMuted = !AudioManager.MusicMuted;
        _musicSwitch.IsOn = !AudioManager.MusicMuted;
    }

    private void OnQuitButtonClick()
    {
        AudioManager.PlaySound(ESound.ClickSoundSO);
        Hide();
        GameplayController.Instance.ExitLevel(true);
    }

    #endregion
}