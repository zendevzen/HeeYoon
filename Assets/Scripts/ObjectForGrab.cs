using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;
using UnityEngine.UI;

public class ObjectForGrab : MonoBehaviour
{
    private GameObject _handPosition;

    public bool isLeft;

    public Transform upPourChecker;
    public Transform downPourChecker;

    private string _leftPosName = "Left_ConicalGrabPointer(Clone)";
    private string _rightPosName = "Right_ConicalGrabPointer(Clone)";

    private Vector3 _leftGrabVector3 = new Vector3(0.01f, 0.01f, 0f);
    private Vector3 _rightGrabVector3 = new Vector3(-0.01f, 0.01f, 0f);

    private TaskManager.ObjectData _grabbedObjectData;

    private Vector3 _handRotationOnGrabbed;

    public GameObject stateGo;
    public Text stateText;
    
    void Update()
    {
        if (isLeft)
        {
            _handPosition = GameObject.Find(_leftPosName);
        }
        else
        {
            _handPosition = GameObject.Find(_rightPosName);
        }

        if (!ReferenceEquals(_handPosition, null))
        {
            transform.SetParent(_handPosition.transform);
            transform.localPosition = (isLeft) ? _leftGrabVector3 : _rightGrabVector3;
            transform.localRotation = Quaternion.Euler(new Vector3(30f,60f,55f));
            transform.SetParent(null);
            
            stateGo.transform.SetParent(transform);
            stateGo.transform.localPosition = Vector3.zero;
            stateGo.transform.SetParent(null);
            
            stateGo.transform.LookAt(TaskManager.Instance.headPosTransform);
            stateGo.transform.Translate(0f,0.01f,-0.05f);
            stateGo.transform.Rotate(20f,0f,0f);

            if (ReferenceEquals(_grabbedObjectData, null))
            {
                stateText.text = "empty";
            }
            else
            {
                stateText.text = $"grab - {_grabbedObjectData.Name}";
                
                if (_grabbedObjectData.Category == TaskManager.ObjectCategory.Bowl)
                {
                    if (Mathf.Abs(upPourChecker.position.y - downPourChecker.position.y) < 0.02f)
                    {
                        // TODO : 왼손 오른손 양쪽에 보울 들고 기울일때 붓는걸로 하자.
                        Debug.LogError($"{Mathf.Abs(upPourChecker.position.y - downPourChecker.position.y)}   기울임!!!!");

                        if (!IsAnotherHandNullOrNotBowl())
                        {
                            stateText.text =
                                $"pour - {_grabbedObjectData.Name} -> {GetAnotherHandObjectData().Name}";
                        }
                    }
                }
                else if (_grabbedObjectData.Category == TaskManager.ObjectCategory.Spoon)
                {
                    if (IsAnotherHandNullOrNotBowl())
                    {
                        _mixTimer = 0f;
                        _maxDistance = 0f;
                            
                        _mixStartPos = Vector3.zero;
                            
                        return;
                    }

                    // TODO : 섞기 하기
                    if (_mixStartPos == Vector3.zero)
                    {
                        _mixTimer = 0f;
                        _maxDistance = 0f;

                        _mixStartPos = transform.position;
                    }
                    else //감별중
                    {
                        _mixTimer += Time.deltaTime;

                        var dist = Vector3.Distance(_mixStartPos, transform.position);
                        
                        if (dist > _maxDistance)
                        {
                            _maxDistance = dist;
                        }

                        if (_maxDistance >= _mixDistance && dist < _mixMinDistance)
                        {
                            Debug.LogError("섞는다 섞는다!!!!!!");
                            
                            stateText.text = $"mix - {_grabbedObjectData.Name} -> {GetAnotherHandObjectData().Name}";
                        }

                        if (_mixTimer > _waitTimer)
                        {
                            _mixTimer = 0f;
                            _maxDistance = 0f;
                            
                            _mixStartPos = Vector3.zero;
                        }
                    }
                }
            }
        }
    }

    public bool IsAnotherHandNullOrNotBowl()
    {
        if (isLeft)
        {
            if (ReferenceEquals(TaskManager.Instance.rightObjectGrabber._grabbedObjectData, null))
            {
                return true;
            }
                            
                            
            if (TaskManager.Instance.rightObjectGrabber._grabbedObjectData.Category !=
                TaskManager.ObjectCategory.Bowl)
            {
                return true;
            }
        }
        else
        {
            if (ReferenceEquals(TaskManager.Instance.leftObjectGrabber._grabbedObjectData, null))
            {
                return true;
            }
                            
                            
            if (TaskManager.Instance.leftObjectGrabber._grabbedObjectData.Category !=
                TaskManager.ObjectCategory.Bowl)
            {
                return true;
            }
        }

        return false;
    }

    public TaskManager.ObjectData GetAnotherHandObjectData()
    {
        return isLeft
            ? TaskManager.Instance.rightObjectGrabber._grabbedObjectData
            : TaskManager.Instance.leftObjectGrabber._grabbedObjectData;
    }

    private Vector3 _mixStartPos = Vector3.zero;

    private float _mixTimer = 0f;
    
    private float _waitTimer = 1f;

    private float _mixDistance = 0.2f;
    private float _mixMinDistance = 0.1f;
    
    private float _maxDistance;
    
    
    public void Grab()
    {
        Debug.LogError("잡음");

        switch (TaskManager.Instance.CurrentTaskState)
        {
            case TaskManager.TaskState.Ready:
            {
                if (isLeft)
                {
                    TaskManager.Instance.startPage.OnLeftFistGrip();
                }
                else
                {
                    TaskManager.Instance.startPage.OnRightFistGrip();
                }
            }
                break;
            
            case TaskManager.TaskState.Match:
            {
                
            }
                break;
            
            case TaskManager.TaskState.Play: // TODO : 이부분 해결하기 뭐 잡았는지 알아야하고 그다음 만나는거에 따라서 컷같은 이벤트 알게하기.
            {
                var minVal = 0.5f; // 거리 임계값
                var minIndex = -1;
                
                for (var i = 0; i < SocketManager.Instance.augmentedObjectList.Count; i++)
                {
                    var dist = Vector3.Distance(SocketManager.Instance.augmentedObjectList[i].transform.position,
                        _handPosition.transform.position);
                    
                    Debug.LogError($"dist {SocketManager.Instance.augmentedObjectList[i].nameText.text} {dist}");
                    
                    if (dist < minVal)
                    {
                        minVal = dist;
                        minIndex = i;
                    }
                }

                if (minIndex > -1)
                {
                    _grabbedObjectData = SocketManager.Instance.augmentedObjectList[minIndex].GetComponent<AugmentedObject>()
                        .objectData;

                    if (ReferenceEquals(_grabbedObjectData, null))
                    {
                        break;
                    }
                    
                    Debug.LogError($"잡은 물체 : {_grabbedObjectData.Name}");

                    _handRotationOnGrabbed = _handPosition.transform.rotation.eulerAngles;
                    
                    Debug.LogError($"잡을때 손 각도 : {_handRotationOnGrabbed}");
                }
                
            }
                break;
        }
    }
    
    public void Release()
    {
        Debug.LogError("놓음");
        
        switch (TaskManager.Instance.CurrentTaskState)
        {
            case TaskManager.TaskState.Ready:
            {
                if (isLeft)
                {
                    TaskManager.Instance.startPage.OnLeftFistRelease();
                }
                else
                {
                    TaskManager.Instance.startPage.OnRightFistRelease();
                }
            }
                break;
            
            case TaskManager.TaskState.Match:
            {
                
            }
                break;
            
            case TaskManager.TaskState.Play:
            {
                if (ReferenceEquals(_grabbedObjectData, null))
                {
                    break;
                }
                
                Debug.LogError($"놓은 물체 : {_grabbedObjectData?.Name}");
                
                
                
                
                var minVal = 0.5f; // 거리 임계값
                var minIndex = -1;
                
                for (var i = 0; i < SocketManager.Instance.augmentedObjectList.Count; i++)
                {
                    var dist = Vector3.Distance(SocketManager.Instance.augmentedObjectList[i].transform.position,
                        _handPosition.transform.position);
                    
                    //Debug.LogError($"dist {SocketManager.Instance.augmentedObjectList[i].nameText.text} {dist}");
                    
                    if (dist < minVal)
                    {
                        minVal = dist;
                        minIndex = i;
                    }
                }

                if (minIndex > -1)
                {
                    var nearObjectData = SocketManager.Instance.augmentedObjectList[minIndex].GetComponent<AugmentedObject>()
                        .objectData;

                    if (ReferenceEquals(nearObjectData, null))
                    {
                        break;
                    }
                    
                    Debug.LogError($"놓은곳에서 가장 가까운 물체 : {_grabbedObjectData.Name}");
                    
                    // TODO : 이게 풋이나 무브가 되어야함.
                    
                    
                }
                
                
                
                _grabbedObjectData = null;
            }
                break;
        }
    }
}
