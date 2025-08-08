using UnityEngine;
using YamlDotNet.Serialization;


public class YamlReaderExample : MonoBehaviour
{
    [System.Serializable]
    private class PlayerData
    {
        public string name;
        public int score;
    }

    private void Start()
    {
        string yaml = @"
name: Alice
score: 100
";
        var deserializer = new DeserializerBuilder().Build();
        PlayerData data = deserializer.Deserialize<PlayerData>(yaml);

        Debug.Log("Name: " + data.name);
        Debug.Log("Score: " + data.score);
    }
}
