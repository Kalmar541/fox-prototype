using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// <c>text</c>
/// </summary>
public class Bomb : MonoBehaviour
{
    // Start is called before the first frame update
    float bottomY =-20;                 // черта з акоторой удаляются бомбы упавние за сцену
    public float gravityScale = 1;      // множитель гравитации

    Rigidbody rb;                       // физ тело
    PlayerController player;                      // скрипт игрока

    GameObject groundGO;                // ссылка на обьект земли под бомбой
    public GameObject explousionGO;     // анимация взрыва, посдтавляется вместо бомбы
  
    SpriteRenderer spriteRenderer;
    public Sprite spriteBomb1;          // спрайт бомбы для мигания
    public Sprite spriteBomb2;          // спрайт бомбы для мигания
    
    AudioSource audioSource;            
    public AudioClip  tochBomb;         // звук касания игроком бомбы
    public AudioClip fallOnGround;      // звук падения бомбы на землю

    bool soundFallWasPlays= false;      //флаг который не задваивает звук касания.
    bool isDamageZone = false;          // флаг игрок в зоне поражения перед взрывом или нет
    bool is_toch = false;               // флаг касания бомбы игроком для откидвания
    bool isGround = false;              // флаг касания игроко мземли от двойного прыжка
    public bool is_activateExplousion;  // флаг активации is_toch бомбы если она летит в сторону BOSSа

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
       
        if (collision.gameObject.tag == "Player") // бомба считается откинутой, для попадания по противнику
        {
            is_toch = true;
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(tochBomb);
            }
            
        }
        if (collision.gameObject.CompareTag("Ground")&& (!isGround)) // таймер после которого бомба взрывается
        {
            isGround = true;
            SmenaColor();
            Invoke("Explousion",3);
        }
    }
    void Explousion() // Вызрыв уничтожает Ground под бомбой и ее саму.
    {
        Vector3 GO = transform.position;
        GameObject EX =  Instantiate(explousionGO); // анимация взрыва
        if (isDamageZone) // отнять жизни если игрок в зоне поражения бомбы
        {
            player.playerLives--;
        }
        EX.transform.position = GO;
        Destroy(gameObject);
        if (groundGO) // если бомба взорвалась на земле то уничтожает блок под собой
        {
            Destroy(groundGO);
        }              
    }
    
    void SmenaColor() // меняет материал на противоположный красный/черный
    {
        if (spriteRenderer.sprite==spriteBomb1)
        {
            spriteRenderer.sprite = spriteBomb2;
        }else spriteRenderer.sprite = spriteBomb1;
        Invoke("SmenaColor", 0.1f); // скрипт вызывает сам себя пока не будет уничтожен сам обьект
    }
    void RemoveBombsOutZone() // убрать бомбы которые упали ниже экрана
    {
        if (transform.position.y < bottomY) 
        { Destroy(this.gameObject); }
    }
    void ActivatingBobmBackUp()// если игрок толкнул бомбу и она высоко летит то активировать ее
    {
        if (transform.position.y > -3 && is_toch) 
        { is_activateExplousion = true; }
    }
    void DeactivatetAndDestroyBomb()// если активированная бомба не долетела до цели и спускается вниз то подорвать ее не высоте
    {
        {
        if (is_activateExplousion == true && rb.velocity.y < 0 && transform.position.y < -2) 
            Explousion();
        }
    }
    void AddGravity()
    {
        if (rb.velocity.y < 0) // дополнительное ускорение падающей бомбе
        {
            rb.AddForce(Vector3.down * gravityScale);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag=="Ground") // перед уничтожением сохраним на каком Ground стояла бомба
        {
            groundGO = other.gameObject;
            if (!soundFallWasPlays)
            {
                soundFallWasPlays = true;
                audioSource.PlayOneShot(fallOnGround); // звук толкания бомбы
            }
        }
        if (other.gameObject.tag=="Player") // Проверка что игрок в зоне поражения взрывом
        {          
            isDamageZone = true;
        }        
    }
    private void OnTriggerExit(Collider other) // игок покинул зону поражения перед взрывом
    {
        if (other.gameObject.tag=="Player")
        {
            isDamageZone = false;            
        }
    }
}
