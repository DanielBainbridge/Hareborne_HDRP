using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "VFX", menuName = "VFXSystem/VFX")]
public class VFX : ScriptableObject
{
    public GameObject m_prefab;
    public bool m_attach;
    public bool m_orient;
    public GameObject Spawn(Transform t)
    {
        Transform parent = m_attach ? t : null;
        Quaternion orientation = m_orient ? t.rotation : Quaternion.identity;
        GameObject newFX = Instantiate(m_prefab, t.position, orientation, parent);
        if (newFX.GetComponent<AudioSource>())
        {
            newFX.GetComponent<AudioSource>().pitch = Random.Range(0.5f, 2.0f);
        }
        return newFX;
    }
    public void Despawn()
    {
        Destroy(this, 10);
    }
}
