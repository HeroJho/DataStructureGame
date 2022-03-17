using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private Vector3 prePos;
    public float pressValue;
    Coroutine co = null;

    private void Start()
    {
        prePos = transform.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(DownButton());
    }

    private void OnCollisionExit(Collision collision)
    {
        if (co != null)
            StopCoroutine(co);
        co = StartCoroutine(UpButton());
        SortManager.isView = false;
    }

    IEnumerator DownButton()
    {
        while (0.2f < transform.localPosition.y)
        {
            transform.localPosition = new Vector3(prePos.x, transform.localPosition.y - pressValue, prePos.z);
            yield return new WaitForSeconds(0.1f);
        }

        SortManager.isView = true;
        transform.localPosition = new Vector3(prePos.x, 0.2f, prePos.z);
    }
    IEnumerator UpButton()
    {
        while(2f > transform.localPosition.y)
        {
            transform.localPosition = new Vector3(prePos.x, transform.localPosition.y + pressValue, prePos.z);
            yield return new WaitForSeconds(0.1f);
        }

        transform.localPosition = new Vector3(prePos.x, prePos.y, prePos.z);
    }

}
