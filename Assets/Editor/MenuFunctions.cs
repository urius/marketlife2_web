using Data;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class MenuFunctions
    {
        [MenuItem("Tools/Reset Player Data")]
        public static void ResetPlayerData()
        {
            PlayerPrefs.SetString(Constants.PlayerDataKey, null);
        }
    }
}