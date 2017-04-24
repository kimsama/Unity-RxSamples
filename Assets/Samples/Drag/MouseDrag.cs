using UnityEngine;
using System.Collections;
using UniRx;
using UniRx.Triggers;

/// <summary>
/// Rotate the gameobject with mouse dragging.
/// </summary>
public class MouseDrag : MonoBehaviour 
{
    public float rotationSpeed = 500.0f;

	void Start () 
    {

        this.OnMouseDownAsObservable()
            .SelectMany(_ => this.UpdateAsObservable())
            .TakeUntil(this.OnMouseUpAsObservable())     // Until the mouse is released
            .Select(_ =>                                 // get the amount of mouse movement.
                new Vector2(Input.GetAxis("Mouse X"),Input.GetAxis("Mouse Y")))
            //.Repeat()                                 // Repeat causes infinite repeat subscribe at GameObject 
                                                        // was destroyed. which leads, if in UnityEditor, Editor goes to freeze.
            .RepeatUntilDestroy(this)                   // Since the stream is completed TakeUntil re Subscribe
            .Subscribe(move =>
            {
                this.transform.rotation =
                    Quaternion.AngleAxis(move.y * rotationSpeed * Time.deltaTime, Vector3.right) *
                        Quaternion.AngleAxis(-move.x * rotationSpeed * Time.deltaTime, Vector3.up) *
                        transform.rotation;
                ;
            });

        /* Same as the above but no freezing even with calling Repeat().
        
        this.UpdateAsObservable()
            .SkipUntil(this.OnMouseDownAsObservable())
            .Select(_ =>                                 // get the amount of mouse movement.
                new Vector2(Input.GetAxis("Mouse X"),
                            Input.GetAxis("Mouse Y")))
            .TakeUntil(this.OnMouseUpAsObservable())     // Until the mouse is released
            .Repeat()                                    // Since the stream is completed TakeUntil re Subscribe
            .Subscribe(move =>
            {
                this.transform.rotation =
                    Quaternion.AngleAxis(move.y * rotationSpeed * Time.deltaTime, Vector3.right) *
                        Quaternion.AngleAxis(-move.x * rotationSpeed * Time.deltaTime, Vector3.up) *
                        transform.rotation;
                ;
            });
        // ObservableTriggers call OnCompleted when the GameObject they are attached to is destroyed.
        // So no need to call AddTo(this) here
         */ 
	}
}
