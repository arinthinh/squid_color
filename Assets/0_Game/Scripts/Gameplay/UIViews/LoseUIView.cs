using DG.Tweening;
using JSAM;
using Redcode.Extensions;
using Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class LoseUIView : UIView
{
    [Header("UI OBJECTS")]
    [SerializeField] private RectTransform _contentPanel;
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _retryButton;

    protected void OnEnable()
    {
        _homeButton.onClick.AddListener(OnHomeButtonClick);
        _retryButton.onClick.AddListener(OnRetryButtonClick);
    }

    protected void OnDisable()
    {
        _homeButton.onClick.RemoveListener(OnHomeButtonClick);
        _retryButton.onClick.RemoveListener(OnRetryButtonClick);
    }

    public override void Show()
    {
        base.Show();
        AudioManager.PlaySound(ESound.LoseSoundSO);
        _canvasGroup.interactable = false;
        _contentPanel.DOKill();
        _contentPanel.SetAnchoredPositionY(_contentPanel.anchoredPosition.y + 1000);
        _contentPanel.DOAnchorPosY(_contentPanel.anchoredPosition.y - 1000, 0.5f)
            .SetEase(Ease.OutBack)
            .OnComplete(() => _canvasGroup.interactable = true);
    }
    
    private void OnHomeButtonClick()
    {
        AudioManager.PlaySound(ESound.ClickSoundSO);
        Hide();
        GameplayController.Instance.ExitLevel();
    }

    private void OnRetryButtonClick()
    {
        AudioManager.PlaySound(ESound.ClickSoundSO);
        Hide();
        GameplayController.Instance.ReplayLevel();
    }
}
