using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFX", menuName = "VFXSystem/VFX")]
public class VFX : ScriptableObject
{
    [Header("Audio/Visual Prefab")]
    public GameObject m_prefab;
    [Header("Pitch Random Check")]
    public bool m_pitchChange;

    [Header("Parenting And Orientation Matching")]
    public bool m_attach;
    public bool m_orient;

    public bool m_soundPersistance;
    public GameObject Spawn(Transform t)
    {
        Transform parent = m_attach ? t : null;
        Quaternion orientation = m_orient ? t.rotation : Quaternion.identity;
        GameObject newFX = Instantiate(m_prefab, t.position, orientation, parent);
        if (newFX.GetComponent<AudioSource>() && m_pitchChange)
        {
            //changes pitch of sound component in a random range
            newFX.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 2.0f);
        }
        if (!m_soundPersistance)
        {
            Despawn();
        }
        return newFX;
    }
    public void Despawn()
    {
        Destroy(this, 10);
    }
}
