using System.Collections.Generic;
using UnityEngine;

public class AudioManager : ManagerBase
{
    private Transform audioContainer;
    private List<Sound> sounds = new List<Sound>();

    private void Awake() => base.onConstructed.AddListener(OnConstructed);

    private void OnConstructed()
    {
        audioContainer = new GameObject("audio-container").transform;
        audioContainer.SetParent(transform);
        GetSounds();

        SetTheme();
    }

    private int themeIndex = 0;
    private void SetTheme()
    {
        Sound s = sounds.Find(c => c.name == "theme");
        if (s == null)
        {
            Debug.LogWarning("Sounds not found! Check it.");
            return;
        }

        themeIndex++;
        AudioClip ac = Resources.Load<AudioClip>("_sounds/theme-" + themeIndex);

        if (ac == null)
        {
            themeIndex = 1;
            ac = Resources.Load<AudioClip>("_sounds/theme-" + themeIndex);
        }

        s.source.clip = ac;
        GameManager.Instance.delayManager.Set(ac.length, () =>
        {
            SetTheme();
        });
        s.source.Play();
    }

    private void GetSounds()
    {
        Audios audios = GameManager.Instance.dataManager.sounds;

        for (int i = 0; i < audios.sounds.Count; i++)
        {
            Sound s = new Sound();
            s.name = audios.sounds[i].name;

            s.source = new GameObject(s.name).AddComponent<AudioSource>();
            s.source.transform.SetParent(audioContainer);
            s.source.clip = Resources.Load<AudioClip>("_sounds/" + audios.sounds[i].prefabName);
            s.source.playOnAwake = false;
            s.source.volume = audios.sounds[i].type == 0 ?
                                                GameManager.Instance.dataManager.user.gameData.settings.fxLevel :
                                                GameManager.Instance.dataManager.user.gameData.settings.musicLevel;
            s.source.loop = audios.sounds[i].isLoop;

            sounds.Add(s);
        }
    }

    public void Play(string name)
    {
        Sound s = sounds.Find(c => c.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sounds not found! Check it.");
            return;
        }

        s.source.Play();
    }

    public void Pause(string name)
    {
        Sound s = sounds.Find(c => c.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sounds not found! Check it.");
            return;
        }

        s.source.Pause();
    }

    public void Stop(string name)
    {
        Sound s = sounds.Find(c => c.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sounds not found! Check it.");
            return;
        }

        s.source.Stop();
    }

    public void SetVolume(string name, float volume)
    {
        Sound s = sounds.Find(c => c.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sounds not found! Check it.");
            return;
        }

        s.source.volume = volume;
    }

    public void SetPitch(string name, float pitch)
    {
        Sound s = sounds.Find(c => c.name == name);
        if (s == null)
        {
            Debug.LogWarning("Sounds not found! Check it.");
            return;
        }

        s.source.pitch = pitch;
    }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioSource source;
    }
}