using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SortManager : MonoBehaviour
{
    [SerializeField]
    private GameObject RecordBox;
    [SerializeField]
    private Scrollbar GerateBar;
    [SerializeField]
    private Camera Camera;

    private int GerateCount;
    private int PreGerateCount;

    private List<Transform> Records = new List<Transform>();

    void Start()
    {
        Camera = Camera.main;
    }

    void Update()
    {
        GerateRecords();
    }

    void GerateRecords()
    {
        GerateCount = (int)(GerateBar.value / 0.01f);
        if (PreGerateCount == GerateCount)
            return;
        else if (GerateCount > PreGerateCount)
        {
            // 늘어난 갯수만큼 일단 넣어준다.
            for (int i = 0; i < GerateCount-PreGerateCount; i++)
            {
                Records.Add(Instantiate(RecordBox).transform);
            }

            AdjRecordPos();
        }
        else
        {
            // 갯수만큼 지운다
            for (int i = 0; i < PreGerateCount-GerateCount; i++)
            {
                GameObject obj = Records[Records.Count-1].gameObject;
                Records.RemoveAt(Records.Count-1);
                Destroy(obj);
            }

            AdjRecordPos();
        }

        // y 천천히 z천천히

        Camera.transform.position = new Vector3(0, 3 + (GerateCount/3f), -15 -(GerateCount*0.5f));

        PreGerateCount = GerateCount;
        Debug.Log(GerateCount);
    }

    void AdjRecordPos()
    {
        int StartXPos = GerateCount / 2 * -1;
        for (int i = 0; i < GerateCount; i++)
        {
            Records[i].transform.position = new Vector3(StartXPos + i, -0.5f, 0);
            Records[i].transform.localScale = new Vector3(1, i+1, 1);
        }
    }
}
