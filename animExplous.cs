using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animExplous : MonoBehaviour
{
    public float second = 1; // время серез которое обьект можно кдалить(длинна анимации)
    
    // Анимация взрыва, ставится на место бомбы которая взорвалась
    void Start()
    {
        Invoke("DeleteGO", second);
    }
    void DeleteGO()
    {
        Destroy(gameObject);
    }
}
