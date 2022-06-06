using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AugmentedObject : MonoBehaviour
{
   public Text nameText;

   public void SetText(string text)
   {
      nameText.text = text;
   }
}
