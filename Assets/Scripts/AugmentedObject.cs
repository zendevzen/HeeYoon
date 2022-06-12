using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmentedObject : MonoBehaviour
{
    public TaskManager.ObjectData objectData;

    public Transform objectParentTransform;

    public void SetObject(string objectName)
    {
        var objList = gameObject.GetComponentsInChildren(typeof(Transform));
        
        foreach (var child in objList)
        {
            if(child.name == "ObjectParent" || child.name == "pass" || child.name == objectName)
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
        gameObject.name = text;

        objectData = TaskManager.Instance.studentObjectDataList.Find(i => i.Name == text);

        SetObject(text);
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

        Debug.LogError($"나는 {objectData.Category}인 {gameObject.name}인데 {other.gameObject.name} 랑 만남");

        if (objectData.Category == TaskManager.ObjectCategory.Knife)
        {
            if (other.GetComponent<AugmentedObject>().objectData.Category == TaskManager.ObjectCategory.Food)
            {
                Debug.LogError($"컷컷컷 이벤트");
                
                // TODO : 컷
            }
        }
    }
}