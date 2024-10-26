using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class LevelSelectUINode : MonoBehaviour
{
    private int _levelIndex;
    private Button _button;
    private INodeClickHandler _nodeClickHandler;

    private void Awake()
    {
        _button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnClick);
    }

    private void OnDisable()
    {
        _button.onClick.AddListener(OnClick);
    }

    public void Init(INodeClickHandler nodeClickHandler)
    {
        _nodeClickHandler = nodeClickHandler;
    }

    private void OnClick()
    {
        _nodeClickHandler.OnNodeClicked(_levelIndex);
    }
}