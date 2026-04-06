using UnityEngine;

public class AVHandlerScript : MonoBehaviour
{
    public static AVHandlerScript instance; //marks singleton. I really shound have done this for PlayScript too 

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void playSoundFXClip(AudioClip audioClip, float volume = 1f)
    {
        AudioSource audioSource = Instantiate(soundFXObject);
        audioSource.volume = volume;
        audioSource.clip = audioClip;
        audioSource.Play();
        float clipLength = audioSource.clip.length;

        Destroy(audioSource.gameObject, clipLength);

    }


}
