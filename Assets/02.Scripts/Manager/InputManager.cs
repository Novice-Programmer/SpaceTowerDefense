using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ETouchMode
{
    Touch,
    TowerBuilding,
    ObstacleBuilding
}

public class InputManager : MonoBehaviour
{
    Camera _mainCamera;
    public static InputManager Instance { set; get; }
    public static ETouchMode TouchMode = ETouchMode.Touch;
    public static Ghost BuildGhost = null;
    public static ObjectGame SelectObject = null;

    [Header("CameraMove")]
    [SerializeField] float _minX = 0;
    [SerializeField] float _maxX = 0;
    [SerializeField] float _minY = 0;
    [SerializeField] float _maxY = 0;
    [SerializeField] float _panSpeed = 10.0f;
    [SerializeField] float _panBorderThicknessX = 200.0f;
    [SerializeField] float _panBorderThicknessY = 150.0f;
    [SerializeField] float _touchBorderThicknessX = 300.0f;
    [SerializeField] float _touchBorderThicknessY = 250.0f;
    [SerializeField] float _touchPanMoveLength = 5.0f;
    [SerializeField] float _doubleTouchTime = 0.5f;

    Vector3 _touchPos = Vector3.zero;
    int _touchX = 0;
    int _touchY = 0;
    [SerializeField] bool _doubleTouchCheck = false;
    float _timeCheck = 0;

    private void Awake()
    {
        Instance = this;
        _mainCamera = GetComponent<Camera>();
    }

    void Update()
    {
        _timeCheck += Time.deltaTime;
        if (_doubleTouchCheck && _timeCheck >= _doubleTouchTime)
        {
            _doubleTouchCheck = false;
        }
        if (TouchMode == ETouchMode.TowerBuilding || TouchMode == ETouchMode.ObstacleBuilding)
        {
            BuildPointerCheck(TouchMode == ETouchMode.TowerBuilding);
        }
        else
        {
            NonePointerCheck();
        }
    }

    public void BuildPointerCheck(bool isTower)
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << LayerMask.NameToLayer("Tile");

        if (Physics.Raycast(ray, out RaycastHit hit, 100f, layerMask))
        {
            Vector3 position = hit.point;
            BuildGhost.transform.position = position;
            Node node = hit.transform.GetComponent<Node>();

            EFitType fitType = node._parentTile.Fits(node._pos, BuildGhost._demision);
            if (fitType != EFitType.OutOfBounds && fitType != EFitType.Overlaps)
            {
                Vector3 nodePosition = node._parentTile.NodeToPosition(node._pos, BuildGhost._demision, fitType);
                BuildGhost.transform.position = nodePosition;
                BuildGhost._fitType = fitType;
                BuildGhost._fitPos = nodePosition;
                BuildGhost.FitMaterialCheck(fitType);
                if (Input.GetMouseButtonUp(0))
                {
                    node._parentTile.Occupy(node._pos, BuildGhost._demision, fitType);
                    BuildGhost._parentTile = node._parentTile;
                    BuildGhost._gridPos = node._pos;
                    if (isTower)
                        GameManager.Instance.TowerBuild(BuildGhost);
                    else
                        GameManager.Instance.ObstacleBuild(BuildGhost);
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                Destroy(BuildGhost.gameObject);
                BuildGhost = null;
                TouchMode = ETouchMode.Touch;
                return;
            }
        }
        else if (Physics.Raycast(ray, out hit, 100f))
        {
            Vector3 position = hit.point;
            BuildGhost.transform.position = position;
            BuildGhost.NoneCheck();

            if (Input.GetMouseButtonUp(0))
            {
                BuildGhost.RotateObject(true);
            }
        }

        if (Input.mousePosition.x >= Screen.width - _panBorderThicknessX)
        {
            if (transform.position.x + _panSpeed * Time.deltaTime <= _maxX)
                transform.Translate(Vector3.right * _panSpeed * Time.deltaTime, Space.World);
            else
                transform.position = new Vector3(_maxX, transform.position.y, transform.position.z);
        }

        if (Input.mousePosition.x <= _panBorderThicknessX)
        {
            if (transform.position.x - _panSpeed * Time.deltaTime >= _minX)
                transform.Translate(Vector3.left * _panSpeed * Time.deltaTime, Space.World);
            else
                transform.position = new Vector3(_minX, transform.position.y, transform.position.z);
        }

        if (Input.mousePosition.y >= Screen.height - _panBorderThicknessY)
        {
            if (transform.position.z + _panSpeed * Time.deltaTime <= _maxY)
                transform.Translate(Vector3.forward * _panSpeed * Time.deltaTime, Space.World);
            else
                transform.position = new Vector3(transform.position.x, transform.position.y, _maxY);
        }
        if (Input.mousePosition.y <= _panBorderThicknessY)
        {
            if (transform.position.z - _panSpeed * Time.deltaTime >= _minY)
                transform.Translate(Vector3.back * _panSpeed * Time.deltaTime, Space.World);
            else
                transform.position = new Vector3(transform.position.x, transform.position.y, _minY);
        }
    }

    public void NonePointerCheck()
    {
        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        int selectLayerMask = 1 << LayerMask.NameToLayer("Tower") | 1 << LayerMask.NameToLayer("Obstacle") | 1 << LayerMask.NameToLayer("PassInfo");
        int mineralLayerMask = 1 << LayerMask.NameToLayer("Mineral");
        if (Input.GetMouseButtonUp(0))
        {
            if (SelectObject != null)
            {
                SelectObject.Select(false);
                SelectObject = null;
            }
        }

        if (Physics.Raycast(ray, out hit, 50f, mineralLayerMask))
        {
            if (Input.GetMouseButtonUp(0))
            {
                DropMineral mineral = hit.transform.GetComponent<DropMineral>();
                mineral.GetMineral();
            }
        }

        else if (Physics.Raycast(ray, out hit, 50f, selectLayerMask))
        {
            if (Input.GetMouseButtonUp(0))
            {
                ObjectGame objectGame = hit.transform.GetComponent<ObjectGame>();
                if (objectGame._objectSelectActive)
                {
                    if (SelectObject != null && SelectObject != objectGame)
                    {
                        SelectObject.Select(false);
                    }
                    objectGame.Select();
                    SelectObject = objectGame;
                }
            }
        }

        else
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (Input.mousePosition.x >= Screen.width - _touchBorderThicknessX)
                {
                    _touchX = 1;
                }
                else if (Input.mousePosition.x <= _touchBorderThicknessX)
                {
                    _touchX = -1;
                }
                else
                {
                    _touchX = 0;
                }
                if (Input.mousePosition.y >= Screen.height - _touchBorderThicknessY)
                {
                    _touchY = 1;
                }
                else if (Input.mousePosition.y <= _touchBorderThicknessY)
                {
                    _touchY = -1;
                }
                else
                {
                    _touchY = 0;
                }

                if (_doubleTouchCheck)
                {
                    Vector2 prevTouch = new Vector2(_touchPos.x, _touchPos.y);
                    Vector2 nowTouch = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                    _doubleTouchCheck = false;
                    if (Vector2.Distance(prevTouch, nowTouch) < 10.0f)
                    {
                        transform.Translate((Vector3.forward * _touchY + Vector3.right * _touchX) * _touchPanMoveLength, Space.World);
                        if (transform.position.x > _maxX)
                            transform.position = new Vector3(_maxX, transform.position.y, transform.position.z);
                        if (transform.position.x < _minX)
                            transform.position = new Vector3(_minX, transform.position.y, transform.position.z);
                        if (transform.position.z > _maxY)
                            transform.position = new Vector3(transform.position.x, transform.position.y, _maxY);
                        if (transform.position.z < _minY)
                            transform.position = new Vector3(transform.position.x, transform.position.y, _minY);
                        return;
                    }
                }

                _touchPos = Input.mousePosition;
                _doubleTouchCheck = _touchX != 0 || _touchY != 0;

                if (_doubleTouchCheck)
                    _timeCheck = 0;
            }
        }
    }

    public void Install(EObjectType objectType, EObjectName objectName, int installCost)
    {
        if (objectType == EObjectType.Tower)
            TouchMode = ETouchMode.TowerBuilding;
        else if (objectType == EObjectType.Obstacle)
            TouchMode = ETouchMode.ObstacleBuilding;
        Ghost ghost = Instantiate(ObjectDataManager.Instance.GetBuildGhost(objectName));
        ghost._installCost = installCost;
        BuildGhost = ghost;
    }

    public void ObjectSelectClose()
    {
        if (SelectObject != null)
        {
            SelectObject.Select(false);
            SelectObject = null;
        }
    }

    public void UITouch()
    {
        _doubleTouchCheck = false;
        _timeCheck = 0;
    }
}
