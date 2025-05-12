using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioClip[] clips;

    public enum SFXType { Shoot, Hit, Death, ButtonClick }

    public void Play(SFXType type)
    {
        AudioClip clip = clips[(int)type];
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}