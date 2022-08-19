using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class AudioManager : MonoBehaviour
{
    private static AudioManager instance = null;
    public Transform litenerPos = null;
    public static AudioManager Instance { get { return instance; } }

    public List<AudioClip> clipList = new List<AudioClip>();
    public AudioSource audioSource = null;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }    
        audioSource = GetComponent<AudioSource>();
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLoadScene;
    }
    private void OnLoadScene(Scene arg0, LoadSceneMode arg1)
    {
        litenerPos = FindObjectOfType<AudioListener>().transform;
    }
    public void PlayOneShot(AudioClip clip)
    {
      audioSource.PlayOneShot(clip);
    }    
    public void PlayOneShotAtPoint(AudioClip clip, Vector3 position)
    {
       AudioSource.PlayClipAtPoint(clip, position);
    }    
}
