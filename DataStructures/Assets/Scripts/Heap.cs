using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeapNode
{
	private float key;
	private GameObject obj;
	
	public void setKey(GameObject _obj) { obj = _obj; key = obj.transform.localScale.y; }
	public GameObject getObj() { return obj; }
	public float getKey() { return key; }
};

public class Heap
{
	const int MAX_ELEMENT = 200;
	private HeapNode[] node = new HeapNode[MAX_ELEMENT];
	private int size = 0;

	public int getSize() { return size; }
	public bool isEmpty() { return size == 0; }
	public bool isFull() { return size == MAX_ELEMENT - 1; }

	HeapNode getParent(int i) { return node[i / 2]; }		// 부모 노드
	HeapNode getLeft(int i) { return node[i * 2]; }        // 왼쪽 자식 노드
	HeapNode getRight(int i) { return node[i * 2 + 1]; }   // 오른쪽 자식 노드

	public HeapNode curRemoveItem = null;

	public void insert(GameObject obj)
	{
		float key = obj.transform.localScale.y;
		if (isFull()) return;
		int i = ++size;         // 증가된 힙 크기 위치에서 시작

		// 트리를 거슬러 올라가면서 부모 노드와 비교하는 과정
		while (i != 1 && key < getParent(i).getKey())   // 루트가 아니고 부모 노드보다 크면
		{
			node[i] = getParent(i);                     // 부모를 자신 노드로 끌어내림
			i /= 2;                                     // 한 레벨 상승
		}
		node[i] = new HeapNode();                            // 최종 위치 데이터 복사
		node[i].setKey(obj);
	}

	IEnumerator removeing()
    {

		yield return null;
    }

	public IEnumerator remove()
	{
		// if (isEmpty()) return null;
		HeapNode item = node[1];        // 루트노드(꺼낼 요소)
		HeapNode last = node[size--];   // 힙의 마지막 노드

		GameObject temp = null;

		int parent = 1;                 // 마지막 노드의 위치를 루트로 생각
		int child = 2;                  // 루트의 왼쪽 자식 위치
		while (child <= size)           // 힙 트리를 벗어나지 않는 동안
		{
			temp = node[parent].getObj();
			temp.GetComponent<MeshRenderer>().material.color = Color.red;

			temp.GetComponent<AudioSource>().pitch = node[parent].getKey() / 100f;
			temp.GetComponent<AudioSource>().Play();

			// 현재 노드의 자식 노드 중 더 큰 자식 노드를 찾음
			if (child < size && getLeft(parent).getKey() > getRight(parent).getKey())
				child++;                // child: 더 큰 자식 노드 인덱스
										// 마지막 노드가 더 큰 자식보다 크면 이동 완료.
			if (last.getKey() <= node[child].getKey()) break;

			// 아니면 한 단계 아래로 이동
			node[parent] = node[child];
			parent = child;
			child *= 2;

			yield return null;
			temp.GetComponent<MeshRenderer>().material.color = Color.white;
		}
		node[parent] = last;    // 마지막 노드를 최종 위치에 저장
		curRemoveItem = item;            // 루트 노드 반환
	}
	public HeapNode find() { return node[1]; }


}
