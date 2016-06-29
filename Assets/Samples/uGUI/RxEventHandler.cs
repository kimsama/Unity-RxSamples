using UnityEngine;
using System;
using System.Collections;
using UnityEngine.UI;
using UniRx;

public class RxEventHandler : MonoBehaviour 
{
    public Slider slider;
    public Button button;

	void Start () 
    {
        slider.value = 1f;

        // Handles button's onclick event. 
        // Whenever the button is clicked, it decrease the value of the slider.
        button.OnClickAsObservable()
            .Subscribe(_ => 
            {
                Debug.Log("button click.");
                slider.value -= 0.2f;
            });

        var sliderStream = slider.OnValueChangedAsObservable();

        // 1) wait until slider's value is changed
        // 2) when the slider's value is changed 
        // 3) it increases the value until it reaches to 1.
        Observable.EveryUpdate()
            .SkipUntil(sliderStream)
            .Select( _ => slider.value )
            .TakeWhile(x => x < 1f)
            .RepeatUntilDisable(this)
            .Subscribe(v =>
                {
                    slider.value += 0.002f;
                });

	}

    IEnumerator UpdateSlider()
    {
        Debug.Log("1st updateslider");
        yield return new WaitForSeconds(1f);
        Debug.Log("2nd updateslider");

        while (slider.value <= 1f)
        {
            slider.value += 0.001f;
            yield return 0;
        }
    }
	
}
