using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum AttackType
{
    Missile
}
public class PoolManager : MonoBehaviour
{
    public static PoolManager instance;
    public Dictionary<AttackType, List<GameObject>> projectilePool;
    public Dictionary<AttackType, List<GameObject>> impactPool;


    private void Awake()
    {
        instance = this;
        projectilePool = new Dictionary<AttackType, List<GameObject>>();
        impactPool = new Dictionary<AttackType, List<GameObject>>();
    }

    public GameObject RequestProjectile(AttackType weaponType, Transform obj, Vector3 pos, Quaternion rot)
    {
        List<GameObject> li;
        projectilePool.TryGetValue(weaponType, out li);
        if (li == null)
        {
            li = new List<GameObject>();
            projectilePool.Add(weaponType, li);
        }
        for (int i = 0; i < li.Count; i++)
        {
            if (!li[i].activeSelf)
            {
                li[i].transform.position = pos;
                li[i].transform.rotation = rot;
                li[i].gameObject.SetActive(true);
                li[i].GetComponent<Missile>().OnSpawned();
                return li[i].gameObject;
            }
        }
        GameObject newObj = Instantiate(obj.gameObject, pos, rot);
        li.Add(newObj);
        newObj.SetActive(true);
        newObj.GetComponent<Missile>().OnSpawned();
        return newObj;
    }

    public GameObject RequestImpact(AttackType weaponType, Transform obj, Vector3 pos, Quaternion rot)
    {
        List<GameObject> li;
        impactPool.TryGetValue(weaponType, out li);
        if (li == null)
        {
            li = new List<GameObject>();
            impactPool.Add(weaponType, li);
        }
        for (int i = 0; i < li.Count; i++)
        {
            if (!li[i].activeSelf)
            {
                li[i].transform.position = pos;
                li[i].transform.rotation = rot;
                li[i].gameObject.SetActive(true);
                return li[i].gameObject;
            }
        }
        GameObject newObj = Instantiate(obj.gameObject, pos, rot);
        li.Add(newObj);
        newObj.SetActive(true);
        newObj.GetComponent<Despawn>().OnSpawned();
        return newObj;
    }
}
