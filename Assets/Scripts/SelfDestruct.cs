using System.Collections;
using UnityEngine;

public class SelfDestruct : MonoBehaviour
{
    public float seconds;

    void Start() { StartCoroutine(Poof(seconds)); }

    private IEnumerator Poof(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Destroy(gameObject);
    }
}
