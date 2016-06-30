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
    public float delay = 1.0f;

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
                    slider.value += this.delta;

                    if (slider.value >= 1f)
                        spEndSubject.OnNext(slider); 
                });

        // Whenever the button is clicked, it decrease the value of the slider.
        actionButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log("button click.");

                // subtract stamina with the g
                slider.value -= this.amount;

                // stop the current stream
                spEndSubject.OnNext(slider);

                Observable.FromCoroutine(UpdateSlider)
                    .Subscribe();
            });

	}


    IEnumerator UpdateSlider()
    {
        // delay before increasing stamina
        yield return new WaitForSeconds(this.delay);

        // send the event to start the stream.
        spStartSubject.OnNext(slider);
    }
	
}
