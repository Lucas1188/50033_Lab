using UnityEngine;
#nullable enable
public class MainCamera : MonoBehaviour
{
    private Camera _camera;
    public CameraBounds boundsAsset;
    void Awake()
    {   //Ensure MainCamera is executing in default time
        if (!GameManager.Instance) return;
        GameManager.Instance.OnControllableCharacterChanged += ChangedControllable;
    }
    void OnDestroy()
    {
        if (!GameManager.Instance) return;
        GameManager.Instance.OnControllableCharacterChanged -= ChangedControllable;       
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GetCamera();
    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (mainControllable == null) return;
        if (_camera == null)
        {
            //Praying now
            if (!GetCamera()) return;
        }
        //Keeping it simple and blindly polling the controllable
        Vector2 screenRatioTarget = mainControllable.TargetCharacterPosition();
        screenRatioTarget = new()
        {
            x = Mathf.Clamp01(screenRatioTarget.x),
            y = Mathf.Clamp01(screenRatioTarget.y),
        };
        Vector3 screenPixelPt = new Vector3(
            screenRatioTarget.x * Screen.width,
            screenRatioTarget.y * Screen.height,
            0
        );
        Vector3 camera_pos = _camera!.transform.position;
        Vector3 screenWorldPt = _camera!.ScreenToWorldPoint(screenPixelPt);
        screenWorldPt.z = camera_pos.z;
        Vector3 offset_req = screenWorldPt - camera_pos;
        //compute target world point
        Vector3 charPt = mainControllable.WorldPosition();
        Vector3 charVelLookahead = mainControllable.TargetLookAhead();
        Vector3 worldTargetPt = charPt + charVelLookahead;
        worldTargetPt.z = camera_pos.z;

        Vector3 new_campt = worldTargetPt + offset_req;
        //lerp camera
        float lerpAmt = Mathf.Clamp(mainControllable.Aggression() * Time.deltaTime, 0.0f, 1f);
        //Debug.Log($"CHR:{screenWorldPt} SCR:{offset_req} TGT:{new_campt} LERP:{lerpAmt} SPT:{screenPixelPt}");

        _camera.transform.position += (new_campt-camera_pos) * lerpAmt;
    }
    bool GetCamera()
    {
        _camera = this.gameObject.GetComponent<Camera>();
        return _camera != null;
    }
    private IControllable? mainControllable;
    void ChangedControllable(IControllable controllable)
    {
        boundsAsset = GameManager.Instance!.cameraBounds;
        mainControllable = controllable;
    }
}
