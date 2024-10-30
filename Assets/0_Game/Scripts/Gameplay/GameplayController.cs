using Cysharp.Threading.Tasks;
using JSAM;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameplayController : SingletonMono<GameplayController>
{
    [SerializeField] private LevelController _levelController;

    private int _currentLevelIndex;
    private EState _currentState;
    
    private async UniTaskVoid Start()
    {
        await UniTask.Yield();
        ChangeState(EState.LevelSelect).Forget();
    }

    private async UniTask ChangeState(EState state)
    {
        await ExitState(_currentState);
        await EnterState(state);
    }

    private async UniTask ExitState(EState state)
    {
        await UIManager.Instance.GetView<TransitionUIView>().Show();
        switch (state)
        {
            case EState.None:
                break;
            case EState.LevelSelect:
                AudioManager.FadeMusicOut(EMusic.HomeBGMSO, 0.5f);
                UIManager.Instance.GetView<LevelSelectUIView>().Hide();
                break;
            case EState.InGame:
                _levelController.gameObject.SetActive(false);
                AudioManager.FadeMusicOut(EMusic.InGameBGMSO, 0.5f);
                UIManager.Instance.GetView<InGameUIView>().Hide();
                break;
        }
        await UniTask.WaitForSeconds(0.1f);
    }

    private  async UniTask EnterState(EState state)
    {
        _currentState = state;
        switch (state)
        {
            case EState.None:
                break;
            case EState.LevelSelect:
                AudioManager.FadeMusicIn(EMusic.HomeBGMSO, 1f);
                UIManager.Instance.GetView<LevelSelectUIView>().Show();
                break;
            case EState.InGame:
                AudioManager.FadeMusicIn(EMusic.InGameBGMSO, 1f);
                UIManager.Instance.GetView<InGameUIView>().Show();
                ActiveLevelController(_currentLevelIndex);
                break;
        }
        await UniTask.Yield();
        await UIManager.Instance.GetView<TransitionUIView>().Hide();
    }

    public void PlayLevel(int level)
    {
        _currentLevelIndex = level;
        ChangeState(EState.InGame).Forget();
    }

    public void PlayNextLevel()
    {
        _currentLevelIndex++;
        ChangeState(EState.InGame).Forget();
    }

    public void ExitLevel()
    {
        if (_currentState != EState.InGame) return;
        ChangeState(EState.LevelSelect).Forget();
    }

    private void ActiveLevelController(int levelIndex)
    {
        _levelController.gameObject.SetActive(true);
        _levelController.StartLevel(levelIndex).Forget();
    }

    public enum EState
    {
        None,
        LevelSelect,
        InGame
    }
}