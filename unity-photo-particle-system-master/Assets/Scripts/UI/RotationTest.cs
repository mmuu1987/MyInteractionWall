using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationTest : MonoBehaviour {

	private Camera m_camera;
    public Transform Target;
    public float Distance = 5.0f;
    public float XSpeed = 5.0f;
    public float YSpeed = 5.0f;
 
    public float YMinLimit = -360f;
    public float YMaxLimit = 360f;
 
    public float DistanceMin = .5f;
    public float DistanceMax = 5000f;
 
    private float m_x = 0.0f;
    private float m_y = 0.0f;
 
    private void Awake()
    {
        m_camera = Camera.main;
    }
 
    private void Start()
    {
        SyncAngles();
    }
 
    public void SyncAngles()
    {
        Vector3 angles = transform.eulerAngles;
        m_x = angles.y;
        m_y = angles.x;
    }
 
    private void LateUpdate()
    {
        float deltaX = Input.GetAxis("Mouse X");
        float deltaY = Input.GetAxis("Mouse Y");
 
        deltaX = deltaX * XSpeed;
        deltaY = deltaY * YSpeed;
 
        m_x += deltaX;
        m_y -= deltaY;
        m_y = ClampAngle(m_y, YMinLimit, YMaxLimit);
        Quaternion.AngleAxis(m_x, Vector3.up);
        Quaternion rotation = Quaternion.Euler(m_y, m_x, 0);
        Target.transform.rotation = rotation;
 
       
    }
 
    private void OnEnable()
    {
        RestRotationInfo();
    }
 
    public void RestRotationInfo()
    {
        m_y = m_camera.transform.localEulerAngles.x;
        m_x = m_camera.transform.localEulerAngles.y;
    }
    public void Zoom()
    {
       
 
       
    }
 
 
 
    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
        {
            angle += 360F;
        }
        if (angle > 360F)
        {
            angle -= 360F;
        }
        return Mathf.Clamp(angle, min, max);
    }


}
