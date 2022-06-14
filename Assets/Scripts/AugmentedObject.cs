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
        Debug.LogError($"SetObject : {objectName}");
        var objList = gameObject.GetComponentsInChildren(typeof(Transform));
        
        foreach (var child in objList)
        {
            if(child.name == "ObjectParent" || child.name == "pass" || child.name == "AugmentedObject(Clone)" || child.name == objectName)
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

        objectData = TaskManager.Instance.studentObjectDataList.Find(i => i.Name == text);

        // 일단은 꺼준다.
        SetObject();
    }

    public void ShowObject(bool show)
    {
        if (show)
        {
            SetObject(objectData.Name);
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

        if (objectData.Category == TaskManager.ObjectCategory.Knife)
        {
            if (other.GetComponent<AugmentedObject>().objectData.Category == TaskManager.ObjectCategory.Food)
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