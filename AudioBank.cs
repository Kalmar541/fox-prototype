using System.Collections;
using System.Collections.Generic;
using UnityEngine; 

public class AudioBank : MonoBehaviour
{
    /* Класс хранит озвучку для эффектов.
     * Звуковые эффекты, которые создаются вместе с игровыми обьектами при инициализации или во время их существования,
     * могут находиться на самих GO. Озвучка фоновой музыки хранится здесь. Обьекты, которые будут механикой игры уничтожены,
     * помещаются сюда и проигрываются здесь.
     */
    public AudioClip soundEndWon;
    public AudioClip GameSound;
    public AudioClip soundPlayerFall;
    AudioSource audioSource;
    bool wonSoundIsPlay = false;
    public float time;
    UI uiScript;
    
    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        uiScript = GetComponent<UI>();
        audioSource.PlayOneShot(GameSound);

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            audioSource.Pause();
            
        }
        if (Time.timeScale==1)
        {
            audioSource.Play();
        }
        time = Time.timeScale;
        if (uiScript.GameIsWIN&& !wonSoundIsPlay)
        {
            wonSoundIsPlay = true;
            audioSource.Stop();
            audioSource.clip = soundEndWon;
           //audioSource.PlayOneShot(soundEndWon);

            audioSource.PlayOneShot(soundEndWon,0.4f);
            audioSource.Play();

        }
    }
     
    public void PlaySound(AudioClip sound)
    {
        audioSource.PlayOneShot(sound);
    }
    public void StopPlaying()
    {
        audioSource.Stop();
    }
}
