using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Project.Programmer.NSJ.RND.Script.Test
{
    public class JsonTest : MonoBehaviour
    {
        [SerializeField] TestData _data;

        [SerializeField] TestData _loadData;

        [ContextMenu("Save")]
        public void Save()
        {
            string path = $"{Application.dataPath}/Project/Programmer/NSJ/RND/Script/Test/Save";
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            string json = JsonUtility.ToJson( _data );

            File.WriteAllText($"{path}/save.text", json);
        }

        [ContextMenu("Load")]
        public void Load()
        {
            string path = $"{Application.dataPath}/Project/Programmer/NSJ/RND/Script/Test/Save/save.text";

            if (!File.Exists(path)) 
            {
                Debug.LogError("파일 없음");
                return;
            }
            
            string json = File.ReadAllText(path);

            _loadData = JsonUtility.FromJson<TestData>(json);
        }
    }

    [System.Serializable]
    public class TestData
    {
        public string Name;
        public GameObject TestObject;
        public ThrowObject ThrowObject;
    }
}