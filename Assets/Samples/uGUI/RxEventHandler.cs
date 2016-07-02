using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UniRx;

/// <summary>
/// A simple demonstration of stamina charge system similar as Dark Soul.
/// </summary>
public class RxEventHandler : MonoBehaviour 
{
    // stamina gauge
    public Slider slider;

    // decrease stamina with the given amount whenever the button is clicked.
    public Button actionButton;

    // decreasing amount of stamina whenever the action is done.
    public float amount = 0.2f;

    // how much gain stamina per frame
    public float delta = 0.002f;

    // delay time before increasing stamina gauge
    public float delay = 2.0f;

    Subject<Slider> spStartSubject = new Subject<Slider>();
    Subject<Slider> spEndSubject = new Subject<Slider>();

	void Start () 
    {
        slider.value = 1f;

        var spStartStream = spStartSubject.AsObservable();
        var spEndStream = spEndSubject.AsObservable();

        // charge stamina
        var recoveryStream = Observable.EveryUpdate()
            .SkipUntil(spStartStream) // start  when the spStartStream is arrived
            .Select( _ => slider.value )
            .TakeUntil(spEndStream)   // repeat spEndStream is arrived
            .RepeatUntilDisable(this)
            .Subscribe(v =>
                {
                    // It increases the value until it reaches to 1.
                    slider.value +=  this.delta;

                    if (slider.value >= 1f)
                        spEndSubject.OnNext(slider); 
                }, 
                ()=> 
                {
                    Debug.Log("completed");
                });

        IDisposable cancel = null;

        // Whenever the button is clicked, it decrease the value of the slider.
        var buttonStream = actionButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log("button click.");

                // force stop if any coroutine already runs.
                if (cancel != null)
                    cancel.Dispose(); 

                cancel = Observable.FromCoroutine(StartStaminaRecovery)
                                       .Subscribe();
            });
    }


    IEnumerator StartStaminaRecovery()
    {
        // subtract stamina with the givent amount of value.
        slider.value -= this.amount;

        // stop the current event on the recoveryStream.
        spEndSubject.OnNext(slider);

        // Hold on for the delay time before increasing stamina.
        yield return new WaitForSeconds(this.delay);

        // send the start event on the recoveryStream.
        spStartSubject.OnNext(slider);
    }
	
}
