using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SortManager : MonoBehaviour
{
    enum STATE { NOMAL, MIX, SORT}

    [SerializeField]
    private GameObject RecordBox;
    [SerializeField]
    private Scrollbar GerateBar;
    [SerializeField]
    private Dropdown WayForSorting;
    [SerializeField]
    private GameObject SortingButton;

    STATE state = STATE.NOMAL;
    private int GerateCount;
    private int PreGerateCount;
    private float StartXPos;
    public static bool isView = false;

    Coroutine nowCo = null;
    List<GameObject> Records = new List<GameObject>();
    List<GameObject> TempRecords = new List<GameObject>();

    public Image pan;
    IEnumerator PadeOutPan()
    {
        float a = 255f;
        float t = 0;
        while (a > 0)
        {
            t += 0.1f;
            a -= t;
            pan.color = new Color(pan.color.r, pan.color.g, pan.color.b, a / 255f);
            yield return new WaitForSeconds(0.1f);
        }
    }

    void Start()
    {
        Screen.SetResolution(1920, 1080, true);
        StartCoroutine(PadeOutPan());
        Cursor.visible = false;
    }

    void Update()
    {
        if(state == STATE.NOMAL)
        {
            GerateRecords();
        }
    }

    void GerateRecords()
    {
        GerateCount = (int)(GerateBar.value / 0.01f);
        if (PreGerateCount == GerateCount)
            return;
        else if (GerateCount > PreGerateCount)
        {
            GameObject obj = null;

            // 늘어난 갯수만큼 일단 넣어준다.
            for (int i = 0; i < GerateCount-PreGerateCount; i++)
            {
                obj = Instantiate(RecordBox);
                Records.Add(obj);
                obj.name = "Record_" + Records.Count;
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

        PreGerateCount = GerateCount;
    }
    void AdjRecordPos()
    {
        StartXPos = GerateCount / 2 * -1;
        for (int i = 0; i < GerateCount; i++)
        {
            Records[i].transform.position = new Vector3(StartXPos + i, -0.5f, 0);
            Records[i].transform.localScale = new Vector3(1, i+1, 1);
        }
    }

    public void MixRecord()
    {
        if (nowCo != null)
            return;

        if (Records.Count <= 0)
            return;

        state = STATE.MIX;

        SortingButton.GetComponentInChildren<Text>().text = "Stoping";
        SortingButton.GetComponentInChildren<Image>().color = new Color(245 / 255f, 10 / 255f, 10 / 255f, 100 / 255f);
        nowCo = StartCoroutine("StartMixing");
    }
    IEnumerator StartMixing()
    {
        int mixingCount = 0;

        int index1 = 0;
        int index2 = 0;
        float cR = 0;
        float cG = 0;
        float cB = 0;

        while (mixingCount < 200)
        {
            index1 = Random.Range(0, Records.Count);
            index2 = Random.Range(0, Records.Count);

            cR = Random.Range(0, 1f);
            cG = Random.Range(0, 1f);
            cB = Random.Range(0, 1f);

            Records[index1].GetComponent<AudioSource>().pitch = GetScaleY(Records[index1]) / 100f;
            Records[index1].GetComponent<AudioSource>().Play();

            Records[index1].GetComponent<MeshRenderer>().material.color = new Color(cR, cG, cB, 0.7f);
            Records[index2].GetComponent<MeshRenderer>().material.color = new Color(cR, cG, cB, 0.7f);

            yield return null;

            Swap(index1, index2);
            ++mixingCount;
        }

        for (int i = 0; i < Records.Count; i++)
        {
            Records[i].GetComponent<MeshRenderer>().material.color = Color.white;
            yield return null;
        }

        state = STATE.NOMAL;
        nowCo = null;
        SortingButton.GetComponentInChildren<Text>().text = "Sorting";
        SortingButton.GetComponentInChildren<Image>().color = new Color(245 / 255f, 245 / 255f, 245 / 255f, 100 / 255f);
    }

    bool isheapri = false;
    public void StartOrStopSorting()
    {
        if (nowCo != null)
        {
            if(isheapri)
            {
                Records = TempRecords.ToList();
                AdjRecordPos();
                isheapri = false;
            }

            StopAllCoroutines();
            nowCo = null;
            state = STATE.NOMAL;
            SortingButton.GetComponentInChildren<Text>().text = "Sorting";
            SortingButton.GetComponentInChildren<Image>().color = new Color(245 / 255f, 245 / 255f, 245 / 255f, 100 / 255f);
            return;
        }

        state = STATE.SORT;

        SortingButton.GetComponentInChildren<Text>().text = "Stoping";
        SortingButton.GetComponentInChildren<Image>().color = new Color(245/255f, 10 / 255f, 10 / 255f, 100 / 255f);

        isheapri = false;
        switch (WayForSorting.value)
        {
            case 0:
                nowCo = StartCoroutine("SelectionSorting");
                break;
            case 1:
                nowCo = StartCoroutine("BubbleSorting");
                break;
            case 2:
                nowCo = StartCoroutine("ShellSorting");
                break;
            case 3:
                nowCo = StartCoroutine("MergeSort");
                break;
            case 4:
                nowCo = StartCoroutine("QuickSorting");
                break;
            case 5:
                nowCo = StartCoroutine("HeapSorting");
                isheapri = true;
                break;
            case 6:
                nowCo = StartCoroutine("RadixSorting");
                isheapri = true;
                break;
            case 7:
                nowCo = StartCoroutine("BogoSorting");
                break;
            default:
                break;
        }
    }
    



    IEnumerator SelectionSorting()
    {
        for (int i = 0; i < Records.Count - 1; i++)
        {
            int least = i;
            Records[i].GetComponent<MeshRenderer>().material.color = Color.yellow;
            for (int j = i + 1; j < Records.Count; j++)
            {
                Records[j].GetComponent<MeshRenderer>().material.color = Color.red;

                Records[j].GetComponent<AudioSource>().pitch = GetScaleY(Records[j]) / 100f;
                Records[j].GetComponent<AudioSource>().Play();

                if (GetScaleY(Records[j]) < GetScaleY(Records[least]))
                {
                    if(i != least)
                        Records[least].GetComponent<MeshRenderer>().material.color = Color.white;
                    least = j;
                    Records[least].GetComponent<MeshRenderer>().material.color = Color.blue;
                }            
                yield return null;
                if(least != j)
                    Records[j].GetComponent<MeshRenderer>().material.color = Color.white;
            }
            Records[least].GetComponent<MeshRenderer>().material.color = Color.white;
            Records[i].GetComponent<MeshRenderer>().material.color = Color.white;
            Swap(i, least);
        }

        nowCo = StartCoroutine("EndSorting");
    }

    IEnumerator BubbleSorting()
    {
        for (int i = Records.Count - 1; i > 0; --i)
        {
            for (int j = 0; j < i; j++)
            {
                Records[j].GetComponent<MeshRenderer>().material.color = Color.red;

                yield return null;

                if (GetScaleY(Records[j]) > GetScaleY(Records[j + 1]))
                {
                    Records[j+1].GetComponent<AudioSource>().pitch = GetScaleY(Records[j + 1]) / 100f;
                    Records[j + 1].GetComponent<AudioSource>().Play();
                    Records[j].GetComponent<MeshRenderer>().material.color = Color.white;
                    Swap(j, j + 1);
                }
                else
                {
                    Records[j + 1].GetComponent<AudioSource>().pitch = GetScaleY(Records[j + 1]) / 100f;
                    Records[j + 1].GetComponent<AudioSource>().Play();

                    Records[j].GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }
        }

        nowCo = StartCoroutine("EndSorting");
    }

    IEnumerator ShellSorting()
    {
        int z, u;
        GameObject objKey;

        for (int gap = Records.Count / 2; gap > 0; gap = gap / 2)
        {
            if ((gap % 2) == 0) gap++;
            for (int i = 0; i < gap; i++)
            {
                for (z = i + gap; z <= Records.Count - 1; z = z + gap)
                {
                    objKey = Records[z];
                    for (u = z - gap; u >= i && GetScaleY(objKey) < GetScaleY(Records[u]); u = u - gap)
                    {
                        Records[u].GetComponent<MeshRenderer>().material.color = Color.red;

                        Records[u].GetComponent<AudioSource>().pitch = GetScaleY(Records[u]) / 100f;
                        Records[u].GetComponent<AudioSource>().Play();

                        yield return null;

                        Records[u].GetComponent<MeshRenderer>().material.color = Color.white;

                        Swap(u + gap, u);
                    }
                    Records[u + gap] = objKey;
                }
            }
        }

        nowCo = StartCoroutine("EndSorting");
    }

    IEnumerator MergeSort()
    {
        // StartCoroutine("MergeSorting");
        yield return StartCoroutine(mergeSort(0, Records.Count-1));
        nowCo = StartCoroutine("EndSorting");
    }
    IEnumerator merge(int left, int mid, int right)
    {
        float xPos = Records[left].transform.position.x;

        // left가 현재 합병중인 첫번째 위치 left-right
        // 합병된 순서대로 left부터 right까지 위치 변경

        int i, j, k = left, l;
        GameObject[] sorted = new GameObject[100];
        // 분할 정렬된 A의 합병
        for (i = left, j = mid + 1; i <= mid && j <= right;)
        {
            if (CheckLength(i) && CheckLength(j))
            {
                Records[i].GetComponent<MeshRenderer>().material.color = Color.red;
                Records[j].GetComponent<MeshRenderer>().material.color = Color.red;

                Records[i].GetComponent<AudioSource>().pitch = GetScaleY(Records[i]) / 100f;
                Records[i].GetComponent<AudioSource>().Play();

                Records[j].GetComponent<AudioSource>().pitch = GetScaleY(Records[j]) / 100f;
                Records[j].GetComponent<AudioSource>().Play();
            }

            yield return null;

            GameObject obj = null;
            if (GetScaleY(Records[i]) <= GetScaleY(Records[j]))
            {
                obj = Records[i++];
            }
            else
            {
                obj = Records[j++];
            }

            if (CheckLength(i-1)) Records[i - 1].GetComponent<MeshRenderer>().material.color = Color.white;
            if (CheckLength(j - 1)) Records[j - 1].GetComponent<MeshRenderer>().material.color = Color.white;

            sorted[k++] = obj;
        }



        // 한쪽에 남아 있는 레코드의 일괄 복사
        if (i > mid)
            for (l = j; l <= right; l++, k++)
                sorted[k] = Records[l];
        else
            for (l = i; l <= mid; l++, k++)
                sorted[k] = Records[l];
        // 배열 sorted[]의 리스트를 배열 A[]로 재복사
        for (l = left; l <= right; l++)
        {
            Records[l] = sorted[l];
            Records[l].transform.position = new Vector3(xPos + (l-left), Records[l].transform.position.y, 0);
            Records[l].GetComponent<MeshRenderer>().material.color = Color.red;
            yield return null;
            Records[l].GetComponent<MeshRenderer>().material.color = Color.white;
        }
    }
    IEnumerator mergeSort(int left, int right)
    {
        if (left < right)
        {
            int mid = (left + right) / 2;
            yield return StartCoroutine(mergeSort(left, mid));
            yield return StartCoroutine(mergeSort(mid + 1, right));
            yield return StartCoroutine(merge(left, mid, right));
        }
    }

    IEnumerator quickSort(int left, int right)
    {
        if (left < right)
        {
            // 좌우 분할
            int low = left;
            int high = right + 1;
            float pivot = GetScaleY(Records[left]);            // 피벗 설정
            Records[left].GetComponent<MeshRenderer>().material.color = Color.yellow;
            do
            {
                do                          // 왼쪽 리스트에서 피벗보다 큰 레코드 선택
                {
                    low+=1;

                    if(CheckLength(low))
                    {
                        Records[low].GetComponent<MeshRenderer>().material.color = Color.red;

                        Records[low].GetComponent<AudioSource>().pitch = GetScaleY(Records[low]) / 100f;
                        Records[low].GetComponent<AudioSource>().Play();

                        yield return null;
                        Records[low].GetComponent<MeshRenderer>().material.color = Color.white;
                    }

                } while (low <= right && GetScaleY(Records[low]) < pivot);
                do                          // 오른쪽 리스트에서 피벗보다 작은 레코드 선택
                {
                    high-=1;

                    if (CheckLength(high))
                    {
                        Records[high].GetComponent<MeshRenderer>().material.color = Color.red;

                        Records[high].GetComponent<AudioSource>().pitch = GetScaleY(Records[high]) / 100f;
                        Records[high].GetComponent<AudioSource>().Play();

                        yield return null;
                        Records[high].GetComponent<MeshRenderer>().material.color = Color.white;
                    }

                } while (high >= left && GetScaleY(Records[high]) > pivot);
                if (low < high)             // 선택된 레코드 교환
                {
                    Swap(low, high);
                    yield return null;
                }

            } while (low < high);

            Swap(left, high);         // 마지막으로 피벗 중간에 옮기기
            yield return null;

            yield return StartCoroutine(quickSort(left, high - 1));
            yield return StartCoroutine(quickSort(high + 1, right));
        }
    }
    IEnumerator QuickSorting()
    {
        yield return StartCoroutine(quickSort(0, Records.Count-1));

        nowCo = StartCoroutine("EndSorting");
    }

    IEnumerator HeapSorting()
    {
        Heap q = new Heap();
        TempRecords = Records.ToList();

        for (int i = 0; i < Records.Count; i++)
        {
            q.insert(Records[i]);
        }

        for (int i = 0; !q.isEmpty(); i++)
        {
            yield return StartCoroutine(q.remove());
            HeapNode node = q.curRemoveItem;
            Records[i] = node.getObj();
            Records[i].transform.position = new Vector3(StartXPos + i, Records[i].transform.position.y, Records[i].transform.position.z);
        }

        yield return null;

        nowCo = StartCoroutine("EndSorting");
    }

    IEnumerator RadixSorting()
    {
        Heap q = new Heap();
        TempRecords = Records.ToList();

        Queue<GameObject>[] queues = new Queue<GameObject>[10];

        for (int i = 0; i < 10; i++)
        {
            queues[i] = new Queue<GameObject>();
        }

        int factor = 1;
        for (int d = 0; d < 4; d++)
        {
            for (int i = 0; i < Records.Count; i++)                 // 데이터 자릿수에 따라 큐에 삽입
                queues[((int)GetScaleY(Records[i]) / factor) % 10].Enqueue(Records[i]);

            for (int b = 0, i = 0; b < 10; b++)       // 버킷에서 꺼내 list로 합친다
                while (queues[b].Count != 0)
                {
                    Records[i] = queues[b].Dequeue();
                    Records[i].GetComponent<MeshRenderer>().material.color = Color.red;

                    Records[i].GetComponent<AudioSource>().pitch = GetScaleY(Records[i]) / 100f;
                    Records[i].GetComponent<AudioSource>().Play();

                    Records[i].transform.position = new Vector3(StartXPos + i, Records[i].transform.position.y, Records[i].transform.position.z);
                    yield return null;
                    Records[i].GetComponent<MeshRenderer>().material.color = Color.white;
                    i++;
                }
            factor *= 10;
        }

        nowCo = StartCoroutine("EndSorting");
    }

    IEnumerator BogoSorting()
    {
        int key = 0;
        int mixingCount = 0;
        int index1 = 0, index2 = 0;
        float cR = 0, cG = 0, cB = 0;

        while (true)
        {
            if (key >= Records.Count)
                break;

            while (mixingCount < 50)
            {
                index1 = Random.Range(key, Records.Count);
                index2 = Random.Range(key, Records.Count);

                cR = Random.Range(0, 1f);
                cG = Random.Range(0, 1f);
                cB = Random.Range(0, 1f);

                Records[index1].GetComponent<AudioSource>().pitch = GetScaleY(Records[index1]) / 100f;
                Records[index1].GetComponent<AudioSource>().Play();

                Records[index1].GetComponent<MeshRenderer>().material.color = new Color(cR, cG, cB, 0.7f);
                Records[index2].GetComponent<MeshRenderer>().material.color = new Color(cR, cG, cB, 0.7f);

                Swap(index1, index2);
                ++mixingCount;
            }
            mixingCount = 0;
            yield return null;

            if((key + 1) == GetScaleY(Records[key]))
            {
                Records[key].GetComponent<MeshRenderer>().material.color = Color.white;
                ++key;
            }

        }

        nowCo = StartCoroutine("EndSorting");
    }



    IEnumerator EndSorting()
    {
        for (int i = 0; i < Records.Count; i++)
        {
            Records[i].GetComponent<MeshRenderer>().material.color = Color.green;
            Records[i].GetComponent<AudioSource>().pitch = 2+ GetScaleY(Records[i]) / 100f;
            Records[i].GetComponent<AudioSource>().Play();
            yield return null;
        }
        for (int i = 0; i < Records.Count; i++)
        {
            Records[i].GetComponent<MeshRenderer>().material.color = Color.white;
        }

        state = STATE.NOMAL;
        SortingButton.GetComponentInChildren<Text>().text = "Sorting";
        SortingButton.GetComponentInChildren<Image>().color = new Color(245 / 255f, 245 / 255f, 245 / 255f, 100 / 255f);
        nowCo = null;
    }

    void Swap(int index1, int index2)
    {
        GameObject obj1 = null;
        GameObject obj2 = null;
        Vector3 postemp;

        obj1 = Records[index1].gameObject;
        obj2 = Records[index2].gameObject;

        Records[index1] = obj2;
        Records[index2] = obj1;

        postemp = obj1.transform.position;
        obj1.transform.position = obj2.transform.position;
        obj2.transform.position = postemp;
    }
    float GetScaleY(GameObject obj)
    {
        return obj.transform.localScale.y;
    }
    bool CheckLength(int index)
    {
        if (index < 0 || index >= Records.Count)
        {
            return false;
        }

        return true;
    }

}
