using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class Utils
{
    static public void DeleteAllChildren(Transform obj, bool editMode = false)
    {
        int childs = obj.childCount;
        for (int i = childs - 1; i > 0; i--)
        {
            if(editMode)
                GameObject.DestroyImmediate(obj.GetChild(i).gameObject);
            else
                GameObject.Destroy(obj.GetChild(i).gameObject);
        }
    }
    public static T RecursiveFindChild<T>(Transform parent, string childName)
    {
        var res = RecursiveFindChild(parent, childName);
        if(res != null)
        {
            return res.GetComponent<T>();
        }
        return default(T);
    }

    public static Transform RecursiveFindChild(Transform parent, string childName)
    {
        foreach (Transform child in parent)
        {
            if (child.name == childName)
            {
                Debug.Log("found " + childName);
                return child.transform;
            }
        }

        foreach (Transform child in parent)
        {
            var res = RecursiveFindChild(child, childName);
            if (res != null)
                return res;
        }

        return null;
    }
}
