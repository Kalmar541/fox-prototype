using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Apple : MonoBehaviour
{
    // Start is called before the first frame update
    float bottomY =-20;                 // ����� � �������� ��������� ����� ������� �� �����
    public float gravityScale = 1;      // ��������� ����������

    Rigidbody rb;                       // ��� ����
    PlayerController player;                      // ������ ������

    GameObject groundGO;                // ������ �� ������ ����� ��� ������
    public GameObject explousionGO;     // �������� ������, ������������� ������ �����
  
    SpriteRenderer spriteRenderer;
    public Sprite spriteBomb1;          // ������ ����� ��� �������
    public Sprite spriteBomb2;          // ������ ����� ��� �������
    
    AudioSource audioSource;            
    public AudioClip  tochBomb;         // ���� ������� ������� �����
    public AudioClip fallOnGround;      // ���� ������� ����� �� �����

    bool soundFallWasPlays= false;      //���� ������� �� ���������� ���� �������.
    bool isDamageZone = false;          // ���� ����� � ���� ��������� ����� ������� ��� ���
    bool is_toch = false;               // ���� ������� ����� ������� ��� ����������
    bool isGround = false;              // ���� ������� ������ ������ �� �������� ������
    public bool is_activateExplousion;  // ���� ��������� is_toch ����� ���� ��� ����� � ������� BOSS�

    void Start()
    {
        rb = GetComponent<Rigidbody>();    
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (GameObject.Find("Player")!=null)
        {
            player = GameObject.Find("Player").GetComponent<PlayerController>();
        }
        
       
    }

    // Update is called once per frame
    void Update()
    {
        RemoveBombsOutZone();
        ActivatingBobmBackUp();
        DeactivatetAndDestroyBomb();

    }
    private void FixedUpdate()
    {
        AddGravity();
    }
    private void OnCollisionEnter(Collision collision)
    {
       
        if (collision.gameObject.tag == "Player") // ����� ��������� ���������, ��� ��������� �� ����������
        {
            is_toch = true;
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(tochBomb);
            }
            
        }
        if (collision.gameObject.CompareTag("Ground")&& (!isGround)) // ������ ����� �������� ����� ����������
        {
            isGround = true;
            SmenaColor();
            Invoke("Explousion",3);
        }
    }
    void Explousion() // ������ ���������� Ground ��� ������ � �� ����.
    {
        Vector3 GO = transform.position;
        GameObject EX =  Instantiate(explousionGO); // �������� ������
        if (isDamageZone) // ������ ����� ���� ����� � ���� ��������� �����
        {
            player.playerLives--;
        }
        EX.transform.position = GO;
        Destroy(gameObject);
        if (groundGO) // ���� ����� ���������� �� ����� �� ���������� ���� ��� �����
        {
            Destroy(groundGO);
        }              
    }
    
    void SmenaColor() // ������ �������� �� ��������������� �������/������
    {
        if (spriteRenderer.sprite==spriteBomb1)
        {
            spriteRenderer.sprite = spriteBomb2;
        }else spriteRenderer.sprite = spriteBomb1;
        Invoke("SmenaColor", 0.1f); // ������ �������� ��� ���� ���� �� ����� ��������� ��� ������
    }
    void RemoveBombsOutZone() // ������ ����� ������� ����� ���� ������
    {
        if (transform.position.y < bottomY) 
        { Destroy(this.gameObject); }
    }
    void ActivatingBobmBackUp()// ���� ����� ������� ����� � ��� ������ ����� �� ������������ ��
    {
        if (transform.position.y > -3 && is_toch) 
        { is_activateExplousion = true; }
    }
    void DeactivatetAndDestroyBomb()// ���� �������������� ����� �� �������� �� ���� � ���������� ���� �� ��������� �� �� ������
    {
        {
        if (is_activateExplousion == true && rb.velocity.y < 0 && transform.position.y < -2) 
            Explousion();
        }
    }
    void AddGravity()
    {
        if (rb.velocity.y < 0) // �������������� ��������� �������� �����
        {
            rb.AddForce(Vector3.down * gravityScale);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Ground") // ����� ������������ �������� �� ����� Ground ������ �����
        {
            groundGO = other.gameObject;
            if (!soundFallWasPlays)
            {
                soundFallWasPlays = true;
                audioSource.PlayOneShot(fallOnGround); // ���� �������� �����
            }
        }
        if (other.gameObject.tag=="Player") // �������� ��� ����� � ���� ��������� �������
        {          
            isDamageZone = true;
        }        
    }
    private void OnTriggerExit(Collider other) // ���� ������� ���� ��������� ����� �������
    {
        if (other.gameObject.tag=="Player")
        {
            isDamageZone = false;            
        }
    }
}
