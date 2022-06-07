using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class TaskManager : MonoBehaviour
{
    private static TaskManager _instance;

    public static TaskManager Instance => _instance;

    
    public bool isTeacher;
    
    public ObjectMatchPage objectMatchPage;
    public StartPage startPage;

    public Transform headPosTransform;
    public Transform pageHandlerTransform;

    public Vector3 workPlacePos;


    public void FixCanvasToHead(bool isFix)
    {
        if (isFix)
        {
            pageHandlerTransform.SetParent(headPosTransform);
            pageHandlerTransform.localPosition = new Vector3(0f, -0.22f, 0.8f);
            pageHandlerTransform.localRotation = Quaternion.Euler(Vector3.zero);
        }
        else
        {
            pageHandlerTransform.SetParent(null);
            pageHandlerTransform.position = new Vector3(0f, -0.1f, 0.6f);
            pageHandlerTransform.rotation = Quaternion.Euler(Vector3.zero);
        }
    }
    
    public enum TaskState
    {
        Ready,
        Match,
        Play
    }

    private TaskState _currentTaskState;

    public TaskState CurrentTaskState
    {
        get => _currentTaskState;
        set
        {
            _currentTaskState = value;
            OnCurrentTaskStateChanged();
        }
    }

    private void OnCurrentTaskStateChanged()
    {
        Debug.LogError($"CurrentTaskState {CurrentTaskState}");
        switch (_currentTaskState)
        {
            case TaskState.Ready:
            {
                startPage.gameObject.SetActive(true);
                objectMatchPage.gameObject.SetActive(false);

                startPage.Init();
            }
                break;
            case TaskState.Match:
            {
                startPage.gameObject.SetActive(false);
                objectMatchPage.gameObject.SetActive(true);
                
                objectMatchPage.Init();
            }
                break;
            case TaskState.Play:
            {
                startPage.gameObject.SetActive(false);
                objectMatchPage.gameObject.SetActive(false);
            }
                break;
        }
    }

    private void Awake()
    {
        _instance = this;

        CurrentTaskState = TaskState.Ready;
    }
    
    // 선생인지 학생인지 선택하게 하기
    
    // Ready 단계에서 선생 학생 선택하게 하기.
    // 선택후엔 파이썬 코드 실행하라고 하기.
    // 그전이나 후에 손 두개로 위치잡게 하기.
    
    // 첨에 켜서
    
    // 손 두개로 위치잡기
    
    // 두손다 쥐어서 넘기기


    public enum ObjectCategory
    {
        Food,
        Knife,
        Spoon,
        Bowl
    }
    
    public class ObjectData
    {
        public string Name;
        public ObjectCategory Category;

        public List<ObjectData> InsideObjectList = new List<ObjectData>();
    }

    public List<ObjectData> teacherObjectDataList = new List<ObjectData>()
    {
        new ObjectData()
        {
            Name = "donut",
            Category = ObjectCategory.Food
        },
        new ObjectData()
        {
            Name = "bottle",
            Category = ObjectCategory.Bowl
        },
        new ObjectData()
        {
            Name = "plate",
            Category = ObjectCategory.Bowl
        },
        new ObjectData()
        {
            Name = "knife",
            Category = ObjectCategory.Knife
        },
        new ObjectData()
        {
            Name = "banana",
            Category = ObjectCategory.Food
        },
        new ObjectData()
        {
            Name = "bowl",
            Category = ObjectCategory.Bowl
        },
        new ObjectData()
        {
            Name = "spoon",
            Category = ObjectCategory.Spoon
        },
    };
    public List<ObjectData> studentObjectDataList = new List<ObjectData>()
    {
        new ObjectData()
        {
            Name = "donut",
            Category = ObjectCategory.Food
        },
        new ObjectData()
        {
            Name = "apple",
            Category = ObjectCategory.Food
        },
        new ObjectData()
        {
            Name = "plate",
            Category = ObjectCategory.Bowl
        },
        new ObjectData()
        {
            Name = "knife",
            Category = ObjectCategory.Knife
        },
        new ObjectData()
        {
            Name = "cup",
            Category = ObjectCategory.Bowl
        },
        new ObjectData()
        {
            Name = "bowl",
            Category = ObjectCategory.Bowl
        },
        new ObjectData()
        {
            Name = "spoon",
            Category = ObjectCategory.Spoon
        },
    };
}
