using UnityEngine;

public class FinalAudio : MonoBehaviour
{
    public AudioClip duringBoss;
    public AudioSource source;

    public void ReplaceMusic()
    {
        source.clip = duringBoss;
        source.Play();
    }
}
