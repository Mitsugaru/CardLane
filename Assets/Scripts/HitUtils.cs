using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitUtils {

    public static bool detectHitLane(Ray ray, out GameObject lane)
    {
        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, LayerMask.GetMask("Lane"));

        if (hits.Length > 0)
        {
            lane = hits[0].transform.parent.gameObject;
        }
        else
        {
            lane = null;
        }

        return hits.Length > 0;
    }

    public static GameObject detectHandCardHit(Ray ray, Transform source)
    {
        GameObject target = null;

        RaycastHit[] hits = Physics.RaycastAll(ray, float.MaxValue, LayerMask.GetMask("Card"));
        for (int i = 0; i < hits.Length; i++)
        {
            //Check that the card touched has parent transform of hand
            if (hits[i].transform.parent.Equals(source))
            {
                target = hits[i].transform.gameObject;
                break;
            }
            else if (hits[i].transform.parent.parent != null && hits[i].transform.parent.parent.Equals(source))
            {
                //Case for Jokers
                target = hits[i].transform.parent.gameObject;
            }
        }

        return target;
    }
}
