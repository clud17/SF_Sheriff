using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    public AudioClip ShootingSound;  // 총기 발사 사운드
    public AudioClip ChargeSound;  // 충전 사운드
    public AudioClip ReloadSound;
    private AudioSource aud;
    
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); // 씬이 바뀌어도 살아남기
        aud = GetComponent<AudioSource>();
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip != null && aud != null)
            aud.PlayOneShot(clip);
    }
}
