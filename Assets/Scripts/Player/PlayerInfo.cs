using UnityEngine;

namespace Player
{
    public static class PlayerInfo
    {
        private const string PlayerNameKey = "PLAYER_NAME";
        public static string PlayerName
        {
            get => PlayerPrefs.GetString(PlayerNameKey, "Player");
            set => PlayerPrefs.SetString(PlayerNameKey, value);
        }
    }
}