using System.Collections;
using System.Collections.Generic;
using Microsoft.MixedReality.Toolkit;
using UnityEngine;

public class ObjectForGrab : MonoBehaviour
{
    private GameObject _handPosition;

    public bool isLeft;

    private string _leftPosName = "Left_ConicalGrabPointer(Clone)";
    private string _rightPosName = "Right_ConicalGrabPointer(Clone)";

    private Vector3 _leftGrabVector3 = new Vector3(0.01f, 0.01f, 0f);
    private Vector3 _rightGrabVector3 = new Vector3(-0.01f, 0.01f, 0f);

    private TaskManager.ObjectData _grabbedObjectData;

    private Vector3 _handRotationOnGrabbed;
    
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
            transform.SetParent(null);
        }
    }
    
    
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
            
            case TaskManager.TaskState.Play:
            {
                var minVal = 0.05f; // 거리 임계값
                var minIndex = -1;
                
                for (var i = 0; i < SocketManager.Instance.augmentedObjectList.Count; i++)
                {
                    var dist = Vector3.Distance(SocketManager.Instance.augmentedObjectList[i].transform.position,
                        _handPosition.transform.position);
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
                Debug.LogError($"놓은 물체 : {_grabbedObjectData.Name}");
                _grabbedObjectData = null;
            }
                break;
        }
    }
}
