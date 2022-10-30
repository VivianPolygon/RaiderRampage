using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.InputSystem.OnScreen;

//Script that controls the way the aiming cursor behaves
public class AimControls : MonoBehaviour
{
    //flag for if the cursor is moving
    private bool tracking;

    [Header("Cursor Move Speeds")]
    //speed along each axis that the cursor can move
    [SerializeField]
    private float horizontalAimSpeed;
    [SerializeField]
    private float verticalAimSpeed;

    [Header("Angle in Degrees the Gun Can Aim")]
    //range in degrees that the gun can aim, 0 is center, 
    //-90, 90 = 180 degrees infront of gun
    [SerializeField]
    private Vector2 horizontalAimRange;
    [SerializeField]
    private Vector2 verticalAimRange;
    

    [Header("Cursor and ControlStick")]
    [Header("vvv LD, Dont Change vvv")]
    //canvas for the cursor
    [SerializeField]
    Canvas cursorCanvas;
    //controlstick's rect transform
    [SerializeField]
    private RectTransform controlStickRect;
    [SerializeField]
    private OnScreenStick stickScript;


    //raycast for the cursor
    private RaycastHit cursorDetect;
    //vector3 used to caculate and limit gun rotation
    private Vector3 gunRotationEuler;

    [Header("CurveForMovement")]
    [SerializeField]
    private AnimationCurve aimSpeedCurve;


    private void Start()
    {
        AimGun();
    }

    private void Update()
    {
        if(tracking)
        {
            AimGun();
        }
    }

    private void AimGun()
    {
        float aimSpeedX = (Time.deltaTime * verticalAimSpeed) * (aimSpeedCurve.Evaluate(Mathf.Abs(-controlStickRect.anchoredPosition.y / stickScript.movementRange)) * Mathf.Sign(-controlStickRect.anchoredPosition.y));
        float aimSpeedY = (Time.deltaTime * horizontalAimSpeed) * (aimSpeedCurve.Evaluate(Mathf.Abs(controlStickRect.anchoredPosition.x / stickScript.movementRange)) * Mathf.Sign(controlStickRect.anchoredPosition.x));
        GunData.instance.gunModelBody.transform.Rotate(aimSpeedX, aimSpeedY, 0);

        gunRotationEuler = GunData.instance.gunModelBody.transform.localEulerAngles;

        gunRotationEuler.x = (gunRotationEuler.x > 180) ? gunRotationEuler.x - 360 : gunRotationEuler.x;
        gunRotationEuler.y = (gunRotationEuler.y > 180) ? gunRotationEuler.y - 360 : gunRotationEuler.y;

        gunRotationEuler.x = Mathf.Clamp(gunRotationEuler.x, -verticalAimRange.y, -verticalAimRange.x);
        gunRotationEuler.y = Mathf.Clamp(gunRotationEuler.y, horizontalAimRange.x, horizontalAimRange.y);
        gunRotationEuler.z = 0;

        GunData.instance.gunModelBody.transform.localRotation = Quaternion.Euler(gunRotationEuler);

        if (Physics.Raycast(GunData.instance.gunModelBody.transform.position, GunData.instance.gunModelBody.transform.forward, out cursorDetect, 100f))
        {
            cursorCanvas.transform.position = cursorDetect.point;

            cursorCanvas.transform.rotation = Quaternion.FromToRotation(Vector3.forward, cursorDetect.normal);
            cursorCanvas.transform.position += cursorCanvas.transform.forward;
            cursorCanvas.transform.rotation = Quaternion.Euler(0, cursorCanvas.transform.rotation.y, cursorCanvas.transform.rotation.z);

            GunData.instance.cursorPositon = cursorCanvas.transform.position;
        }
    }

    public void StartTracking()
    {
        tracking = true;
    }
    public void StopTracking()
    {
        tracking = false;
    }
}
