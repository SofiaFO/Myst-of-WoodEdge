using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsUtility
{
    private const string KeyRegistry = "ITEM_";

    // Salva a key no registro
    public static void RegisterItemKey(string key)
    {
        var keys = GetItemKeys();

        if (!keys.Contains(key))
        {
            keys.Add(key);
            PlayerPrefs.SetString(KeyRegistry, string.Join(",", keys));
            PlayerPrefs.Save();
        }
    }

    // Retorna todas as keys de item já registradas
    public static List<string> GetItemKeys()
    {
        if (!PlayerPrefs.HasKey(KeyRegistry))
            return new List<string>();

        string raw = PlayerPrefs.GetString(KeyRegistry);

        if (string.IsNullOrEmpty(raw))
            return new List<string>();

        return new List<string>(raw.Split(','));
    }

    // Remove todas as keys de item
    public static void ClearAllItemKeys()
    {
        var keys = GetItemKeys();

        foreach (string key in keys)
        {
            PlayerPrefs.DeleteKey(key);
        }

        // limpa o registro também
        PlayerPrefs.DeleteKey(KeyRegistry);

        PlayerPrefs.Save();
    }
}
