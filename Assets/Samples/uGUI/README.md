
uGUI Integration Sample
========================

이 예제에서는 다크소울의 스태미나 시스템과 유사한 시스템을 rx의 연산과 이벤트를 통해 처리하는 방법에 대해서 소개하는 것으로 아래의 내용들을 포함한다. 

* uGUI 이벤트 처리
* Subject를 이용한 메시지 통지
* 하나 이상의 Rx 연산들의 조합(SkipUntil + TakeUntil + Repeat)


기본적인 시스템의 내용은 다음과 같다. 

1. Action 버튼(다크소울에서의 공격, 구르기 등)을 클릭하면 미리 정한 양(amount)만큼의 스태미나가 감소한다. 
2. 일정 시간(delay)이 지난 후 스태미나는 자동으로 회복된다.
3. 스태미나가 회복중에 다시 액션을 실행하는 경우 2번을 반복한다.


이를 이벤트의 관점에서 나누어서 분석해 보면 아래와 같이 정리할 수 있다. 

1. 액션 이벤트(예제에서는 Action 버튼 클릭한 경우)가 발생한 경우 스태미너를 감소한 후
2. deley만큼 대기 후에 감소 이벤트를 통지한다. 
3. 감소 이벤트를 통지하면 스태미너를 채우는 스트림을 반복한다. 
4. 스태미너를 채우는 스트림을 반복하는 도중에 다시 액션 이벤트가 통지되면 반복을 중지(중요!)한 후 2. 3.을 반복한다.
5. 스태미너를 모두 회복하면 (slider.value == 1) 스태미너 감소 이벤트가 통지될 때까지 대기한다.

먼저 스태미너의 감소와 회복 완료에 대한 두 가지 이벤트가 필요하다. RX에서 이와 같은 사용자 정의형 이벤트는 Subject를 사용해서 생성할 수 있다. 

``` csharp 
    Subject<Slider> spStartSubject = new Subject<Slider>();
    Subject<Slider> spEndSubject = new Subject<Slider>();
```

다음으로는 두 subject 객체에 대한 Observable 객체를 생성한다. 

``` csharp
    var spStartStream = spStartSubject.AsObservable();
    var spEndStream = spEndSubject.AsObservable();
```

스태미너의 회복은 spStartStream이 도착하면 회복을 시작한다. 회복 중에 spEndStream이 도착하면 이벤트 처리를 완료한다. 이렇게 이벤트 A가 올 때부터 이벤트 B가 올 때까지에 대한 처리는 아래의 RX 연산들의 조합으로 쉽게 처리할 수 있다. 

```
    SkipUnity + TakeUntil + Repeat
```

실제로 스트림을 생성하는 코드는 아래와 같다. 

``` csharp
    var recoveryStream = Observable.EveryUpdate()
        .SkipUntil(spStartStream) // spStartStream 이벤트 통지시까지 대기
        .Select( _ => slider.value )
        .TakeUntil(spEndStream)   // spEndStream 통지시까지,
        .RepeatUntilDisable(this) // 반복
        .Subscribe(v =>
            {
                // 스태미너를 delta만큼 회복
                slider.value += this.delta;

                // 스태미너 회복이 완료되면 spEndSubject 이벤트 통지로 스트림 완료.
                if (slider.value >= 1f)
                    spEndSubject.OnNext(slider); 
            });
```

MonoBehaviour의 Update 시마다 이벤트 통지를 검사하기 위해서 Observable.EveryUpdate()로 스트림을 생성한다. 

[UniRx](https://github.com/neuecc/UniRx)에서는 UnityEvent.AsObservable를 통해서 uGUI의 UnityEvent들에 대한 처리에 할 수 있다. 

Button의 경우 Button 객체의 OnClickAsObservable() 을 사용해서 해당 버튼을 클릭한 경우에 대한 이벤트를 처리한다.

``` csharp
    public Button actionButton;
    ...
    actionButton.OnClickAsObservable()
        .Subscribe(_ =>
        {
            // 버튼 클릭시 (액션 이벤트 발생시) 스태미너를 amount 만큼 감소.
            slider.value -= this.amount;

            // 현재 스태미너가 회복중인 경우 spEndSubject의 메세지를 보내서 현재 회복중(repeat)인 상태를 중지하도록 한다.
            spEndSubject.OnNext(slider);

            // 
            Observable.FromCoroutine(UpdateSlider)
                .Subscribe();            
        });

```

``` csharp

```