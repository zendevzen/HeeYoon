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

    public static bool LeftGrab;
    public static bool RightGrab;
    
    public void Grab()
    {
        Debug.LogError("잡음");

        if (isLeft)
        {
            TaskManager.Instance.startPage.OnLeftFistGrip();
        }
        else
        {
            TaskManager.Instance.startPage.OnRightFistGrip();
        }
    }
    
    public void Release()
    {
        Debug.LogError("놓음");
        
        if (isLeft)
        {
            TaskManager.Instance.startPage.OnLeftFistRelease();
        }
        else
        {
            TaskManager.Instance.startPage.OnRightFistRelease();
        }
    }
}
