using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightManager : MonoBehaviour {

    public GameObject highlightPrefab;

    private GameObject highlight;

    private Vector3 positionOffset = new Vector3(0f, 0f, -0.001f);

    private Quaternion zeroRotation = Quaternion.Euler(Vector3.zero);

    private Vector3 scale = new Vector3(0.073f, 0.1f, 0.001f);

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void select(Transform target)
    {
        if(highlight == null)
        {
            highlight = GameObject.Instantiate(highlightPrefab);
            highlight.transform.SetParent(transform);
        }

        highlight.SetActive(false);

        if (target != null)
        {
            highlight.transform.SetParent(target);
            highlight.transform.localPosition = positionOffset;
            highlight.transform.localRotation = zeroRotation;
            highlight.transform.localScale = scale;
            highlight.SetActive(true);
        }
        else
        {
            highlight.transform.SetParent(transform);
        }
    }
}
