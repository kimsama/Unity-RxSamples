# uGUI Drag

A sample which demonstrates dragging uGUI object with UniRx. It uses various Rx's opearators so it can be confused at first look but simple if you understand what *'ObservableEventTrigger'*, *'TakeUntil'* and *'TakeWhile'* are.

## ObservableEventTrigger

*'ObservableEventTrigger'* can help a lot with UI programming. It converts uGUI events to Obseravble.

``` csharp
using UniRx.Triggers;
...
var  eventTrigger  =  this.gameObject.AddComponent <ObservableEventTrigger>();
// PointerDown
eventTrigger.OnPointerDownAsObservable ()
            .Subscribe ( pointerEventData  =>  Debug.Log (pointerEventData.position));
// Drag
eventTrigger.OnDragAsObservable ()
            .Subscribe (pointerEventData  =>  Debug.Log (pointerEventData.position));
```

So you can easily handle various UI events, actually what Canvas deals with, as Observable.


See [UniRx.Triggers wiki page](https://github.com/neuecc/UniRx/wiki/UniRx.Triggers#observableeventtrigger) for other trigger operators.

## Timer

Before looking *TakeUntil* and *TakeWhile* opearators, let's look *'Timer'* first to make it much easily to understand other code samples.

``` csharp
public  class  TimerSample  :  MonoBehaviour  {
    void  Start  ()  {
        // After five seconds, it start to subscribes an event.
        Observable . Timer (TimeSpan.FromSeconds (5)). Subscribe ( _  =>
        {
            //  Change color of the sprite to be blue.
            GetComponent <SpriteRenderer>().color  =  Color.blue ;
        }). AddTo ( this );
    }
}
```

The above code streams OnNext event after five seconds. Note *'Timer'* is one-shot operator. It does not stream again after it streams OnNext once.

## TakeUntil

In the following code, *'TakeUntil'* does not stream OnNext event until there is left mouse button click down event.

``` csharp
TimeSpan dueTime = TimeSpan.FromSeconds (1);
TimeSpan period = TimeSpan.FromSeconds (1);
Observable.Timer (dueTime, period)
    .TakeUntil (this.UpdateAsObservable () .Where (_ => Input.GetMouseButton (0)))
    .Subscribe (x => Debug.Log (x), () => Debug.Log ( "OnComplete"));
```

In the above code, there are actually two streams.

1) Observable.Timer:  stream-A
2) this.UpdateAsObservable: stream-B

*'TakeUntil'* of stream-A get OnNext message only when there is OnNext message of stream-B.

> *'Timer'* has two arguments, first is for due time which means it starts after one second and the second is for a period. So it starts to get any event after one second and only get event during one second.

## TakeWhile

With *'TakeWhile'*, it only get event in the stream it takes true, ohterwise it streams OnComplete message.

``` csharp
TimeSpan dueTime = TimeSpan.FromSeconds (1);
TimeSpan period = TimeSpan.FromSeconds (1);
Observable.Timer (dueTime, period)
            .TakeWhile (count => count <2)
            .Subscribe (x => Debug.Log (x), () => Debug.Log ( "OnComplete"));
```

So the above code put a time only one then put "end" log for its OnComplete

## Take

The last, although the drag sample does not use it, there is *'Take'* operator which only accepts with the given amount of event as its argument.

``` csharp
TimeSpan dueTime = TimeSpan.FromSeconds (1);
TimeSpan period = TimeSpan.FromSeconds (1);
Observable.Timer (dueTime, period)
           .Take (2)
           .Subscribe (x => Debug.Log (x), () => Debug.Log ( "OnComplete"));
```

It takes two OnNext then get OnComplete on its stream.
