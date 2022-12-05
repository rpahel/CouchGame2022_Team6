using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineSystem : MonoBehaviour {
    


    protected IEnumerator DelayedCoroutine(float delay, System.Action a) {
        yield return new WaitForSeconds(delay);
        a();
    }

    protected Coroutine RunDelayed(float delay, System.Action a) {
        return StartCoroutine(DelayedCoroutine(delay, a));
    }
}
