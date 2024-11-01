using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JSAM;
using Toolkit.UI;
using UnityEngine;
using UnityEngine.UI;

public class LevelSelectUIView : UIView, INodeClickHandler
{
    [Header("CONFIGS)")]
    [SerializeField] private LevelConfigCollectionSO _levelConfigCollection;

    [Header("UI OBJECTS")]
    [SerializeField] private ScrollRect _nodeScroll;
    [SerializeField] private Button _settingsButton;

    [Header("PREFABS")]
    [SerializeField] private LevelSelectUINode _levelSelectNodePrefab;

    private List<LevelSelectUINode> _nodes = new();

    private void OnEnable()
    {
        _settingsButton.onClick.AddListener(OnSettingsButtonClick);
    }

    private void OnDisable()
    {
        _settingsButton.onClick.RemoveListener(OnSettingsButtonClick);
    }

    private void OnSettingsButtonClick()
    {
        AudioManager.PlaySound(ESound.ClickSoundSO);
        UIManager.Instance.GetView<SettingsUIView>().Show(false);
    }

    public override void Init()
    {
        base.Init();
        var currentLevel = GameDataController.Instance.GameData.CurrentLevel;
        foreach (var level in _levelConfigCollection.Configs)
        {
            var nodeInstance = Instantiate(_levelSelectNodePrefab, _nodeScroll.content);
            nodeInstance.Init(level.Index, this);
            _nodes.Add(nodeInstance);
        }
    }

    public override void Show()
    {
        base.Show();

        var currentLevel = GameDataController.Instance.GameData.CurrentLevel;
        // Show unlocked levels
        _nodes.ForEach(n =>
        {
            var isLevelUnlocked = n.LevelIndex <= currentLevel;
            n.gameObject.SetActive(isLevelUnlocked);
            var isDone = n.LevelIndex < currentLevel;
            n.UpdateComplete(isDone);
        });

        ScrollToCurrentLevel().Forget();
    }

    private async UniTask ScrollToCurrentLevel()
    {
        await UniTask.WaitForSeconds(0.1f);
        _nodeScroll.verticalNormalizedPosition = 1f;
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