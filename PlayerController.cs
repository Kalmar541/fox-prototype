using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public int playerLives = 1;         // жизни игрока

    public float speed = 10;            // скорость игрока
    public float jumpForceTest = 1;     // сила прыжка
    public float cancelRate = 100;      // сила приземл€юща€ игрока после прыжка
    public float buttonTime=0.5f;       // длинна прыжка по времени максимальна€
    float timeJump;                     // врем€ , которое игрок держит конпку прыжка

    Rigidbody rb;                       //физ. тело
    public SpriteRenderer sprite;
    public Animator anim;

     bool jumpCancelled;
     bool isJump;                 // игрок прыгает , дл€ fixedupfate
     bool is_ground;              //проверка на касание земли

    AudioSource AudioSource;
    public AudioClip jump;              // звук прыжка
 
    // Start is called before the first frame update
    void Start()
    {
    rb          = GetComponent<Rigidbody>();
    AudioSource = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    
    void Update()
    {
        ChangeAnimations();

        //----ввод клавиш, управление-----
        if (Input.GetKeyDown(KeyCode.UpArrow)&&is_ground)
        {
            float jumpForce = /*Mathf.Sqrt(*/jumpForceTest * -2 * Physics.gravity.y/*)*/;
            rb.AddForce(new Vector3(0, jumpForce, 0));
            isJump = true;
            jumpCancelled = false;
            timeJump = 0;         
            AudioSource.PlayOneShot(jump);            
        }
        if (isJump)
        {
            timeJump += Time.deltaTime;
            if (Input.GetKeyUp(KeyCode.UpArrow))
            {
                jumpCancelled = true;
            }
            if (timeJump > buttonTime)
            {
                isJump = false;
            }
        }
        // если жизни кончились, включить кнопки "рестарт меню" а модель игрока деактивировать

        //------работа со сценой------
        if (playerLives<1) //предложить рестар если проиграл
        {
            AudioBank AB =  Camera.main.GetComponent<AudioBank>()  ; 
            
            AB.StopPlaying();
            AB.PlaySound(AB.soundPlayerFall);
            
            Camera.main.GetComponent<UI>().restartMenuGO.SetActive(true);
            gameObject.SetActive(false);
        }
        //вернем игрока на сцену если он упал и отнимем жизнь
        if (transform.position.y < -20)
        {           
                //transform.position = Vector3.zero;
                playerLives=0;
                       
        }
       
    }
    
    private void FixedUpdate()
    {
        MoveHorizontal();
     //------ввод клавиш-----
        if (Input.GetKey(KeyCode.UpArrow))
        {        
            if (!is_ground)
            {
                if (rb.velocity.y > 0)
                {
                    anim.SetBool("isJump", true);
                }
                if (rb.velocity.y < 0)
                {
                    anim.SetBool("isJump", false);
                }
            }

        }
        //ускоренное приземление
        if (jumpCancelled && isJump && rb.velocity.y > 0|| timeJump > buttonTime)
        {
            rb.AddForce(Vector3.down * cancelRate);
        }
    }  
    
    void MoveHorizontal()
    {
        float moveHorizontal = Input.GetAxis("Horizontal");
        Vector3 movement = new Vector3(moveHorizontal * speed, rb.velocity.y, 0);
        rb.velocity = movement;
    }
    void ChangeAnimations()
    {
        float moveHorizontal = Input.GetAxis("Horizontal"); // ћен€ем анимацию с
        //-----работа анимации-----
        if (Mathf.Abs(moveHorizontal) > 0.1)
        {
            anim.SetBool("run", true);
        }
        else anim.SetBool("run", false);
        if (rb.velocity.y < 0 && !is_ground)
        {
            anim.SetBool("isJump", false);
        }
        anim.SetFloat("velocityY", rb.velocity.y);
        anim.SetBool("idle", is_ground);
        if (moveHorizontal < 0)
        {
            sprite.flipX = true;
        }
        if (moveHorizontal > 0)
        {
            sprite.flipX = false;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            is_ground = true;
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            is_ground = false;
        }
    }



}

