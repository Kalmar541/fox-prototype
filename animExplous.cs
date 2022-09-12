using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class animExplous : MonoBehaviour 
{
    //Скрипт создает анимацию взрыва. second должен быть равен величине времени анимации.
    //Спрайт взрыва помещается на место где (исчезла) взорвалась бомба. На GO висит звук взрыва,
    //которой воспроизводится при инициалиализации обьекта

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
