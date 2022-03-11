using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineData<T>
{
    public Coroutine coroutine { get; private set; }
    public T result;
    private IEnumerator target;

    private int maxCalls = 1;
    private int currentCalls = 0;
    
    public CoroutineData(MonoBehaviour owner, IEnumerator target) {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
    }

    public CoroutineData(MonoBehaviour owner, IEnumerator target, int maxCalls) {
        this.target = target;
        this.coroutine = owner.StartCoroutine(Run());
        this.maxCalls = maxCalls;
    }

    private IEnumerator Run() {
        result = default(T);
        try {
            while (target.MoveNext()) {
                result = (T) target.Current;
                if (currentCalls + 1 < maxCalls) {
                    target.Reset();
                    currentCalls++;
                }
            }
        } catch (System.Exception e) {
            Debug.LogWarning(e);
        }

        yield return result;
     }
}
