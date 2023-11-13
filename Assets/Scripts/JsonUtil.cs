using System.Collections;
using Boomlagoon.JSON;
using System;

public class JsonUtil {

    public static string CollectionToJsonString<T>(T arr, string jsonKey) where T : IList
    {
        JSONObject jObject = new JSONObject();
        JSONArray jArray = new JSONArray();

        for (int i = 0; i < arr.Count; i++)
            jArray.Add(new JSONValue(arr[i].ToString()));
        jObject.Add(jsonKey, jArray);

        return jObject.ToString();
    }

    public static T[] JsonStringToArray<T>(string jsonString, string jsonKey, Func<string, T> parser)
    {
        JSONObject jObject = JSONObject.Parse(jsonString);
        JSONArray jArray = jObject.GetArray(jsonKey);

        T[] convertedArray = new T[jArray.Length];
        for (int i = 0; i < jArray.Length; i++)
            convertedArray[i] = parser(jArray[i].Str.ToString());

        return convertedArray;
    }

    public static JSONArray CollectionToJsonArray<T>(T arr) where T : IList
    {
        JSONArray jArray = new JSONArray();

        for (int i = 0; i < arr.Count; i++)
            jArray.Add(new JSONValue(arr[i].ToString()));

        return jArray;
    }

    public static string StrToJsonString(string data, string jsonKey)
    {
        JSONObject jObject = new JSONObject();

        jObject.Add(jsonKey, data);

        return jObject.ToString();
    }

    public static string JsonStringToStr(string jsonString, string jsonKey)
    {
        
        JSONObject jObject = JSONObject.Parse(jsonString);

        return jObject.GetNumber(jsonKey).ToString();
    }
}