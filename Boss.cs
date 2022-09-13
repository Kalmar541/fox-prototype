using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
 
public class Boss : MonoBehaviour
{  /*
    * Скрипт для управления Боссом. Босс движется влево и вправо над игроком и сбрасывает бомбы,
    * которые должны взрывом либо задеть игрока либо разружить основание под ним. Босс побежден,
    * если у него закончились жизни и падает вниз.
    
    * Обьект Boss доходит до края экрана и разворачивается, есть шанс случайного изменения 
    * направления движения. Бомбы создаются через определенное время, падают вниз и уничтожают основание под собой
    * или/и ранят игрока. 
    * BOSS падает вниз если у него закончились жизни. 
    * TODO - с изменением сложности BOSS долже менять спрайт на более внушительный, двигаться быстрее, быстрее скидывать бомбы.
    * BOSS падает вниз если у него закончились жизни, деактивируется на сцене, ему заменяется спрайт, меняются поля и он снова помещается 
    * над игроком и так пока последний босс не будет повержен.
    */

    public GameObject bombPrefab;                  //шаблон создания яблок
    public float speed = 1f;
    public float leftAndRightEdge = 10f;            //растояние на котором должноизменяться направление движения яблони 
    public float chanceToChangeDirections = 0.1f;   //вероятность случайного изменения движения   
    
    public float secondsBetweenDropApple = 2f;      //частота сброса яблок
    public int lives=1;

    bool beginDownOnce = true;                      // флаг по которому BOSS падает вниз
    bool BossIsDead = false;                        // Boss повержен, для запуска анимации уничтожения и спавна заново.

    public float timeOnChange = 1f;                 // таймер ожидания после смены движения
    public GameObject animExplousions;              // анимация взрывов
    public SpriteRenderer sprite;
    public Sprite[] sprites;                //спрайты босса мин сред и большой
    public int level = 1;                   // уровень игры задает хар-ки BOSS, сложность
    public Vector3 posBosGO;

    void Start()
    {        
        //сбрасывать яблоки раз в секунду
        Invoke("DropApple", 2f);       
        capsColl = GetComponent<CapsuleCollider>();
    }
  
    void Update()
    {           
        //простое перемещение
        Vector3 posBosGO = transform.position;
        posBosGO.x += speed * Time.deltaTime;
        transform.position = posBosGO;
        if (lives<0)
        {
            lives = 0;
        }
        if (lives < 1)
        {
            BossIsDead = true;
            MoveBossDown(-22);        // спустим BOSS за сцену, у него кончились жизни 
            if (BossIsDead)
            {
                if (beginDownOnce)
                {
                    AnimBossDead(); // сработает 1 раз
                    beginDownOnce = false;
                    CancelInvoke("DropApple"); // прекратить сброс бомб
                }
                
            }
                
        }
        else
        {
            MoveBossDown(10); // опустим нового BOSS до уровня игрока
            
        }
        
        //изменение направления, с ожиданием задержки, что бы обьект не дергался сразу после
        //смены направления если шанс смены направления опять выпал
        if (timeOnChange>0)
        {
            timeOnChange -=Time.deltaTime;
        }
        
        //BOSS двигается влево и вправо пока не достигнет крайних на экране позиций, после чего разворачивается
        if (posBosGO.x < -leftAndRightEdge&&timeOnChange<=0)
        {           
            speed *=-1;
            timeOnChange = 1f;
        } 
        if (posBosGO.x > leftAndRightEdge && timeOnChange <= 0)
        {
            speed *= -1;       
            timeOnChange = 1f;
        }
        if (speed < 0)
        {
            sprite.flipX = false;
        }
        else sprite.flipX = true;

    }
    private void FixedUpdate()
    {
        if (Random.value < chanceToChangeDirections&& timeOnChange <= 0)
        {
            speed *= -1;

        }
    }
    void SetDiffical(int level)
    {
        AudioBank AB = Camera.main.GetComponent<AudioBank>();
        switch (level)
        {
            case 2:
                speed = 8;
                lives = 7;
                secondsBetweenDropApple = 3;
                sprite.sprite = sprites[1];
                AB.audioSource.pitch = 0.95f;
                Camera.main.GetComponent<UI>().chanceAttackSuriken = 0.002f;  
                break;
            case 3:
                speed = 10;
                lives = 10;
                secondsBetweenDropApple = 2.5f;
                sprite.sprite = sprites[2];
                Camera.main.GetComponent<UI>().chanceAttackSuriken = 0.004f;
                AB.audioSource.pitch = 1f;
                break;
            case 4:
                Camera.main.GetComponent<UI>().restartMenuGO.SetActive(true);
                Camera.main.GetComponent<UI>().GameIsWIN = true;
                gameObject.SetActive(false);
                CancelInvoke("DropApple");
                AB.audioSource.pitch = 0.9f;
                break;


        }
    }
    void MoveBossDown(float yPos)
    //спускает BOSSа вниз до координаты yPos. Если Boss повержен BossIsDead(true), то ставит его над сценой,
    //что бы снова плавно спустить до уровня над игроком.
    {
        Vector3 pos;
        pos = transform.position;
        if (transform.position.y > yPos)
        {            
            pos.y -= 5 * Time.deltaTime;
            transform.position = pos;
     
        }
        else if(BossIsDead)
        {
           
            CancelInvoke("AnimBossDead");
            pos.y = 23;
            transform.position = pos;
            BossIsDead = false;
            Invoke("DropApple", 2f);
            level++;
            SetDiffical(level);
            beginDownOnce = true;

        }
        }
  
    void DropApple()
    {
        GameObject apple = Instantiate<GameObject>(bombPrefab);
        apple.transform.position = transform.position;
        Invoke("DropApple", secondsBetweenDropApple);
        
    }
    public CapsuleCollider capsColl;
    
     void AnimBossDead() { //спавнит взрывы вокруг BOSSа когда он уничтожен

        
        Vector3 centrPoint = transform.position;     // позиция в которой находится BOSS  
        float radius = 4; // радиус разброса анимаций взрывов
        //выбрать точку анимации взрыва вокруг BOSSа
        Vector3 randomPos = new Vector3(Random.value-0.5f, Random.value-0.5f, 0).normalized*radius+centrPoint;
        Vector3 pos = new Vector3(0,0,2.9f); // подвинуть координату Z что бы спрайт взрыва перекрывал спрайт BOSSа
        Instantiate(animExplousions, randomPos+pos,Quaternion.identity);
        Invoke("AnimBossDead", 0.33f); 
    }
    private void OnTriggerEnter(Collider other) // считает попадания бомб которые откинули
    {
        if (other.gameObject.tag == "Apple" && other.gameObject.GetComponent<Bomb>().is_activateExplousion == true)
        {
            lives--;
            Vector3 tempPos=other.transform.position ;
            GameObject go = Instantiate(animExplousions); // проиграть анимацию взрыва
            go.transform.position = tempPos;
            Destroy(other.gameObject);
        }
    }
}
