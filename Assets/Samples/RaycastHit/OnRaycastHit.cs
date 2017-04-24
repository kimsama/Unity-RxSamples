using UnityEngine;
using System.Collections;
using System.Linq;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// Attach to a gameobject which is needed to do ray cast hit detection.
/// 
/// </summary>
public class OnRaycastHit : MonoBehaviour
{
    // stay message 
    public Subject<Unit> onRaycastStayStream = new Subject<Unit>();

    public IObservable<Unit> onRaycastStayAsObservable { get { return onRaycastStayStream.AsObservable(); } }

    // exit message 
    public Subject<Unit> onRaycastExitStream = new Subject<Unit>();

    public IObservable<Unit> onRaycastExitAsObservable { get { return onRaycastExitStream.AsObservable(); } }

    // enter message 
    public Subject<Unit> onRaycastEnterStream = new Subject<Unit>();

    public IObservable<Unit> onRaycastEnterAsObservable { get { return onRaycastEnterStream.AsObservable(); } }

    private bool isOnNext;

    public void RaycastHit()
    {
        isOnNext = true;
    }

    void Start()
    {
        this.UpdateAsObservable()
            .Select(_ => isOnNext)
            .Buffer(2, 1)
            .Subscribe(list =>
            {
                bool before = list.First();
                bool current = list.Last();
                if (!current && before) onRaycastExitStream.OnNext(default(Unit));
                if (current && !before) onRaycastEnterStream.OnNext(default(Unit));
                if (current) onRaycastStayStream.OnNext(default(Unit));
                isOnNext = false;
            });
    }
}