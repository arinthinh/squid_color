using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SquidColorSelector : MonoBehaviour
{
    [SerializeField] private SquidAvatarController _avatarController;
    [SerializeField] private SquidChangeColorButton _buttonPrefab;
    [SerializeField] private RectTransform _switchColorButtonPanel;

    private ObjectPool<SquidChangeColorButton> _buttonPool;
    private List<SquidChangeColorButton> _curButtons;

    private void Start()
    {
        _buttonPool = UnityObjectPoolCreator.CreatePool(_buttonPrefab, _switchColorButtonPanel);
    }

    public void CreateChangeColorButtons(List<EColor> colors)
    {
    }
}