using System.Collections.Generic;
using UnityEngine;

public class CloudVariables : MonoBehaviour
{
    public static long Highscore { get; set; }
    public static int StarCoins { get; set; }
    public static List<int> Skins { get; set; }
    public static int CurrentSkin { get; set; }

    private void Awake()
    {
        Skins = new List<int>();
    }
}