using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using CoroutineRunner = Utils.CoroutineRunner;
using EventDispatcher = Utils.EventDispatcher;


namespace Game
{
    public class PersistentSceneLogic : MonoBehaviour
    {

        [SerializeField]
        private GameDataSo _gameDataSO;

        private void Start()
        {
            InitScene();
        }

        private void InitScene()
        {
            Application.targetFrameRate = 60;
            Screen.sleepTimeout = SleepTimeout.NeverSleep;
            RegisterServices();
            LoadMainScene();
        }

       
        private void RegisterServices()
        {
            ServiceLocator.UnregisterAll();

            CoroutineRunner _coroutineRunner = new CoroutineRunner();
            EventDispatcher _eventDispatcher = new EventDispatcher();
            GameConfigService _gameconfig = new GameConfigService(_gameDataSO);

            ServiceLocator.RegisterService(_coroutineRunner);
            ServiceLocator.RegisterService(_eventDispatcher);
            ServiceLocator.RegisterService(_gameconfig);
        }

        private void LoadMainScene()
        {
            SceneManager.LoadScene("Main");
        }
  
    }
}
