using UnityEngine;

public class GameDataController : SingletonMono<GameDataController>
{
    [SerializeField] private GameData _gameData;

    private const string DATA_KEY = "GAME_DATA";
    public GameData GameData => _gameData;

    public void Start()
    {
        if (PlayerPrefs.HasKey(DATA_KEY))
        {
            UpdateData();
        }
        else
        {
            CreateNewData();
        }
    }

    public void UpdateData()
    {
        var dataString = PlayerPrefs.GetString(DATA_KEY);
        var oldData = JsonUtility.FromJson<GameData>(dataString);
        _gameData = oldData;
    }

    public void CreateNewData()
    {
        _gameData = new();
    }

    public void Save()
    {
        var dataString = JsonUtility.ToJson(_gameData);
        PlayerPrefs.SetString(DATA_KEY, dataString);
    }

    public void OnWinLevel(int levelIndex)
    {
        if (_gameData.CurrentLevel <= levelIndex)
        {
            var maxLevel = ConfigManager.Instance.GetConfig<LevelConfigCollectionSO>().Configs.Count;
            _gameData.CurrentLevel = Mathf.Min(levelIndex + 1, maxLevel + 1);
        }
        Save();
    }
}