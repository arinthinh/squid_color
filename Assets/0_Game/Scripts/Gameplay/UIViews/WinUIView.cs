using Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class WinUIView : UIView
{
    [Header("UI OBJECTS")]
    [SerializeField] private Button _homeButton;
    [SerializeField] private Button _nextButton;


    protected void OnEnable()
    {
        _homeButton.onClick.AddListener(OnHomeButtonClick);
        _nextButton.onClick.AddListener(OnNextButtonClick);
    }

    protected void OnDisable()
    {
        _homeButton.onClick.RemoveListener(OnHomeButtonClick);
        _nextButton.onClick.RemoveListener(OnNextButtonClick);
    }

    private void OnHomeButtonClick()
    {
        Hide();
        GameplayController.Instance.ExitLevel();
    }

    private void OnNextButtonClick()
    {
        Hide();
        GameplayController.Instance.PlayNextLevel();
    }
}