using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmentedObject : MonoBehaviour
{
    public TaskManager.ObjectData objectData;

    public Transform objectParentTransform;

    public void SetObject(string objectName = "")
    {
        if (ReferenceEquals(objectName, null))
        {
            objectName = "";
        }
        
        var objList = gameObject.GetComponentsInChildren(typeof(Transform));

        foreach (var child in objList)
        {
            if (child.name == "ObjectParent" || child.name == "pass" || child.name == "AugmentedObject(Clone)" ||
                child.name == objectName)
            {
                continue;
            }

            child.gameObject.SetActive(false);
        }

        var objectGo = objectParentTransform.Find(objectName);

        if (ReferenceEquals(objectGo, null))
        {
            return;
        }

        objectGo.gameObject.SetActive(true);
    }


    public void SetObjectData(string text)
    {
        //gameObject.name = text;

        if (TaskManager.Instance.isTeacher)
        {
            objectData = TaskManager.Instance.teacherObjectDataList.Find(i => i.Name == text);
        }
        else
        {
            objectData = TaskManager.Instance.studentObjectDataList.Find(i => i.Name == text);
        }

        if (ReferenceEquals(objectData, null))
        {
            if (!TaskManager.Instance.isTeacher)
            {
                var idx = TaskManager.Instance.teacherObjectDataList.FindIndex(i => i.Name == text);
                objectData = TaskManager.Instance.studentObjectDataList[idx];
            }
        }


        // 일단은 꺼준다.
        ShowObject(true);
        //SetObject();
    }

    public void ShowObject(bool show)
    {
        if (show)
        {
            SetObject(objectData?.Name);
        }
        else
        {
            SetObject();
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (ReferenceEquals(objectData, null))
        {
            return;
        }

        if (ReferenceEquals(other.GetComponent<AugmentedObject>(), null))
        {
            return;
        }

        if (ReferenceEquals(other.GetComponent<AugmentedObject>().objectData, null))
        {
            return;
        }

        //Debug.LogError($"나는 {objectData.Category}인 {gameObject.name}인데 {other.gameObject.name} 랑 만남");

        if (TaskManager.Instance.isTeacher)
        {
            if (objectData.Category == TaskManager.ObjectCategory.Knife)
            {
                if (other.GetComponent<AugmentedObject>().objectData.Category == TaskManager.ObjectCategory.Food)
                {
                    if (TaskManager.Instance.leftObjectGrabber.GetHandObjectData()?.Category ==
                        TaskManager.ObjectCategory.Knife ||
                        TaskManager.Instance.rightObjectGrabber.GetHandObjectData()?.Category ==
                        TaskManager.ObjectCategory.Knife)
                    {
                        Debug.LogError($"CUT");

                        TaskManager.Instance.AddAnimationData(new TaskManager.AnimationData()
                        {
                            Category = TaskManager.AnimationCategory.Cut,
                            MainName = objectData.Name,
                            SubName = other.GetComponent<AugmentedObject>().objectData.Name,
                        });
                    }
                }
            }
        }
    }
}