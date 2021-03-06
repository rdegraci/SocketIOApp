﻿/*
 *
Copyright 2018 Rodney Degracia

MIT License:

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*
*/

using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using UnityEngine.Assertions;
using UnityEngine.XR.MagicLeap;



using Prestige;


/*
 * Controller Behavior should be attached to the gameObject that represents the Magic Leap Controller
 * 
 * This script works in conjunction with the Cursor script, which should be attached to the GameObject
 * which represents the Cursor (end of the picker "ray").
 * */

public class InputController : MonoBehaviour
{
    [SerializeField]
    Cursor cursor = null;

    public delegate void TriggerDown(byte controllerId, float value, GameObject gameObject, Transform cursorTransform);
    public delegate void TriggerUp(byte controllerId, float value, GameObject gameObject, Transform cursorTransform);

    public static event TriggerDown OnTriggerDown;
    public static event TriggerUp OnTriggerUp;

    public delegate void TouchpadGestureStart(byte controllerId,
        MLInputControllerTouchpadGesture gesture,
        Cursor cursor);

    public delegate void TouchpadGestureEnd(byte controllerId,
        MLInputControllerTouchpadGesture gesture,
        Cursor cursor);

    public static event TouchpadGestureEnd OnTouchpadGestureEnd;
    public static event TouchpadGestureStart OnTouchpadGestureStart;

    private Prestige.InputController inputController;

    protected Prestige.InputController GetInputController()
    {
        return inputController;
    }



    virtual public void Awake()
    {
        Assert.raiseExceptions = true;  // Debugging
        inputController = new Prestige.InputController(Prestige.DeviceType.ControllerFirst);
        inputController.Start();


    }

    virtual public void OnDestroy()
    {
        inputController.Stop();
    }

    virtual public void Update()
    {
        if (inputController != null)
        {
            MLInputController mlInputController = inputController.GetMLInputController();

            if (mlInputController != null)
            {
                transform.position = mlInputController.Position;
                transform.rotation = mlInputController.Orientation;
            }

        }
    }

    private void OnEnable()
    {
        if (inputController == null)
        {
            inputController = new Prestige.InputController(Prestige.DeviceType.ControllerFirst);
            inputController.Start();
        }

        inputController.RegisterTriggerUpHandler(OnTriggerUpHandler);
        inputController.RegisterTriggerDownHandler(OnTriggerDownHandler);

        inputController.RegisterTouchpadGestureStartHandler(OnTouchpadGestureStartHandler);
        inputController.RegisterTouchpadGestureEndHandler(OnTouchpadGestureEndHandler);
    }

    private void OnDisable()
    {
        if (inputController != null)
        {
            inputController.UnregisterTriggerUpHandler(OnTriggerUpHandler);
            inputController.UnregisterTriggerDownHandler(OnTriggerDownHandler);

            inputController.UnregisterTouchpadGestureStartHandler(OnTouchpadGestureStartHandler);
            inputController.UnregisterTouchpadGestureEndHandler(OnTouchpadGestureEndHandler);
        }

    }

    virtual public void OnTriggerDownHandler(byte controllerId, float triggerValue)
    {
        if (OnTriggerDown == null)
        {
            return;
        }

        GameObject hoveredGameObject = cursor.GetHoveredGameObject();
        Transform cursorTransform = cursor.GetAdjustedCursorTransform();

        if (hoveredGameObject == null)
        {
            OnTriggerDown(controllerId, triggerValue, null, cursorTransform);
        }
        else
        {
            OnTriggerDown(controllerId, triggerValue, hoveredGameObject, cursorTransform);
        }

    }

    virtual public void OnTriggerUpHandler(byte controllerId, float triggerValue)
    {
        if (OnTriggerUp == null)
        {
            return;
        }

        GameObject hoveredGameObject = cursor.GetHoveredGameObject();
        Transform cursorTransform = cursor.GetAdjustedCursorTransform();

        OnTriggerUp(controllerId, triggerValue, hoveredGameObject, cursorTransform);
    }

    virtual public void OnTouchpadGestureStartHandler(byte controllerId, MLInputControllerTouchpadGesture gesture)
    {
        if (OnTouchpadGestureStart != null)
        {
            OnTouchpadGestureStart(controllerId, gesture, cursor);
        }

    }

    virtual public void OnTouchpadGestureEndHandler(byte controllerId, MLInputControllerTouchpadGesture gesture)
    {
        if (OnTouchpadGestureEnd != null)
        {
            OnTouchpadGestureEnd(controllerId, gesture, cursor);
        }
    }
}

