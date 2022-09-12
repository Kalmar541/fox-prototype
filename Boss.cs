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
    public float timeOnChange = 1f;                 // таймер ожидания после смены движения
    public GameObject animExplousions;              // анимация взрывов
    public SpriteRenderer sprite;

    void Start()
    {        
        //сбрасывать яблоки раз в секунду
        Invoke("DropApple", 2f);       
        capsColl = GetComponent<CapsuleCollider>();
    }
  
    void Update()
    {           
        //простое перемещение
        Vector3 pos = transform.position;
        pos.x += speed * Time.deltaTime;
        
        if (lives < 1)
        {
            pos.y -=Mathf.Abs( speed) * Time.deltaTime;  
            if ( beginDownOnce)
            {
                AnimBossDead(); // сработает 1 раз
                beginDownOnce = false;
            }            
        }
        transform.position = pos;
        //изменение направления, с ожиданием задержки, что бы обьект не дергался сразу после
        //смены направления если шанс смены направления опять выпал
        if (timeOnChange>0)
        {
            timeOnChange -=Time.deltaTime;
        }
        
        //BOSS двигается влево и вправо пока не достигнет крайних на экране позиций, после чего разворачивается
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
        GameObject apple = Instantiate<GameObject>(bombPrefab);
        apple.transform.position = transform.position;
        Invoke("DropApple", secondsBetweenDropApple);
        
    }
    public CapsuleCollider capsColl;
    
     void AnimBossDead() { //спавнит взрывы вокруг BOSSа когда он уничтожен

        CancelInvoke("DropApple"); // прекратить сброс бомб
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
