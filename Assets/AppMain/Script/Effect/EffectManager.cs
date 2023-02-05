using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    [SerializeField]
    private List<EffectData> _effectPrefabs;

    public static EffectManager Instance;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void ShowCharacterEffect(EffectData.EFFECT effectLabel, Vector3 position)
    {
        Debug.Log("ShowCharacterEffect");
        GameObject particle = null;

        foreach(EffectData effectData in _effectPrefabs)
        {
            if (effectData.effectLabel == effectLabel)
            {
                particle = effectData.effectParticle;
            }
        }

        if (particle != null)
        {
            Debug.Log("DeadEffect" + position);
            particle = Instantiate(particle, position, Quaternion.identity);

            particle.GetComponent<ParticleSystem>().Play();
        }

    }
}

[System.Serializable]
public class EffectData
{
    public enum EFFECT
    {
        Die,
    }

    public EFFECT effectLabel;
    public GameObject effectParticle;
    //public SE se;
    //public AudioClip audioClip;
    //[Range(0, 1)]
    //public float volume = 1;
}
