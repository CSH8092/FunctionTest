using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCom : MonoBehaviour
{
    [Header("Camera")]
    [SerializeField] Camera camera_main;
    [SerializeField] Transform transform_mainCamera;

    [Header("Objects")]
    [SerializeField] GameObject object_target;
    [SerializeField] Transform transform_gizmo;
    [SerializeField] GameObject obejct_pivot = null;

    float mouse_scrollwheel;
    float mouse_horizontal;
    float mouse_vertical;

    Vector3 pivotPoint;
    Vector3 objectOriginPosition;
    Vector3 screenUpperRightPoint;

    bool isRotate = false;
    bool isPan = false;
    bool isZoom = false;

    void Start()
    {
        objectOriginPosition = object_target.transform.position;
        screenUpperRightPoint = new Vector3(Screen.width, Screen.height);

        SetPanSpeed();
    }

    void Update()
    {
        GetMouseEvent();

        CameraRotate();
        CameraPan();
        CameraZoom();
    }

    void GetMouseEvent()
    {
        mouse_horizontal = Input.GetAxis("Mouse X");
        mouse_vertical = Input.GetAxis("Mouse Y");
        mouse_scrollwheel = Input.GetAxisRaw("Mouse ScrollWheel") * zoomSpeed;
    }

    void ShowPivotPoint(bool isOn)
    {
        if (obejct_pivot == null)
            return;

        obejct_pivot.transform.position = pivotPoint;
        obejct_pivot.SetActive(isOn);
    }

    Vector3 GetScreenCenterPoint()
    {
        // 화면 Center Point 계산
        Vector3 screenCenter = camera_main.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        Vector3 worldCenter = camera_main.ScreenToWorldPoint(screenCenter);

        Vector3 dir = object_target.transform.position - transform_mainCamera.position;
        worldCenter += transform_mainCamera.forward * dir.magnitude;

        return worldCenter;
    }

    [Header("Rotate Option")]
    [SerializeField] float rotateSpeed = 10f;
    [SerializeField] float zoomSpeed = 10f;

    void CameraRotate()
    {
        if (Input.GetMouseButtonDown(1))
        {
            isRotate = true;

            pivotPoint = GetScreenCenterPoint();
            ShowPivotPoint(true);
        }
        else if (Input.GetMouseButton(1))
        {
            // x축 회전
            transform_mainCamera.RotateAround(pivotPoint, transform_mainCamera.right, mouse_vertical * -rotateSpeed);
            // y축 회전
            transform_mainCamera.RotateAround(pivotPoint, transform_mainCamera.up, mouse_horizontal * rotateSpeed);

            if (transform_gizmo != null)
            {
                Matrix4x4 camMtx = transform_mainCamera.localToWorldMatrix.inverse;
                transform_gizmo.localRotation = camMtx.rotation;
            }
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRotate = false;

            ShowPivotPoint(false);
        }
    }


    [Header("Pan Option")]
    [SerializeField] bool _isPerfectPanning = false;
    [SerializeField] float xPanSpeed = 10f;
    [SerializeField] float yPanSpeed = 10f;
    void CameraPan()
    {
        CameraPerfectPan();

        if (!_isPerfectPanning)
            CameraDefaultPan();
    }

    Vector3 a, b, c, d, e;
    Ray ray;
    RaycastHit hit;
    void CameraPerfectPan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            // 충돌 판정이 됬을 때만 해당
            a = transform_mainCamera.position;
            ray = camera_main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == 7) // Layer 7 : Model
                {
                    c = hit.point;
                    e = ray.origin + ray.direction * camera_main.farClipPlane;
                    isPan = true;
                    _isPerfectPanning = true;
                }
                else
                {
                    _isPerfectPanning = false;
                }
            }
            else
            {
                _isPerfectPanning = false;
            }
        }

        if (!_isPerfectPanning)
            return;

        else if (Input.GetMouseButton(2) && isPan)
        {
            ray = camera_main.ScreenPointToRay(Input.mousePosition);
            d = ray.origin + ray.direction * camera_main.farClipPlane;

            Vector3 dir = (e - d).normalized;

            float ac = (a - c).magnitude;
            float ed = (e - d).magnitude;
            float ec = (e - c).magnitude;

            float ab = ac * (ed / ec);

            b = a + dir * ab;
            transform_mainCamera.position = b;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isPan = false;

            _isPerfectPanning = false;
        }
    }

    Vector3 panStartPoint, panCurrentPoint;
    Vector3 originCamPos, prevCamPos;
    Vector3 prevY, prevX;
    void CameraDefaultPan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isPan = true;
            panStartPoint = Input.mousePosition;
            originCamPos = transform_mainCamera.position;
        }
        else if (Input.GetMouseButton(2))
        {
            panCurrentPoint = Input.mousePosition;

            // Cal Input Event to delta (0~1)
            float calX = (panCurrentPoint.x - panStartPoint.x) / camera_main.pixelWidth;
            float calY = (panCurrentPoint.y - panStartPoint.y) / camera_main.pixelHeight;
            Vector3 delta = new Vector3(calX * -xPanSpeed, calY * -yPanSpeed, 0);

            // Cal delta to move position
            Vector3 xResult = transform_mainCamera.right * delta.x;
            Vector3 yResult = transform_mainCamera.up * delta.y;
            Vector3 calPos = originCamPos + xResult + yResult;

            prevCamPos = transform_mainCamera.position;
            transform_mainCamera.position = calPos;

            // Check Area : https://forum.unity.com/threads/is-target-in-view-frustum.86136/
            Vector3 screenPoint = camera_main.WorldToScreenPoint(objectOriginPosition);
            if ((screenPoint.x < 0 || screenPoint.x > screenUpperRightPoint.x) && (screenPoint.y < 0 || screenPoint.y > screenUpperRightPoint.y))
            {
                transform_mainCamera.position = prevCamPos;
            }
            else
            {
                if (screenPoint.y < 0 || screenPoint.y > screenUpperRightPoint.y)
                {
                    // x좌표 move만 허용
                    transform_mainCamera.position = originCamPos + prevY + xResult;
                    prevX = xResult;
                }
                else if (screenPoint.x < 0 || screenPoint.x > screenUpperRightPoint.x)
                {
                    // y좌표 move만 허용
                    transform_mainCamera.position = originCamPos + prevX + yResult;
                    prevY = yResult;
                }
                else
                {
                    prevX = xResult;
                    prevY = yResult;
                }
            }
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isPan = false;
        }
    }

    [Header("Zoom Option")]
    [SerializeField] float ZOOM_MIN = 12f;
    [SerializeField] float ZOOM_MAX = 1f;

    void CameraZoom()
    {
        if (mouse_scrollwheel != 0)
        {
            isZoom = true;

            mouse_scrollwheel = Mathf.Clamp(mouse_scrollwheel, -1, 1);

            // Check Zoom In & Out
            Vector3 screenPoint = camera_main.WorldToScreenPoint(objectOriginPosition);
            // Range Over (Zoom In)
            if (screenPoint.z < ZOOM_MAX)
            {
                if (mouse_scrollwheel > 0)
                {
                    mouse_scrollwheel = 0;
                }
            }
            // Range Over (Zoom Out)
            if (screenPoint.z > ZOOM_MIN)
            {
                if (mouse_scrollwheel < 0)
                {
                    mouse_scrollwheel = 0;
                }
            }

            // Zoom With Panning
            Vector3 panPoint;
            ray = camera_main.ScreenPointToRay(Input.mousePosition);
            panPoint = ray.origin + ray.direction;
            float moveDistance = Vector3.Distance(panPoint, transform_mainCamera.position);
            // Notice! : ScreenPointToRay는 Camera의 Near Plane에서 시작한다 -> forward 방향 생각 안해도 Zoom기능 실행 가능
            Vector3 direction = Vector3.Normalize(panPoint - transform_mainCamera.position) * (moveDistance * mouse_scrollwheel);

            // Set Pan & Zoom Value
            transform_mainCamera.position += direction;
            // transform_camera.position += transform_camera.forward * mouse_scrollwheel;

            // Cal Pan Speed
            SetPanSpeed();
        }
        else
        {
            isZoom = false;
        }
    }

    void SetPanSpeed()
    {
        // TODO : 해당 방법은 Pan 에 따라서도 Speed가 변경되므로 수정 필요
        //Ray ray = camera_this.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        //Vector3 farPoint = ray.origin + ray.direction * camera_this.farClipPlane;
        float distance = Vector3.Distance(objectOriginPosition, transform_mainCamera.position);
        //xPanSpeed = (distance) + 5;
        //yPanSpeed = (xPanSpeed / 3) * 2;
    }

    // Testing Code
    /*
    bool CheckTargetSight(Vector3 currPos)
    {
        float angle = camera_this.fieldOfView;
        angle = angle / 2;
        Vector3 dir = object_target.transform.position - currPos; // transform_camera.position;

        float dot = Vector3.Dot(transform_camera.forward, dir.normalized);
        float result = Mathf.Acos(dot) * Mathf.Rad2Deg;

        if (result <= angle)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    void GetCorner()
    {
        // 4 Corners
        float depth = camera_this.transform.position.z * -1;

        Vector3 upperLeftScreen = new Vector3(0, camera_this.pixelHeight, depth);
        Vector3 upperRightScreen = new Vector3(camera_this.pixelWidth, camera_this.pixelHeight, depth);
        Vector3 lowerLeftScreen = new Vector3(depth, depth, depth);
        Vector3 lowerRightScreen = new Vector3(camera_this.pixelWidth, depth, depth);

        Vector3 upperLeft = camera_this.ScreenToWorldPoint(upperLeftScreen);
        Vector3 upperRight = camera_this.ScreenToWorldPoint(upperRightScreen);
        Vector3 lowerLeft = camera_this.ScreenToWorldPoint(lowerLeftScreen);
        Vector3 lowerRight = camera_this.ScreenToWorldPoint(lowerRightScreen);

        GameObject point1 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point1.transform.position = upperLeft;
        GameObject point2 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point2.transform.position = upperRight;
        GameObject point3 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point3.transform.position = lowerLeft;
        GameObject point4 = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        point4.transform.position = lowerRight;
    }
    */
}