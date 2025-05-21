using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;
    public static AudioManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindAnyObjectByType<AudioManager>();

                if (instance == null)
                {
                    GameObject go = new("AudioManager");
                    instance = go.AddComponent<AudioManager>();
                }
            }

            return instance;
        }
    }

    private Dictionary<string, AudioClip> m_OneShotAudios = new Dictionary<string, AudioClip>();
    private void Awake()
    {
        AudioClip[] oneShotClips = Resources.LoadAll<AudioClip>("Audios/OneShot");

        foreach (AudioClip clip in oneShotClips)
        {
            if (!m_OneShotAudios.ContainsKey(clip.name))
            {
                m_OneShotAudios.Add(clip.name, clip);
            }
        }
    }

    public void PlayOneShot(string clipName, Vector3 position)
    {
        GameObject go = new($"{clipName} SFX");
        AudioSource source = go.AddComponent<AudioSource>();
        go.transform.position = position;
        AudioClip clip = m_OneShotAudios[clipName];
        source.PlayOneShot(clip);

        Destroy(go, clip.length);
    }
}
