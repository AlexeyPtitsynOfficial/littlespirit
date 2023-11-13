using UnityEngine;

public class BGGradient : MonoBehaviour {

    public string startColor;
    public string endColor;
    // Use this for initialization
    void Start () {
        var mesh = GetComponent<MeshFilter>().mesh;
        var colors = new Color[mesh.vertices.Length];
        colors[0] = hexToColor(startColor);
        colors[1] = hexToColor(endColor);
        colors[2] = hexToColor(startColor);
        colors[3] = hexToColor(endColor);
        mesh.colors = colors;
    }

    public static Color hexToColor(string hex)
    {
        hex = hex.Replace("0x", "");//in case the string is formatted 0xFFFFFF
        hex = hex.Replace("#", "");//in case the string is formatted #FFFFFF
        byte a = 255;//assume fully visible unless specified in hex
        byte r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
        byte g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
        byte b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
        //Only use alpha if the string has enough characters
        if (hex.Length == 8)
        {
            a = byte.Parse(hex.Substring(6, 2), System.Globalization.NumberStyles.HexNumber);
        }
        return new Color32(r, g, b, a);
    }
}
