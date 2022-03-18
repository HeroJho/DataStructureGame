using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private Vector3 prePos;
    public float pressValue;
    Coroutine co = null;

    AudioSource audio;
    public List<AudioClip> clips = new List<AudioClip>();
    public GameObject light;
    public Animator SortUIButtonAnim;

    private void Start()
    {
        audio = GetComponent<AudioSource>();
        prePos = transform.localPosition;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        if (co != null)
            StopCoroutine(co);

        audio.pitch = 1;
        audio.PlayOneShot(clips[0]);
        co = StartCoroutine(DownButton());
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag != "Player")
            return;

        if (co != null)
            StopCoroutine(co);

        audio.pitch = 0.7f;
        audio.PlayOneShot(clips[0]);
        co = StartCoroutine(UpButton());
        SortManager.isView = false;
        light.SetActive(false);
        SortUIButtonAnim.SetBool("isShow", false);
        Cursor.visible = false;
    }

    IEnumerator DownButton()
    {
        while (0.2f < transform.localPosition.y)
        {
            transform.localPosition = new Vector3(prePos.x, transform.localPosition.y - pressValue, prePos.z);
            yield return new WaitForSeconds(0.1f);
        }

        SortManager.isView = true;
        light.SetActive(true);
        transform.localPosition = new Vector3(prePos.x, 0.2f, prePos.z);
        audio.PlayOneShot(clips[1]);
        SortUIButtonAnim.SetBool("isShow", true);
        Cursor.visible = true;
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
