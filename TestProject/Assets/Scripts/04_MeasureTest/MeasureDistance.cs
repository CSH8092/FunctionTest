using TMPro;
using UnityEngine;

public class MeasureDistance : MonoBehaviour
{
    const float TEMP_CORE_OFFSET = 1;
    const float TEMP_START_RAY_POINT = 10f; // 가상의 Point로 부터 Ray를 쏠 때 필요한 Offset
    const float MAX_DISTANCE = 5000; // ray cast max distance
    const string DISTANCE_FORMAT = "{0:0.0}cm";

    float _distance_line = 0;
    float _temp_OffsetAngle = 2f;
    float _temp_offsetFront = 0.001f;

    [Header("Elements")]
    [SerializeField] GameObject object_left;
    [SerializeField] GameObject object_right;
    [SerializeField] LineRenderer line_distance;
    [SerializeField] Transform transform_distance;
    [SerializeField] TextMeshPro text_distance;

    // values
    [SerializeField] Vector3 vector3_LeftPos; // todo : refactor, 항상 object_left 반환할 수 있도록
    [SerializeField] Vector3 vector3_RightPos;

    // need gas values
    [SerializeField] public int hitTriangle_left { get; set; }
    [SerializeField] public int hitTriangle_right { get; set; }
    public Transform transform_left { get { return object_left.transform; } }
    public Transform transform_right { get { return object_right.transform; } }

    RaycastHit _hitSkin;
    Vector3 _hitSkinPoint;
    int _layerMask;
    int _clickCount;
    GameObject _objectTargetSphere;

    // flags
    bool _flagFirst = false;
    bool _flagSecond = false;
    bool _isClickLeft = false;

    // status
    [SerializeField]
    bool _isCreated = false;
    bool _isCanDrag = false;

    void Awake()
    {
        // Init Setting
        bool isOn = false;
        object_left.SetActive(isOn);
        object_right.SetActive(isOn);
        line_distance.gameObject.SetActive(isOn);
        transform_distance.gameObject.SetActive(isOn);

        _clickCount = 0;
        _layerMask = (1 << LayerMask.NameToLayer("Model"));
    }

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            LeftClickMeasureEvent();
        }
        else if (Input.GetMouseButton(0))
        {
            LeftDragMeasureEvent();
        }
    }

    bool GetCursorRayModel()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, MAX_DISTANCE, _layerMask))
        {
            _hitSkin = hit;
            _hitSkinPoint = hit.point;
            return true;
        }
        else
        {
            return false;
        }
    }

    bool GetCursorRayThisSphere()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out var hit, MAX_DISTANCE))
        {
            if (hit.transform.gameObject.Equals(object_left))
            {
                _objectTargetSphere = object_left;
                _isClickLeft = true;
                return true;
            }
            else if (hit.transform.gameObject.Equals(object_right))
            {
                _objectTargetSphere = object_right;
                _isClickLeft = false;
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    void LeftDragMeasureEvent()
    {
        if (_isCanDrag)
        {
            if (_objectTargetSphere == null)
            {
                return;
            }

            ModifiyMeasure(_objectTargetSphere);
        }
    }

    void LeftClickMeasureEvent()
    {
        if (!_isCreated)
        {
            // is Created false : Create Measure Event
            if (GetCursorRayModel())
            {
                CreateMeasure();
            }
            else
            {
                if (_clickCount == 0)
                {
                    MeasureDistanceMode.Instance.EndMeasureDistanceMode();
                    DestroyMeasure();
                }
                else
                {
                    CreateMeasure();
                }
            }
        }
        else
        {
            // is Created true : Check Measure Modify Event
            if (GetCursorRayThisSphere())
            {
                _isCanDrag = true;
            }
            else
            {
                _isCanDrag = false;
            }
        }
    }

    void ModifiyMeasure(GameObject targetSphere)
    {
        if (!GetCursorRayModel())
        {
            return;
        }

        targetSphere.gameObject.transform.position = _hitSkinPoint;

        if (_isClickLeft)
        {
            vector3_LeftPos = _hitSkinPoint;
            hitTriangle_left = _hitSkin.triangleIndex;
        }
        else
        {
            vector3_RightPos = _hitSkinPoint;
            hitTriangle_right = _hitSkin.triangleIndex;
        }

        SetIineRender(vector3_LeftPos, vector3_RightPos);
    }

    void CreateMeasure()
    {
        if (_clickCount == 0)
        {
            // Set Position
            vector3_LeftPos = _hitSkinPoint;
            hitTriangle_left = _hitSkin.triangleIndex;
            object_left.gameObject.transform.position = vector3_LeftPos;

            // Set Object Active
            object_left.SetActive(true);
            object_right.SetActive(false);
            line_distance.gameObject.SetActive(false);
            transform_distance.gameObject.SetActive(false);
            SetDistanceText(0, object_left.transform.position);

            // Set Flag
            _flagFirst = true;
        }
        else if (_clickCount == 1)
        {
            // Set Position
            vector3_RightPos = _hitSkinPoint;
            hitTriangle_right = _hitSkin.triangleIndex;
            object_right.gameObject.transform.position = vector3_RightPos;

            // Set Object Active
            object_left.SetActive(true);
            object_right.SetActive(true);
            line_distance.gameObject.SetActive(true);
            transform_distance.gameObject.SetActive(true);

            // Set Object Active
            SetIineRender(vector3_LeftPos, vector3_RightPos);

            // Set Flag
            _flagSecond = true;
            _isCreated = true;
            MeasureDistanceMode.Instance.AddMeasureList(this);

            // End Mode
            MeasureDistanceMode.Instance.EndMeasureDistanceMode();
        }

        _clickCount++;
    }

    void SetIineRender(Vector3 left, Vector3 right, bool checkForward = false)
    {
        // 설정된 두 지점을 기준으로, 가상의 중심 Point를 향해 일정 거리 밖에서 Ray를 쏴서 Line을 그리는 개념
        Vector3 temp_lineCenter = new Vector3(0, 0, 0);

        Vector3 forward = Camera.main.transform.forward.normalized;
        if (checkForward)
        {
            forward = Vector3.forward;
        }

        temp_lineCenter = (left + right) * 0.5f + (forward * 2f);

        Vector3 dirLeft = (left - temp_lineCenter).normalized;
        Vector3 dirRight = (right - temp_lineCenter).normalized;
        float angleByTwoPoint = Vector3.Angle(dirLeft, dirRight);

        Debug.DrawRay(temp_lineCenter, dirLeft * 10f, Color.red, 20);
        Debug.DrawRay(temp_lineCenter, dirRight * 10f, Color.yellow, 20);

        // line Render Init
        LineRenderer tempLineRender = line_distance;
        _distance_line = 0;
        int index = -1;
        tempLineRender.positionCount = 0;

        for (int i = 0; i < angleByTwoPoint; i++)
        {
            float ratio = i / angleByTwoPoint;
            Vector3 tmp = Vector3.Lerp(left, right, ratio);
            Vector3 dir = (temp_lineCenter - tmp).normalized;
            Ray ray = new Ray(tmp - dir * TEMP_START_RAY_POINT, dir);

            Debug.DrawRay(tmp - dir * TEMP_START_RAY_POINT, dir * 10f, Color.green, 10);

            if (Physics.Raycast(ray, out var hit, MAX_DISTANCE, _layerMask))
            {
                index = tempLineRender.positionCount++;

                Vector3 hitPoint = hit.point - _temp_offsetFront * ray.direction;
                tempLineRender.SetPosition(index, hitPoint);

                if (index != 0)
                {
                    _distance_line += Vector3.Distance(hitPoint, tempLineRender.GetPosition(index - 1));
                }
            }
        }

        if (tempLineRender.positionCount > 0)
        {
            Vector3 textPosition = tempLineRender.GetPosition(tempLineRender.positionCount / 2);
            _distance_line *= TEMP_CORE_OFFSET;
            SetDistanceText(_distance_line, textPosition);
        }
    }

    void SetDistanceText(float distance, Vector3 position)
    {
        // Print Distance Text
        string str_distance = string.Format(DISTANCE_FORMAT, distance);
        text_distance.text = str_distance;
        transform_distance.transform.position = position;
    }

    public void DestroyMeasure()
    {
        if (_isCreated)
        {
            MeasureDistanceMode.Instance.RemoveMeasureList(this);
        }

        Destroy(this.gameObject);
    }
}
