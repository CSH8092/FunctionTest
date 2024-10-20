using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCom : MonoBehaviour
{
    // Components
    public GameObject object_gizmoCam;
    public GameObject object_target;
    GameObject obejct_pivot = null;

    Camera camera_this;
    Transform transform_camera;
    SphereCollider collider_camera;
    public Transform transform_gizmo;

    // Values
    float mouse_scrollwheel;
    float mouse_horizontal;
    float mouse_vertical;

    float rotateSpeed = 10f;
    float zoomSpeed = 10f;

    Vector3 pivotPoint;
    Vector3 objectOriginPosition;
    Vector3 screenUpperRightPoint;

    // flags
    bool isRotate = false;
    bool isPan = false;
    bool isZoom = false;
    bool isHitCam = false;

    void Start()
    {
        camera_this = this.GetComponent<Camera>();
        transform_camera = this.GetComponent<Transform>();
        collider_camera = this.GetComponent<SphereCollider>();

        objectOriginPosition = object_target.transform.position;
        screenUpperRightPoint = new Vector3(Screen.width, Screen.height);

        object_gizmoCam.SetActive(true);

        SetPanSpeed();
    }

    void Update()
    {
        GetMouseEvent();

        CameraRotate();
        CameraPerfectPan();
        //CameraPan();
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
        {
            obejct_pivot = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            obejct_pivot.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        }
        obejct_pivot.transform.position = pivotPoint;
        obejct_pivot.SetActive(isOn);
    }

    Vector3 GetScreenCenterPoint()
    {
        // 화면 Center Point 계산
        Vector3 screenCenter = camera_this.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        Vector3 worldCenter = camera_this.ScreenToWorldPoint(screenCenter);

        Vector3 dir = object_target.transform.position - transform_camera.position;
        worldCenter += transform_camera.forward * dir.magnitude;

        return worldCenter;
    }

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
            transform_camera.RotateAround(pivotPoint, transform_camera.right, mouse_vertical * -rotateSpeed);
            // y축 회전
            transform_camera.RotateAround(pivotPoint, transform_camera.up, mouse_horizontal * rotateSpeed);

            Matrix4x4 camMtx = transform_camera.localToWorldMatrix.inverse;
            transform_gizmo.localRotation = camMtx.rotation;
        }
        else if (Input.GetMouseButtonUp(1))
        {
            isRotate = false;

            ShowPivotPoint(false);
        }
    }

    Vector3 a, b, c, d, e;
    Ray ray;
    RaycastHit hit;

    void CameraPerfectPan()
    {
        // Perpect Panning : https://prideout.net/blog/perfect_panning/

        if (Input.GetMouseButtonDown(2))
        {
            // 충돌 판정이 됬을 때만 해당 -> map 같은 오브젝트 옮길 때 유용할듯?
            a = transform_camera.position;
            ray = camera_this.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.gameObject.layer == 7) // Layer 7 : Model
                {
                    c = hit.point;
                    isPan = true;
                }

                e = ray.origin + ray.direction * camera_this.farClipPlane;
            }

            //Debug.DrawLine(a, c, Color.red, 1000000);
            //Debug.DrawLine(c, e, Color.magenta, 1000000);
        }
        else if (Input.GetMouseButton(2) && isPan)
        {
            ray = camera_this.ScreenPointToRay(Input.mousePosition);
            d = ray.origin + ray.direction * camera_this.farClipPlane;

            Vector3 dir = (e - d).normalized;

            float ac = (a - c).magnitude;
            float ed = (e - d).magnitude;
            float ec = (e - c).magnitude;

            float ab = ac * (ed / ec);

            b = a + dir * ab;
            transform_camera.position = b;
        }
        else if (Input.GetMouseButtonUp(2))
        {
            isPan = false;

            //Debug.DrawLine(b, d, Color.yellow, 1000000);
            //Debug.DrawLine(c, d, Color.blue, 1000000);
        }
    }


    Vector3 panStartPoint, panCurrentPoint;
    Vector3 originCamPos, prevCamPos;
    Vector3 prevY, prevX;
    [SerializeField]
    float xPanSpeed = 10f;
    [SerializeField]
    float yPanSpeed = 10f;
    void CameraPan()
    {
        if (Input.GetMouseButtonDown(2))
        {
            isPan = true;
            panStartPoint = Input.mousePosition;
            originCamPos = transform_camera.position;
        }
        else if (Input.GetMouseButton(2))
        {
            panCurrentPoint = Input.mousePosition;

            // Cal Input Event to delta (0~1)
            float calX = (panCurrentPoint.x - panStartPoint.x) / camera_this.pixelWidth;
            float calY = (panCurrentPoint.y - panStartPoint.y) / camera_this.pixelHeight;
            Vector3 delta = new Vector3(calX * -xPanSpeed, calY * -yPanSpeed, 0);

            // Cal delta to move position
            Vector3 xResult = transform_camera.right * delta.x;
            Vector3 yResult = transform_camera.up * delta.y;
            Vector3 calPos = originCamPos + xResult + yResult;

            prevCamPos = transform_camera.position;
            transform_camera.position = calPos;

            // Check Area : https://forum.unity.com/threads/is-target-in-view-frustum.86136/
            Vector3 screenPoint = camera_this.WorldToScreenPoint(objectOriginPosition);
            if ((screenPoint.x < 0 || screenPoint.x > screenUpperRightPoint.x) && (screenPoint.y < 0 || screenPoint.y > screenUpperRightPoint.y))
            {
                transform_camera.position = prevCamPos;
            }
            else
            {
                if (screenPoint.y < 0 || screenPoint.y > screenUpperRightPoint.y)
                {
                    // x좌표 move만 허용
                    transform_camera.position = originCamPos + prevY + xResult;
                    prevX = xResult;
                }
                else if (screenPoint.x < 0 || screenPoint.x > screenUpperRightPoint.x)
                {
                    // y좌표 move만 허용
                    transform_camera.position = originCamPos + prevX + yResult;
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

    void CameraZoom()
    {
        if (mouse_scrollwheel != 0)
        {
            isZoom = true;

            mouse_scrollwheel = Mathf.Clamp(mouse_scrollwheel, -1, 1);

            // Check Zoom In & Out
            Vector3 screenPoint = camera_this.WorldToScreenPoint(objectOriginPosition);
            // Range Over (Zoom In)
            if (screenPoint.z < 2)
            {
                if (mouse_scrollwheel > 0)
                {
                    mouse_scrollwheel = 0;
                }
            }
            // Range Over (Zoom Out)
            if (screenPoint.z > 10)
            {
                if (mouse_scrollwheel < 0)
                {
                    mouse_scrollwheel = 0;
                }
            }

            // Zoom With Panning
            Vector3 panPoint;
            ray = camera_this.ScreenPointToRay(Input.mousePosition);
            panPoint = ray.origin + ray.direction;
            float moveDistance = Vector3.Distance(panPoint, transform_camera.position);
            // Notice! : ScreenPointToRay는 Camera의 Near Plane에서 시작한다 -> forward 방향 생각 안해도 Zoom기능 실행 가능
            Vector3 direction = Vector3.Normalize(panPoint - transform_camera.position) * (moveDistance * mouse_scrollwheel);

            // Set Pan & Zoom Value
            transform_camera.position += direction;
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
        float distance = Vector3.Distance(objectOriginPosition, transform_camera.position);
        //xPanSpeed = (distance) + 5;
        //yPanSpeed = (xPanSpeed / 3) * 2;
    }

    private void OnTriggerEnter(Collider other)
    {
        isHitCam = true;
    }

    private void OnTriggerExit(Collider other)
    {
        isHitCam = false;
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