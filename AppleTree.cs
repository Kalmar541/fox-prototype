using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AppleTree : MonoBehaviour
{  
    
    public GameObject applePrefab;                  //шаблон создани€ €блок
    public float speed = 1f;
    public float leftAndRightEdge = 10f;            //расто€ние на котором должноизмен€тьс€ направление движени€ €блони 
    public float chanceToChangeDirections = 0.1f;   //веро€тность случайного изменени€ движени€   
    public float secondsBetweenDropApple = 2f;      //частота сброса €блок
    public int lives=1;
    bool beginDownOnce = true;                      // флаг по которому BOSS падает вниз
    public float timeOnChange = 1f;                 // таймер ожидани€ после смены движени€
    public GameObject animExplousions;              // анимаци€ взрывов
    public SpriteRenderer sprite;

    void Start()
    {        
        //сбрасывать €блоки раз в секунду
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
        //изменение направлени€, с ожиданием задержки, что бы обьект не дергалс€ сразу после
        //смены направлени€ если шанс смены направлени€ оп€ть выпал
        if (timeOnChange>0)
        {
            timeOnChange -=Time.deltaTime;
        }
        
        //BOSS двигаетс€ влево и вправо пока не достигнет крайних на экране позиций, после чего разворачиваетс€
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
    
     void AnimBossDead() { //спавнит взрывы вокруг BOSSа когда он уничтожен

        CancelInvoke("DropApple"); // прекратить сброс бомб
        Vector3 centrPoint = transform.position;     // позици€ в которой находитс€ BOSS  
        float radius = 4; // радиус разброса анимаций взрывов
        //выбрать точку анимации взрыва вокруг BOSSа
        Vector3 randomPos = new Vector3(Random.value-0.5f, Random.value-0.5f, 0).normalized*radius+centrPoint;
        Vector3 pos = new Vector3(0,0,2.9f); // подвинуть координату Z что бы спрайт взрыва перекрывал спрайт BOSSа
        Instantiate(animExplousions, randomPos+pos,Quaternion.identity);
        Invoke("AnimBossDead", 0.33f); 
    }
    private void OnTriggerEnter(Collider other) // считает попадани€ бомб которые откинули
    {
        if (other.gameObject.tag == "Apple" && other.gameObject.GetComponent<Apple>().is_activateExplousion == true)
        {
            lives--;
            Vector3 tempPos=other.transform.position ;
            GameObject go = Instantiate(animExplousions); // проиграть анимацию взрыва
            go.transform.position = tempPos;
            Destroy(other.gameObject);
        }
    }
}
