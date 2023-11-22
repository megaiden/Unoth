using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Route : MonoBehaviour
{
   private Transform[] childObjects;
   public List<Transform> childNodeList = new();

   private void Start()
   {
#if !UNITY_EDITOR
      FillNodes();
#endif
   }
   #if UNITY_EDITOR
   private void OnDrawGizmos()
   {
      Gizmos.color = Color.green;
      //FillNodes();

      for (int i = 0; i < childNodeList.Count; i++)
      {
         Vector3 currentPos = childNodeList[i].position;
         if (i>0)
         {
            Vector3 prevPos = childNodeList[i - 1].position;
            Gizmos.DrawLine(prevPos,currentPos);
         }
      
        // childNodeList[i].GetComponent<PanelBehavior>().FillDirections();
      }
   }
   #endif
   void FillNodes()
   {
      childNodeList.Clear();
      childObjects = GetComponentsInChildren<Transform>();
      foreach (Transform child in childObjects)
      {
         if (child != this.transform)
         {
            childNodeList.Add(child);
         }
      }
   }
   
}
