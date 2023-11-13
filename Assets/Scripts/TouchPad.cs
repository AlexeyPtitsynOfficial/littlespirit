using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;
using System;

public class TouchPad : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    private bool touched;
    private bool firstTouch;
    private int pointerID;

    void Awake()
    {
        touched = false;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!touched)
        {
            touched = true;
            pointerID = eventData.pointerId;
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.pointerId == pointerID)
        {
            touched = false;
        }
    }

    public int GetTouchCount()
    {
        return Input.touchCount;
    }
    public bool isTouched()
    {
        return touched;
    }

    public void setFirstTouch(bool state)
    {
        firstTouch = false;
    }

    public bool isFirstTouch()
    {
        return firstTouch;
    }

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Application.platform != RuntimePlatform.Android)
        {
            // use the input stuff
            if (Input.GetMouseButton(0));
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.mousePosition)), Vector2.zero);
                if (hit.collider != null)
                {
                    if (touched == false)
                        firstTouch = true;

                    touched = true;
                    Debug.Log("Touched it");
                }
        }
        else
        {
            if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint((Input.GetTouch(0).position)), Vector2.zero);
                if (hit.collider != null)
                {
                    if (touched == false)
                        firstTouch = true;

                    touched = true;
                    Debug.Log("Touched it");
                }
            }
            else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended)
            {
                touched = false;
            }
        }
    }
}
