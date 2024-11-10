using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeasureDistanceMode : MonoBehaviour
{
    static MeasureDistanceMode instance;
    public static MeasureDistanceMode Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<MeasureDistanceMode>();

                if (instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MeasureDistanceMode).Name);
                    instance = singletonObject.AddComponent<MeasureDistanceMode>();
                }
            }
            return instance;
        }
    }

    [Header("Measure Distance")]
    [SerializeField] GameObject prefab_MeasureDistance;
    [SerializeField] Transform transform_parent;

    [Header("Measure Distance Lists")]
    [SerializeField] List<MeasureDistance> list_measure = new List<MeasureDistance>();

    public bool isCreating { get; set; }

    GameObject measure;

    void Start()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); 
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            AllDeleteMeasureList();
        }
        else if (Input.GetKeyDown(KeyCode.R))
        {
            CreateMeasureDistance(true);
        }
    }

    // ############################################################################## :: Measure List
    public void AddMeasureList(MeasureDistance measure)
    {
        list_measure.Add(measure);
    }

    public void RemoveMeasureList(MeasureDistance measure)
    {
        list_measure.Remove(measure);
    }

    public void AllDeleteMeasureList()
    {
        for (int i = list_measure.Count - 1; i >= 0; i--)
        {
            list_measure[i].DestroyMeasure();
        }
    }

    // ############################################################################## :: Measure Distance 
    public void CreateMeasureDistance(bool isOn)
    {
        if (isOn)
        {
            Debug.Log("Create Measure!");

            measure = GameObject.Instantiate(prefab_MeasureDistance, transform_parent);
            isCreating = true;
        }
        else
        {
            // 이미 생성중인 상태라면, 현재 생성중인 상태를 중단&삭제 및 Mode 종료
            if (isCreating)
            {
                measure.GetComponent<MeasureDistance>().DestroyMeasure();
                EndMeasureDistanceMode();
            }
        }
    }

    public void EndMeasureDistanceMode()
    {
        Debug.Log("End Measure!");
        isCreating = false;
    }
}