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
            Debug.LogError("������ ã�� �� �����ϴ�: " + _filePath);
            yield break;
        }

        // ���� ��Ʈ�� ����
        using (FileStream stream = new FileStream(_filePath, FileMode.Open))
        {
            GameObject obj = new OBJLoader().Load(stream);  // Dummiesman�� OBJLoader
            obj.transform.position = Vector3.zero;
        }

        yield return null;
    }
}
