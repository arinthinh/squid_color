using System;
using Sirenix.OdinInspector;
using Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUIView : UIView, INodeClickHandler
{
    [SerializeField] private Button _playButton;
    
    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlayButtonClicked);
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlayButtonClicked);
    }

    private void OnPlayButtonClicked()
    {
        GameplayController.Instance.PlayLevel(0);
    }
    
    public void OnNodeClicked(int level)
    {
        GameplayController.Instance.PlayLevel(level);
    }
}

public interface INodeClickHandler
{
    void OnNodeClicked(int level);
}
