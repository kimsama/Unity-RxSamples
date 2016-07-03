
UniRx를 이용한 스태미나 시스템의 구현
==========================================

이 예제는 [다크소울의 스태미나 시스템](http://darksouls.wiki.fextralife.com/Stamina)과 유사한 시스템 구현을 위해 필요한 RX의 연산들의 조합 방법과 사용자 정의 이벤트 처리 방법 대해서 소개하는 글로써 아래의 내용들을 포함한다. 

* uGUI 이벤트 처리
* Subject를 이용한 메시지 통지
* 하나 이상의 Rx 연산들의 조합(SkipUntil + TakeUntil + Repeat)


구현한 시스템은 다음과 같이 작동한다. 

1. *Action* 버튼(다크소울에서의 공격, 구르기 등)을 클릭하면 미리 정한 양(`amount`)만큼의 스태미나가 감소한다. 
2. 일정 시간(`delay`)이 지난 후 스태미나는 자동으로 회복된다.
3. 스태미나가 회복중에 다시 액션을 실행하는 경우 `2.`를 반복한다.


<p align="center">
  <img src="https://github.com/kimsama/Unity-RxSamples/blob/master/Assets/Samples/uGUI/stamina-system.gif?raw=true" alt="Stamina-System"/>
</p>


이를 RX에서의 이벤트 스트림의 관점으로 보면 아래와 같이 정리할 수 있다. 

1. 액션 이벤트(예제에서는 *Action* 버튼을 클릭한 경우)가 발생한 경우 스태미나를 감소한 후
2. `delay`만큼 대기 후에 감소 이벤트(= 스태미나 회복 시작)를 통지한다. 
3. 감소 이벤트를 통지하면 스태미나를 채우는 스트림(회복)을 반복한다. 
4. 스태미나를 회복하는 도중에 다시 액션 이벤트가 통지되면 기존의 반복을 중지(*중요!*)한 후 2. 3.을 반복한다.
5. 스태미나를 모두 회복하면 (*slider.value == 1*) 스태미나 감소 이벤트가 통지될 때까지 대기한다.

먼저 스태미나의 감소와 회복 완료에 대한 두 가지 이벤트 통지를 위한 메시지가 필요하다. RX에서 이와 같은 사용자 정의형 이벤트는 `Subject`를 사용해서 생성할 수 있다. 

``` csharp 
    Subject<Slider> spStartSubject = new Subject<Slider>();
    Subject<Slider> spEndSubject = new Subject<Slider>();
```

다음으로는 두 `Subject` 객체에 대한 `Observable` 객체를 생성한다. 

``` csharp
    var spStartStream = spStartSubject.AsObservable();
    var spEndStream = spEndSubject.AsObservable();
```

스태미나의 회복은 `spStartStream`이 도착하면 회복을 시작한다. 회복 중에 `spEndStream`이 도착하면 이벤트 처리를 완료한다. 이벤트 A가 올 때부터 이벤트 B가 올 때까지에 대한 처리는 아래 RX 연산 조합을 이용하면 쉽게 처리할 수 있다. 

`SkipUntil + TakeUntil + Repeat`

각각의 연산에 대해서는 아래 링크를 참조하자.
* [SkipUntil](http://reactivex.io/documentation/operators/skipuntil.html)
* [TakeUntil](http://reactivex.io/documentation/operators/takeuntil.html)
* [Repeat](http://reactivex.io/documentation/operators/repeat.html)


스태미나의 회복을 위한 `recorveryStream` 스트림을 생성하는 코드는 아래와 같다. 

``` csharp
    var recoveryStream = Observable.EveryUpdate()
        .SkipUntil(spStartStream) // spStartStream 이벤트 통지시까지 대기
        .Select( _ => slider.value )
        .TakeUntil(spEndStream)   // spEndStream 이벤트가 통지될 때까지,
        .RepeatUntilDisable(this) // 반복
        .Subscribe(v =>
            {
                // 스태미나를 delta만큼 회복
                slider.value += this.delta;

                // 스태미나 회복이 완료되면 spEndSubject 이벤트 통지로 스트림 완료.
                // -> 다시 spStartStream 통지를 대기하는 상태로 전이.
                if (slider.value >= 1f)
                    spEndSubject.OnNext(slider); 
            });
```

MonoBehaviour의 Update 시마다 이벤트 통지를 검사하기 위해서 `Observable.EveryUpdate()`로 스트림을 생성한다. 

반복은 Repeat 연산으로 처리가 가능한데, 무한루프에 빠지지 않도록 주의해야 한다. UniRx에서는 이를 위해서 `RepeatSafe`와 `RepeatUntilDestroy(gameObject/component)`, `RepeatUntilDisable(gameObject/component)`와 같은 메소드를 제공한다.([UniRx ReadMe](https://github.com/neuecc/UniRx/blob/master/README.md) 문서 참고) 여기에서는 컴포넌트가 disable 상태일 때에는 반복하지 않도록 RepeatUntilDisable 연산을 사용했다.

다음으로는 uGUI의 UI 객체들의 이벤트를 UniRx를 사용해서 처리하는 방법에 대해서 살펴 보자. 

[UniRx](https://github.com/neuecc/UniRx)에서는 `UnityEvent.AsObservable`를 통해서 uGUI의 UnityEvent들에 대한 처리에 할 수 있다. 

Button의 경우 Button 객체의 `OnClickAsObservable()` 을 사용해서 해당 버튼을 클릭한 경우에 대한 이벤트를 처리한다.

``` csharp
    public Button actionButton;
    ...
        IDisposable cancel = null;

        var buttonStream = actionButton.OnClickAsObservable()
            .Subscribe(_ =>
            {
                Debug.Log("button click.");

                // 회복 관련해서 이미 실행중인 코루틴이 있는 경우 실행을 종료.
                if (cancel != null)
                    cancel.Dispose(); 

                cancel = Observable.FromCoroutine(StartStaminaRecovery)
                                       .Subscribe();
            });

```

스태미나 소모의 시작을 `Observable.FromCoroutine`을 사용해서 코루틴 함수 `StartStaminaRecovery`를 통해서 통지한 것은 스태미나 소모가 일어난 직후 바로 `recorveryStream`에서 스태미나 회복을 시작하는 것이 아니라 `delay`만큼 대기한 후 `spStartSubject`로 스태미나 회복 시작 이벤트에 대한 통지 처리를 손쉽게 하기 위해서이다. 

```csharp
    IEnumerator StartStaminaRecovery()
    {
        //  스태미나를 amount 만큼 감소.
        slider.value -= this.amount;

        // 현재 스태미나가 회복중인 경우 spEndSubject의 메세지를 보내서 현재 회복중(repeat)인 상태를 중지하도록 한다.
        spEndSubject.OnNext(slider);

        // delay 시간만큼 대기.
        yield return new WaitForSeconds(this.delay);

        // 스태미너 회복 이벤트 통지
        spStartSubject.OnNext(slider);
    }    
```

실제 게임에 적용할 때에는 슬라이드의 값을 바로 변경하지 말고 플레이어 캐릭터의 스태미나 값을 변경하도록 하고 슬라이드는 플레이어 캐릭터의 스태미나 값과 `Binding`해 두어서 플레이어 캐릭터의 스태미나 값을 변경할 때마다 슬라이드의 값에도 변경된 값이 자동으로  반영되도록 처리하는 것이 좋다. UniRx에서는 이러한 처리를 위해 `ReactiveProperty`를 제공하고 있으니 이를 사용하는 것도 좋은 방법이다. 

또한 예제에서는 `Observable.EveryUpdate()`을 사용해서 매프레임마다 정해진 양만큼 스태미나를 회복하도록 처리하고 있지만 다크소울과 유사하게 처리하고자 한다면 레벨이나 관련 아이템의 착용 유무에 따라 정해진 초당 스테미나 증가량을 이용해서 회복하도록 처리해야 한다. 그러므로 프레임과 무관하게 주어진 시간에 스태미나가 회복할 수 있는 방법으로 변경해야 할 것이다. 바꾸어 말하면 프레임에 독립적인 방법을 고려해야 한다는 이야기이다. 

