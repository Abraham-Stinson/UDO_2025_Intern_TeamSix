using UnityEngine;

public static class TransformExtensions
{
   public static void Clear(this Transform transform)
   {
      while (transform.childCount > 0)
      {
         Transform t = transform.GetChild(0);
         t.SetParent(null);
         Object.Destroy(t.gameObject);
      }
   }

   public static Transform GetLast(this Transform transform)
   {
      if (transform.childCount <= 0)
         return null;
         
      return transform.GetChild(transform.childCount - 1);
   }
}
