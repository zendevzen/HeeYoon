using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public ObjectForGrab leftObjectGrabber;
    public ObjectForGrab rightObjectGrabber;


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
            pageHandlerTransform.position = new Vector3(0f, -0.1f, 0.4f);
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
                
                pageHandlerTransform.gameObject.SetActive(false);
                
                foreach (var obj in SocketManager.Instance.augmentedObjectList)
                {
                    obj.ShowObject(false);
                }
            }
                break;
        }
    }

    private void Awake()
    {
        _instance = this;

        //CurrentTaskState = TaskState.Play;

        CurrentTaskState = TaskState.Ready;
    }
    
    // ???????????? ???????????? ???????????? ??????
    
    // Ready ???????????? ?????? ?????? ???????????? ??????.
    // ???????????? ????????? ?????? ??????????????? ??????.
    // ???????????? ?????? ??? ????????? ???????????? ??????.
    
    // ?????? ??????
    
    // ??? ????????? ????????????
    
    // ????????? ????????? ?????????


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
    
    public List<AnimationData> animationDataList = new List<AnimationData>();

    public class AnimationData
    {
        public AnimationCategory Category;

        public string MainName;
        public string SubName;
    }

    public enum AnimationCategory
    {
        Put,
        Mix,
        Cut,
        Pour
    }

    /*private void Start()
    {
        AddAnimationData(new AnimationData()
        {
            Category = AnimationCategory.Put,
            MainName = "banana",
            SubName = "bottle",
        });
        
        AddAnimationData(new AnimationData()
        {
            Category = AnimationCategory.Mix,
            MainName = "spoon",
            SubName = "bottle",
        });
    }*/

    public void AddAnimationData(AnimationData data)
    {
        if (animationDataList.Count < 1)
        {
            animationDataList.Add(data);
        }
        else
        {
            // ????????? ???????????? ????????????.
            if (!animationDataList.Any(i =>
                    i.Category == data.Category && i.MainName == data.MainName && i.SubName == data.SubName))
            {
                animationDataList.Add(data);
            }
        }
    }
}
