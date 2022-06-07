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
      Debug.LogError($"{gameObject.name} 이랑 {other.gameObject.name} 랑 만남"); // 만났을때 cut..
   }
}
