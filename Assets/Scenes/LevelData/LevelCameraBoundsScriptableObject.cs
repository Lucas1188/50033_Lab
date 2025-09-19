using UnityEngine;

[CreateAssetMenu(fileName = "CameraBounds", menuName = "Camera/Bounds Lines")]
public class CameraBounds : ScriptableObject
{
    // Each line is defined by two points in world space
    public Line[] lines;

    [System.Serializable]
    public struct Line
    {
        public Vector2 start;
        public Vector2 end;
    }
}
