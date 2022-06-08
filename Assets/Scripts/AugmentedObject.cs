using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmentedObject : MonoBehaviour
{
    public Text nameText;

    public TaskManager.ObjectData objectData;


    public void SetObjectData(string text)
    {
        gameObject.name = text;
        nameText.text = text;

        objectData = TaskManager.Instance.studentObjectDataList.Find(i => i.Name == text);
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

        Debug.LogError($"나는 {objectData.Category}인 {gameObject.name}인데 {other.gameObject.name} 랑 만남"); // 만났을때 cut..

        if (objectData.Category == TaskManager.ObjectCategory.Knife)
        {
            if (other.GetComponent<AugmentedObject>().objectData.Category == TaskManager.ObjectCategory.Food)
            {
                Debug.LogError($"컷컷컷 이벤트");
            }
        }
    }
}