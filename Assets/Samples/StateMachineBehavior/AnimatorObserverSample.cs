using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

public class AnimatorObserverSample : MonoBehaviour {

    
    Animator animator;
    ObservableStateMachineTrigger trigger;
    IObservable<ObservableStateMachineTrigger.OnStateInfo> stateEnterObservable;

	// Use this for initialization
	void Start () {
        
        animator = GetComponent<Animator>();
        if (animator != null)
        {
            trigger = animator.GetBehaviour<ObservableStateMachineTrigger>();
            //stateEnterObservable = trigger.OnStateEnterAsObservable();
            //stateEnterObservable.Where(x => x.StateInfo.IsName("Waiting_Stance")).Subscribe(x => Debug.LogFormat("{0} {1} {2}", x.Animator.name, x.StateInfo.shortNameHash, x.LayerIndex))
            //    .AddTo(this.gameObject);
            // Waiting_Stance
            trigger.OnStateEnterAsObservable()
                .Where(x => x.StateInfo.IsName("Intro")).Subscribe(x => Debug.LogFormat("{0} {1} {2}", x.Animator.name, x.StateInfo.shortNameHash, x.LayerIndex))
                .AddTo(this.gameObject);
                
        }
        
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
