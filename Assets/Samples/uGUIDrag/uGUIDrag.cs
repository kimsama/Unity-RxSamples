using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UniRx;
using UniRx.Triggers;
using Extensions;

/// <summary>
/// A class enables to drag uGUI component.
/// </summary>
public class uGUIDrag : MonoBehaviour
{
    public RectTransform target;
    public bool horizontal = true;
    public bool vertical = true;

    public bool interactable
    {
        get { return this.graphic.raycastTarget; }
        set { this.graphic.raycastTarget = value; }
    }

    private Canvas _canvas;

    public Canvas canvas
    {
        get
        {
            if (_canvas == null)
            {
                _canvas = this.GetComponentInParent<Canvas>();
            }
            return _canvas;
        }
        set { _canvas = value; }
    }

    private Graphic _graphic;

    private Graphic graphic
    {
        get { return _graphic ?? (_graphic = this.GetComponent<Graphic>()); }
    }

    private ObservableEventTrigger _trigger;

    private ObservableEventTrigger trigger
    {
        get { return _trigger ?? (_trigger = this.gameObject.GetOrAddComponent<ObservableEventTrigger>()); }
    }

    void Awake()
    {
        if (this.graphic == null)
        {
            Debug.LogWarning("Graphic component is required.");
            this.gameObject.AddComponent<Image>().color = Color.clear;
        }

        if (this.target == null)
        {
            this.target = this.GetComponent<RectTransform>();
        }

        this.trigger
            .OnBeginDragAsObservable()
            .Where(e => this.interactable && this.target)
            .SelectMany(this.trigger.OnDragAsObservable())
            .TakeUntil(this.trigger.OnEndDragAsObservable())
            .TakeWhile(e => this.interactable && this.target)
            .Select(e => GetPosition(e))
            .Pairwise()  // NOTE : Buffer (2,1)이라고 마지막에 쌍 아닌 값이 올 
            .RepeatUntilDestroy(this)
            .Subscribe(OnDrag)
            .AddTo(this);

    }

    private Vector3 GetPosition(PointerEventData eventData)
    {
        var screenPosition = eventData.position;
        var result = Vector3.zero;

        switch (this.canvas.renderMode)
        {
            case RenderMode.ScreenSpaceOverlay:
            case RenderMode.ScreenSpaceCamera:
                RectTransformUtility.ScreenPointToWorldPointInRectangle(
                    this.target,
                    screenPosition,
                    eventData.pressEventCamera,
                    out result);
                break;
            case RenderMode.WorldSpace:
            // TODO : WorldSpace의 Canvas 대응 
            Debug.LogWarning("not supported RenderMode.WorldSpace.");
                break;
        }

        return result;
    }

    private void OnDrag(Pair<Vector3> positions)
    {
        var deltaX = this.horizontal ? positions.Current.x - positions.Previous.x : 0;
        var deltaY = this.vertical ? positions.Current.y - positions.Previous.y : 0;
        this.target.position += new Vector3(deltaX, deltaY, 0);
    }
}