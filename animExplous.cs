using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animExplous : MonoBehaviour
{
    public float second = 1; // ����� ����� ������� ������ ����� �������(������ ��������)
    
    // �������� ������, �������� �� ����� ����� ������� ����������
    void Start()
    {
        Invoke("DeleteGO", second);
    }
    void DeleteGO()
    {
        Destroy(gameObject);
    }
}
