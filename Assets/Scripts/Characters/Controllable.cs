using UnityEngine;
public class ControllableCharacter : MonoBehaviour, IControllable
{
    private Rigidbody2D rigidbody2D;
    void Awake()
    {
        GameManager.Instance.RegisterControllable(this);
    }
    void Start()
    {
        rigidbody2D = this.gameObject.GetComponent<Rigidbody2D>();
    }
    //Controls Section for controllable character
    private bool isInControl;
    private bool camerasettings_changed = false;
    [SerializeField]
    private float cameraLerpAggression = 1.0f;
    [SerializeField]
    private Vector2 targetCharacterPosition= new(0.3f,0.2f);
    [SerializeField]
    private float targetLookAheadRatio= 0.5f;
    public void OnControlRemoved()
    {
        isInControl = false;
    }

    public bool OnTakeControl()
    {
        isInControl = true;
        return true;
    }

    public bool IsInControl()
    {
        return isInControl;
    }

    public Vector2 TargetCharacterPosition()
    {
        return targetCharacterPosition.normalized;
    }

    public Vector2 TargetLookAhead()
    {
        if (rigidbody2D == null) return Vector2.zero;
        return rigidbody2D.linearVelocity * targetLookAheadRatio;
    }
    public Vector3 WorldPosition()
    {
        if (rigidbody2D == null) return Vector2.zero;
        return rigidbody2D.position;
    }

    public float Aggression()
    {
        return cameraLerpAggression;
    }

    public bool CameraSettingsChanged()
    {
        if (camerasettings_changed)
        {
            camerasettings_changed = false;
            return true;
        }
        return false;
    }
}