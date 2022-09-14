﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
 
public class UI : MonoBehaviour
{

    /* Класс управляет элементами на канвасе, текстом, кнопками.
     * Стартовый текст появляется первым на экране, он обьясняет правила и управление, пропадает по нажатию любой клавиши.
     * На Canvas сгруппированы элементы меню-паузы( 3 кнопки: вернуться, рестарт и выход) - активируется нажатием кнопки ESC.
     * И рестарта(кнопка рестарт показывается по окончинии игры).
     *  
     */

    public GameObject restartMenuGO; // группа показывается при окончании игры
    public GameObject escapeMenuGO;  // группа меню паузы с кпопками выхода рестарта и возврата
    public GameObject player;
    public GameObject bossGO;
    public GameObject leftWarn;
    public GameObject rightWarn;
    public GameObject redBomb;
    public GameObject surikenGO;

    public float chanceAttackSuriken = 0.001f; // 0.005 -3  0.0025 -2 0.001 -3

    public PlayerController basketScript;
    public Boss appleTreeScript;
    public AudioBank auduoBankScript;
    
    public GameObject startingTxt;   // текст при старте игры, описывающий элементы управления
    public TMPro.TextMeshProUGUI playerLivesTMP;
    public TMPro.TextMeshProUGUI bossLivesTMP;
    public TMPro.TextMeshProUGUI textGameOverTMP;

    public bool GameIsWIN;           // флаг указывающий на победу в игре
    public bool escapeVisible;       // флаг видимости меню паузы
    float timeScaleActual;           // хранит скорость игры, что бы вернуть ее после паузы.
    void Start()
    {
        escapeVisible = escapeMenuGO.activeSelf;
        timeScaleActual = Time.timeScale;
        Time.timeScale = 0; // даем игроку прочитать стартовый текст, игра на паузе
        basketScript = player.GetComponent<PlayerController>();
        appleTreeScript = bossGO.GetComponent<Boss>();
        auduoBankScript = GetComponent<AudioBank>();

        //StartCoroutine(MigatTablichkoi(derect.Left));
        

    }

    // Update is called once per frame
    void Update()
    {   // GameIsWIN устанавливается на true если игра закончена победой
        if (GameIsWIN) //заменим текс на победный если игрок выиграл
        {
            textGameOverTMP.text = "ПОБЕДА!!!";
            
        }
        else // текст поражения если игрок проиграл
        {
            textGameOverTMP.text = "GAME OVER, ты проиграл...";
        }
        // Нажатие на ESC вызывает меню в игре, активируя обьекты на UI канвасе
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapeVisible = !escapeVisible; // escapeVisible - флаг видимости меню ESC

            escapeMenuGO.SetActive(escapeVisible);
            if (escapeVisible)
            {
                Time.timeScale = 0; // поставим игру на паузу, пока показывается ESC меню
            }
            else Time.timeScale = timeScaleActual; // пауза выключена, игра продолжается
        }
        // здесь снимается стартовый текст по нажатию на любую клавишу 
        if (Input.anyKeyDown &&startingTxt.activeSelf==true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // если эта любая клавиша стала клавишей ESC, то мы должнв не продолжить игру а поставить на паузу сразу как она начнется.
            {
                Time.timeScale = 0; 
                escapeMenuGO.SetActive(escapeVisible);
                startingTxt.SetActive(false);
                return;
            }
            startingTxt.SetActive( false); // запустим игру и уберем стартовый текст.
            Time.timeScale = timeScaleActual;
        }
        //---------- отрисовка UI ----------//
        playerLivesTMP.text= "PLAYER LIVES x "+ basketScript.playerLives;
        bossLivesTMP.text = "BOSS LIVES x "+ appleTreeScript.lives;
    }
    void FixedUpdate()
    {       
        if (Random.value < chanceAttackSuriken) 
        {
            countSuriken++;
            if (Random.value<0.5)
            {
                
                StartCoroutine(MigatTablichkoi(derect.Left));
            } else StartCoroutine(MigatTablichkoi(derect.Rigth));
        }
    }
    public void Restart() // кнопка Рестарт из меню ESC
    {   // Перезагрузим сцену
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        escapeVisible = false;
        Time.timeScale = timeScaleActual;
    }
    public void ExitApplication()
    {   //выход из игры в рабочий стол
        Application.Quit();
        Debug.Log("Вы вышли из игры");
    }
    public void Rescume()
    {   //снимает с паузы
        escapeVisible = false;
        escapeMenuGO.SetActive(escapeVisible);
        Time.timeScale = timeScaleActual;
    }
    public enum derect { Left,Rigth}
    public int countSuriken = 0;

    IEnumerator MigatTablichkoi(derect dir)
    {
        float second=0.25f;
        GameObject surikenGO;
        GameObject plate= leftWarn;    
        int i = 8;
        switch (dir)
        {
            case derect.Left:
                plate = leftWarn;
                break;
            case derect.Rigth:
                plate = rightWarn;
                break;
            default:
                break;
        }        
        while (i > 0)
        {
           if (plate.activeSelf)
           {
               yield return new WaitForSeconds(second);
               plate.SetActive(false);
           }
           else
           {
               yield return new WaitForSeconds(second);
               plate.SetActive(true);
           }
            
            i--;
        }
        plate.SetActive(false);


        while (countSuriken>0)
        {
            yield return new WaitForSeconds(1);
            surikenGO = Instantiate(this.surikenGO);
            surikenGO.transform.position = plate.transform.position;
            countSuriken--;
        }
                /*surikenGO = Instantiate(this.surikenGO);
                surikenGO.transform.position = plate.transform.position; */
    }
    
    
}
