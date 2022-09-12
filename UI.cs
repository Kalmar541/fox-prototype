using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UI : MonoBehaviour
{

    // ����� ��������� ���������� �� �������, �������, ��������.

    public GameObject restartMenuGO; // ������ ������������ ��� ��������� ����
    public GameObject escapeMenuGO;  // ������ ���� ����� � �������� ������ �������� � ��������
    public GameObject player;
    public GameObject bossGO;
    public PlayerController basketScript;
    public Boss appleTreeScript;
    public AudioBank auduoBankScript;
    
    public GameObject startingTxt;   // ����� ��� ������ ����, ����������� �������� ����������
    public TMPro.TextMeshProUGUI playerLivesTMP;
    public TMPro.TextMeshProUGUI bossLivesTMP;
    public TMPro.TextMeshProUGUI textGameOverTMP;

    public bool GameIsWIN;           // ���� ����������� �� ������ � ����
    public bool escapeVisible;       // ���� ��������� ���� �����
    float timeScaleActual;           // ������ �������� ����, ��� �� ������� �� ����� �����.
    void Start()
    {
        escapeVisible = escapeMenuGO.activeSelf;
        timeScaleActual = Time.timeScale;
        Time.timeScale = 0; // ���� ������ ��������� ��������� �����, ���� �� �����
        basketScript = player.GetComponent<PlayerController>();
        appleTreeScript = bossGO.GetComponent<Boss>();
        auduoBankScript = GetComponent<AudioBank>();



    }

    // Update is called once per frame
    void Update()
    {   // GameIsWIN ��������������� �� true ���� ���� ��������� �������
        if (GameIsWIN) //������� ���� �� �������� ���� ����� �������
        {
            textGameOverTMP.text = "������!!!";
            
        }
        else // ����� ��������� ���� ����� ��������
        {
            textGameOverTMP.text = "GAME OVER, �� ��������...";
        }
        // ������� �� ESC �������� ���� � ����, ��������� ������� �� UI �������
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            escapeVisible = !escapeVisible; // escapeVisible - ���� ��������� ���� ESC

            escapeMenuGO.SetActive(escapeVisible);
            if (escapeVisible)
            {
                Time.timeScale = 0; // �������� ���� �� �����, ���� ������������ ESC ����
            }
            else Time.timeScale = timeScaleActual; // ����� ���������, ���� ������������
        }
        // ����� ��������� ��������� ����� �� ������� �� ����� ������� 
        if (Input.anyKeyDown &&startingTxt.activeSelf==true)
        {
            if (Input.GetKeyDown(KeyCode.Escape)) // ���� ��� ����� ������� ����� �������� ESC, �� �� ������ �� ���������� ���� � ��������� �� ����� ����� ��� ��� ��������.
            {
                Time.timeScale = 0; 
                escapeMenuGO.SetActive(escapeVisible);
                startingTxt.SetActive(false);
                return;
            }
            startingTxt.SetActive( false); // �������� ���� � ������ ��������� �����.
            Time.timeScale = timeScaleActual;
        }
        //---------- ��������� UI ----------//
        playerLivesTMP.text= "PLAYER LIVES x "+ basketScript.playerLives;
        bossLivesTMP.text = "BOSS LIVES x "+ appleTreeScript.lives;
    }
    public void Restart() // ������ ������� �� ���� ESC
    {   // ������������ �����
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        escapeVisible = false;
        Time.timeScale = timeScaleActual;
    }
    public void ExitApplication()
    {   //����� �� ���� � ������� ����
        Application.Quit();
        Debug.Log("�� ����� �� ����");
    }
    public void Rescume()
    {   //������� � �����
        escapeVisible = false;
        escapeMenuGO.SetActive(escapeVisible);
        Time.timeScale = timeScaleActual;
    }
    
}
