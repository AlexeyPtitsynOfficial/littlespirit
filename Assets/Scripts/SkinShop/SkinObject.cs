using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

[System.Serializable]
public class SkinObject : ScriptableObject {

    public int skinNumber;
    public string skinName = "";
    public int cost;
    public bool isBought;
}
