using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameplayController : SingletonMono<GameplayController>
{
    [SerializeField] private LevelController _levelControllerPrefab;

    private LevelController _currentLevelController;
    private int _currentLevelIndex;
    private EState _currentState;
    
    private async UniTaskVoid Start()
    {
        await UniTask.Yield();
        ChangeState(EState.LevelSelect);
    }

    private void ChangeState(EState state)
    {
        ExitState(_currentState);
        EnterState(state);
    }

    private void ExitState(EState state)
    {
        switch (state)
        {
            case EState.None:
                break;
            case EState.LevelSelect:
                UIManager.Instance.GetView<LevelSelectUIView>().Hide();
                break;
            case EState.InGame:
                DestroyLevelController();
                UIManager.Instance.GetView<InGameUIView>().Hide();
                break;
        }
    }

    private void EnterState(EState state)
    {
        _currentState = state;
        switch (state)
        {
            case EState.None:
                break;
            case EState.LevelSelect:
                UIManager.Instance.GetView<LevelSelectUIView>().Show();
                break;
            case EState.InGame:
                UIManager.Instance.GetView<InGameUIView>().Show();
                SpawnLevelController(_currentLevelIndex);
                break;
        }
    }

    public void PlayLevel(int level)
    {
        _currentLevelIndex = level;
        ChangeState(EState.InGame);
    }

    public void PlayNextLevel()
    {
        _currentLevelIndex++;
        ChangeState(EState.InGame);
    }

    public void ExitLevel()
    {
        if (_currentState != EState.InGame) return;
        ChangeState(EState.LevelSelect);
    }

    private void SpawnLevelController(int levelIndex)
    {
        _currentLevelController = Instantiate(_levelControllerPrefab, transform);
        var inGameUIView = UIManager.Instance.GetView<InGameUIView>();
        _currentLevelController.StartLevel(levelIndex, inGameUIView);
    }

    private void DestroyLevelController()
    {
        if (_currentLevelController != null)
            Destroy(_currentLevelController.gameObject);
        _currentLevelController = null;
    }

    public enum EState
    {
        None,
        LevelSelect,
        InGame
    }
}