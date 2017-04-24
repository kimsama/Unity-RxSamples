using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class UniChanAnimatorObserver : MonoBehaviour 
{
    ObservableStateMachineTrigger[] triggers;

    void Start () 
    {
        Animator animator = GetComponent<Animator>();
        if (animator != null)
        {
            triggers = animator.GetBehaviours<ObservableStateMachineTrigger>();

            foreach (ObservableStateMachineTrigger trigger in triggers)
            {
                trigger.OnStateEnterAsObservable()
                    .Subscribe(x =>
                    {
                        Debug.LogFormat("state name:{0} nameHash:{1} layerIndex:{2}", x.Animator.name, x.StateInfo.shortNameHash, x.LayerIndex);
                    })
                    .AddTo(this.gameObject);
            }
        }
    }
}
