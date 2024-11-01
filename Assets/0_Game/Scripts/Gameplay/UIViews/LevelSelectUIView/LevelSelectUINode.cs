using System;
using JSAM;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUINode : MonoBehaviour
{
    [Header("UI OBJECTS")]
    [SerializeField] private Button _button;
    [SerializeField] private TextMeshProUGUI _levelIndexTMP;
    [SerializeField] private TextMeshProUGUI _completeTMP;
    
    private int _levelIndex;
    private INodeClickHandler _nodeClickHandler;

    public int LevelIndex => _levelIndex;

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnClick);
    }

    public void Init(int levelIndex, INodeClickHandler nodeClickHandler)
    {
        _levelIndex = levelIndex;
        _nodeClickHandler = nodeClickHandler;
        _levelIndexTMP.text = levelIndex.ToString();
    }
    
    public void UpdateComplete(bool isComplete)
    {
        _completeTMP.gameObject.SetActive(isComplete);
    }

    private void OnClick()
    {
        AudioManager.PlaySound(ESound.ClickSoundSO);
        _nodeClickHandler.OnNodeClicked(_levelIndex);
    }
}