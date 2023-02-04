using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RandomString : MonoBehaviour
{
    public string[] names = new string[3] { "程序猿", "王豌豆", "东方高数" };
    public Text text;

    void Awake()
    {
        List<string> nameList = new List<string>();
        foreach (var str in names)
        {
            nameList.Add(str);
        }
        text.text = "";
        for (int i = 0; i < 3; i++)
        {
            var r = Random.Range(0, nameList.Count);
            text.text += nameList[r] + "\n";
            nameList.RemoveAt(r);
        }
    }
}
