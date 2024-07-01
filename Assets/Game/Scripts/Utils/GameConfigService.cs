using System;

namespace Utils
{
    public class GameConfigService : IDisposable
    {
        private GameDataSo _gameData;
        public GameDataSo GameData => _gameData;

        public GameConfigService(GameDataSo gameData)
        {
            _gameData = gameData;
        }

        public void Dispose()
        {
        }
    }
}
