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

	HeapNode getParent(int i) { return node[i / 2]; }		// �θ� ���
	HeapNode getLeft(int i) { return node[i * 2]; }        // ���� �ڽ� ���
	HeapNode getRight(int i) { return node[i * 2 + 1]; }   // ������ �ڽ� ���

	public HeapNode curRemoveItem = null;

	public void insert(GameObject obj)
	{
		float key = obj.transform.localScale.y;
		if (isFull()) return;
		int i = ++size;         // ������ �� ũ�� ��ġ���� ����

		// Ʈ���� �Ž��� �ö󰡸鼭 �θ� ���� ���ϴ� ����
		while (i != 1 && key < getParent(i).getKey())   // ��Ʈ�� �ƴϰ� �θ� ��庸�� ũ��
		{
			node[i] = getParent(i);                     // �θ� �ڽ� ���� �����
			i /= 2;                                     // �� ���� ���
		}
		node[i] = new HeapNode();                            // ���� ��ġ ������ ����
		node[i].setKey(obj);
	}

	IEnumerator removeing()
    {

		yield return null;
    }

	public IEnumerator remove()
	{
		// if (isEmpty()) return null;
		HeapNode item = node[1];        // ��Ʈ���(���� ���)
		HeapNode last = node[size--];   // ���� ������ ���

		GameObject temp = null;

		int parent = 1;                 // ������ ����� ��ġ�� ��Ʈ�� ����
		int child = 2;                  // ��Ʈ�� ���� �ڽ� ��ġ
		while (child <= size)           // �� Ʈ���� ����� �ʴ� ����
		{
			temp = node[parent].getObj();
			temp.GetComponent<MeshRenderer>().material.color = Color.red;

			temp.GetComponent<AudioSource>().pitch = node[parent].getKey() / 100f;
			temp.GetComponent<AudioSource>().Play();

			// ���� ����� �ڽ� ��� �� �� ū �ڽ� ��带 ã��
			if (child < size && getLeft(parent).getKey() > getRight(parent).getKey())
				child++;                // child: �� ū �ڽ� ��� �ε���
										// ������ ��尡 �� ū �ڽĺ��� ũ�� �̵� �Ϸ�.
			if (last.getKey() <= node[child].getKey()) break;

			// �ƴϸ� �� �ܰ� �Ʒ��� �̵�
			node[parent] = node[child];
			parent = child;
			child *= 2;

			yield return null;
			temp.GetComponent<MeshRenderer>().material.color = Color.white;
		}
		node[parent] = last;    // ������ ��带 ���� ��ġ�� ����
		curRemoveItem = item;            // ��Ʈ ��� ��ȯ
	}
	public HeapNode find() { return node[1]; }


}
