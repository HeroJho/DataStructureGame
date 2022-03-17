using System.Collections;
using System.Collections.Generic;
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
    private Camera Camera;

    STATE state = STATE.NOMAL;
    private int GerateCount;
    private int PreGerateCount;
    private float StartXPos;

    public static bool isView = false;

    public List<GameObject> Records = new List<GameObject>();

    void Start()
    {
        Camera = Camera.main;
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

        // 카메라
        Camera.transform.position = new Vector3(0, 3 + (GerateCount/3f), -15 -(GerateCount*0.5f));

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
        if (state != STATE.NOMAL)
            return;

        if (Records.Count <= 0)
            return;

        state = STATE.MIX;
        StartCoroutine("StartMixing");
    }
    IEnumerator StartMixing()
    {
        int mixingCount = 0;

        int index1 = 0;
        int index2 = 0;

        while (mixingCount < 200)
        {
            index1 = Random.Range(0, Records.Count);
            index2 = Random.Range(0, Records.Count);

            Swap(index1, index2);

            ++mixingCount;

            yield return null;
        }

        state = STATE.NOMAL;
    }




    public void SelectionSort()
    {
        if (state != STATE.NOMAL)
            return;

        state = STATE.SORT;
        StartCoroutine("SelectionSorting");
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

        StartCoroutine("EndSorting");
    }

    public void BubbleSort()
    {
        if (state != STATE.NOMAL)
            return;

        state = STATE.SORT;
        StartCoroutine("BubbleSorting");
    }
    IEnumerator BubbleSorting()
    {
        for (int i = Records.Count - 1; i > 0; --i)
        {
            for (int j = 0; j < i; j++)
            {
                if (GetScaleY(Records[j]) > GetScaleY(Records[j + 1]))
                {
                    Swap(j, j + 1);
                }
                yield return null;
            }
        }
    }

    public void ShellSort()
    {
        if (state != STATE.NOMAL)
            return;

        state = STATE.SORT;
        StartCoroutine("ShellSorting");
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
                        Swap(u + gap, u);
                        yield return null;
                    }
                    Records[u + gap] = objKey;
                }
            }
        }

        StartCoroutine("EndSorting");
    }

    public void MergSort()
    {
        if (state != STATE.NOMAL)
            return;

        state = STATE.SORT;
        StartCoroutine("MergeSort");
    }
    IEnumerator MergeSort()
    {
        // StartCoroutine("MergeSorting");
        yield return StartCoroutine(mergeSort(0, Records.Count-1));
        StartCoroutine("EndSorting");
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

            if(CheckLength(i-1))
                Records[i-1].GetComponent<MeshRenderer>().material.color = Color.white;
            if (CheckLength(j - 1))
                Records[j-1].GetComponent<MeshRenderer>().material.color = Color.white;

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
            yield return StartCoroutine(merge(left, mid, right)); //merge(left, mid, right);
        }
    }

    public void QuickSort()
    {
        if (state != STATE.NOMAL)
            return;

        state = STATE.SORT;
        StartCoroutine("QuickSorting");
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

        StartCoroutine("EndSorting");
    }

    public void HeapSort()
    {
        if (state != STATE.NOMAL)
            return;

        state = STATE.SORT;
        StartCoroutine("HeapSorting");
    }
    IEnumerator HeapSorting()
    {
        Heap q = new Heap();

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

        StartCoroutine("EndSorting");
    }

    public void RadixSort()
    {
        if (state != STATE.NOMAL)
            return;

        state = STATE.SORT;
        StartCoroutine("RadixSorting");
    }
    IEnumerator RadixSorting()
    {
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
                    Records[i].transform.position = new Vector3(StartXPos + i, Records[i].transform.position.y, Records[i].transform.position.z);
                    yield return null;
                    Records[i].GetComponent<MeshRenderer>().material.color = Color.white;
                    i++;
                }
            factor *= 10;
        }

        StartCoroutine("EndSorting");
    }




    IEnumerator EndSorting()
    {
        for (int i = 0; i < Records.Count; i++)
        {
            Records[i].GetComponent<MeshRenderer>().material.color = Color.green;
            yield return null;
        }
        for (int i = 0; i < Records.Count; i++)
        {
            Records[i].GetComponent<MeshRenderer>().material.color = Color.white;
        }

        state = STATE.NOMAL;
        StopAllCoroutines();
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
