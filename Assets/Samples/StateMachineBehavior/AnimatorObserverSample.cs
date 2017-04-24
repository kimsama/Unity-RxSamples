using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class AnimatorObserverSample : MonoBehaviour 
{
    ObservableStateMachineTrigger trigger;

    void Start () 
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            trigger = animator.GetBehaviour<ObservableStateMachineTrigger>();

            trigger.OnStateEnterAsObservable()
                //.Where(x => x.StateInfo.IsName("Intro"))
                .Subscribe(x => 
                { 
                    Debug.LogFormat("state name:{0} nameHash:{1} layerIndex:{2}", x.Animator.name, x.StateInfo.shortNameHash, x.LayerIndex);
                })
                .AddTo(this.gameObject);
        }
    }
}
