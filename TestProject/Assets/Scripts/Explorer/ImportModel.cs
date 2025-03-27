using UnityEngine;
using System.Collections;
using System.IO;
using Dummiesman;

public class ImportModel : MonoBehaviour
{
    [SerializeField] string _filePath = @"C:\Users\soo50\Documents\ModelTest\\Cat.obj";

    void Start()
    {
        StartCoroutine(LoadObjModel());
    }

    IEnumerator LoadObjModel()
    {
        if (!File.Exists(_filePath))
        {
            Debug.LogError("파일을 찾을 수 없습니다: " + _filePath);
            yield break;
        }

        // 파일 스트림 열기
        using (FileStream stream = new FileStream(_filePath, FileMode.Open))
        {
            GameObject obj = new OBJLoader().Load(stream);  // Dummiesman의 OBJLoader
            obj.transform.position = Vector3.zero;
        }

        yield return null;
    }
}
