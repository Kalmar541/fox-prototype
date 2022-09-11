using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppleTree : MonoBehaviour
{  
    
    public GameObject applePrefab;                  //������ �������� �����
    public float speed = 1f;
    public float leftAndRightEdge = 10f;            //��������� �� ������� ���������������� ����������� �������� ������ 
    public float chanceToChangeDirections = 0.1f;   //����������� ���������� ��������� ��������   
    public float secondsBetweenDropApple = 2f;      //������� ������ �����
    public int lives=1;
    bool beginDownOnce = true;                      // ���� �� �������� BOSS ������ ����
    public float timeOnChange = 1f;                 // ������ �������� ����� ����� ��������
    public GameObject animExplousions;              // �������� �������
    public SpriteRenderer sprite;

    void Start()
    {        
        //���������� ������ ��� � �������
        Invoke("DropApple", 2f);       
        capsColl = GetComponent<CapsuleCollider>();
    }
  
    void Update()
    {           
        //������� �����������
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        
        if (lives < 1)
        {
            pos.y -=Mathf.Abs( speed) * Time.deltaTime;  
            if ( beginDownOnce)
            {
                AnimBossDead(); // ��������� 1 ���
                beginDownOnce = false;
            }            
        }
        transform.position = pos;
        //��������� �����������, � ��������� ��������, ��� �� ������ �� �������� ����� �����
        //����� ����������� ���� ���� ����� ����������� ����� �����
        if (timeOnChange>0)
        {
            timeOnChange -=Time.deltaTime;
        }
        
        //BOSS ��������� ����� � ������ ���� �� ��������� ������� �� ������ �������, ����� ���� ���������������
        if (pos.x < -leftAndRightEdge&&timeOnChange<=0)
        {           
            speed *=-1;
            timeOnChange = 1f;
        } 
        if (pos.x > leftAndRightEdge && timeOnChange <= 0)
        {
            speed *= -1;       
            timeOnChange = 1f;
        }
        if (speed < 0)
        {
            sprite.flipX = false;
        }
        else sprite.flipX = true;

        if (transform.position.y<-20)
        {
            CancelInvoke("AnimBossDead");
            Camera.main.GetComponent<UI>().restartMenuGO.SetActive(true);
            Camera.main.GetComponent<UI>().GameIsWIN = true;
        }
    }
    private void FixedUpdate()
    {
        if (Random.value < chanceToChangeDirections&& timeOnChange <= 0)
        {
            speed *= -1;

        }
    }
    
    void DropApple()
    {
        GameObject apple = Instantiate<GameObject>(applePrefab);
        apple.transform.position = transform.position;
        Invoke("DropApple", secondsBetweenDropApple);
        
    }
    public CapsuleCollider capsColl;
    
     void AnimBossDead() { //������� ������ ������ BOSS� ����� �� ���������

        CancelInvoke("DropApple"); // ���������� ����� ����
        Vector3 centrPoint = transform.position;     // ������� � ������� ��������� BOSS  
        float radius = 4; // ������ �������� �������� �������
        //������� ����� �������� ������ ������ BOSS�
        Vector3 randomPos = new Vector3(Random.value-0.5f, Random.value-0.5f, 0).normalized*radius+centrPoint;
        Vector3 pos = new Vector3(0,0,2.9f); // ��������� ���������� Z ��� �� ������ ������ ���������� ������ BOSS�
        Instantiate(animExplousions, randomPos+pos,Quaternion.identity);
        Invoke("AnimBossDead", 0.33f); 
    }
    private void OnTriggerEnter(Collider other) // ������� ��������� ���� ������� ��������
    {
        if (other.gameObject.tag == "Apple" && other.gameObject.GetComponent<Apple>().is_activateExplousion == true)
        {
            lives--;
            Vector3 tempPos=other.transform.position ;
            GameObject go = Instantiate(animExplousions); // ��������� �������� ������
            go.transform.position = tempPos;
            Destroy(other.gameObject);
        }
    }
}
