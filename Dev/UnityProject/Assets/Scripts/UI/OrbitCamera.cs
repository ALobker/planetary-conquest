﻿using UnityEngine;
using System.Collections;

/**
 * Change the camera into an orbital camera. An orbital is a camera
 * that can be rotated and that will automatically reorient itself to
 * always point to the target.
 * 
 * The orbit camera allow zooming and dezooming with the mouse wheel.
 * 
 * By clicking the mouse and dragging on the screen, the camera is moved. 
 * The angle of rotation  correspond to the distance the cursor travelled. 
 *  
 * The camera will keep the angular position when the button is pressed. To
 * rotate more, simply repress the mouse button et move the cursor.
 *
 * This script must be added on a camera object.
 *
 * @author Mentalogicus
 * @date 11-2011
 */
public class OrbitCamera : MonoBehaviour
{

    //The target of the camera. The camera will always point to this object.
    public Transform _target;

    //The default distance of the camera from the target.
    public float _distance = 20.0f;

    //Control the speed of zooming and dezooming.
    public float _zoomStep = 1.0f;

    //The speed of the camera. Control how fast the camera will rotate.
    public float _xSpeed = 1f;
    public float _ySpeed = 1f;

    //The position of the cursor on the screen. Used to rotate the camera.
    private float _x = 0.0f;
    private float _y = 0.0f;

    //Distance vector. 
    private Vector3 _distanceVector;

    /**
     * Move the camera to its initial position.
     */
    void Start()
    {
        _distanceVector = new Vector3(0.0f, 0.0f, -_distance);

        Vector2 angles = this.transform.localEulerAngles;
        _x = angles.x;
        _y = angles.y;

        this.Rotate(_x, _y);
    }

    /**
     * Rotate the camera or zoom depending on the input of the player.
     */
    void LateUpdate()
    {
        if (_target)
        {
            this.RotateControls();
            this.Zoom();
        }
    }

    /**
     * Rotate the camera when the first button of the mouse is pressed.
     * 
     */
    private void RotateControls()
    {
        if (Input.GetButton("Fire2"))
        {
            _x += Input.GetAxis("Mouse X") * _xSpeed;
            _y += -Input.GetAxis("Mouse Y") * _ySpeed;
            _y = Mathf.Clamp(_y, -90f, 90f);

            this.Rotate(_x, _y);
        }
    }

    /**
     * Transform the cursor mouvement in rotation and in a new position
     * for the camera.
     */
    private void Rotate(float x, float y)
    {
        //Transform angle in degree in quaternion form used by Unity for rotation.
        Quaternion rotation = Quaternion.Euler(y, x, 0.0f);

        //The new position is the target position + the distance vector of the camera
        //rotated at the specified angle.
        Vector3 position = rotation * _distanceVector + _target.position;

        //Update the rotation and position of the camera.
        transform.rotation = rotation;
        transform.position = position;
    }

    /**
     * Zoom or dezoom depending on the input of the mouse wheel.
     */
    private void Zoom()
    {
        if (Input.GetAxis("Mouse ScrollWheel") < 0.0f)
        {
            this.ZoomOut();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") > 0.0f)
        {
            this.ZoomIn();
        }
    }

    /**
     * Reduce the distance from the camera to the target and
     * update the position of the camera (with the Rotate function).
     */
    private void ZoomIn()
    {
        _distance -= _zoomStep;
        _distance = Mathf.Clamp(_distance, 2f, 5000f);
        _distanceVector = new Vector3(0.0f, 0.0f, -_distance);
        this.Rotate(_x, _y);
    }

    /**
     * Increase the distance from the camera to the target and
     * update the position of the camera (with the Rotate function).
     */
    private void ZoomOut()
    {
        _distance += _zoomStep;
        _distance = Mathf.Clamp(_distance, 2f, 5000f);
        _distanceVector = new Vector3(0.0f, 0.0f, -_distance);
        this.Rotate(_x, _y);
    }

    public void SetRotation(float x, float y)
    {
        _x = x;
        _y = y;

        if (_y < -180) _y += 360;
        if (_y > 180) _y -= 360;

        Rotate(_x, _y);
    }
}