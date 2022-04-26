using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraService : BaseSingleService<CameraService>, IUpdate
{
    Camera m_mainCamera;
    Transform m_cameraTarget;
    Vector3 m_deltaPos = new Vector3(0, 12, -12);
    Quaternion m_rotate = Quaternion.AngleAxis(45, Vector3.right);

    public override void DoStart()
    {
        base.DoStart();
        var go = GameObject.FindWithTag("MainCamera");
        if (go == null) 
        {
            go = new GameObject("MainCamera");
            go.tag = "MainCamera";
            go.AddComponent<Camera>();
        }

        GameObject.DontDestroyOnLoad(go);
        m_mainCamera = go.GetComponent<Camera>();
        m_mainCamera.orthographic = false;
        m_mainCamera.fieldOfView = 45;
        m_mainCamera.nearClipPlane = 0.3f;
        m_mainCamera.farClipPlane = 100.0f;
    }

    public void SetTarget(Transform trans) 
    {
        m_cameraTarget = trans;
    }

    public void DoUpdate(float deltaTime)
    {
        if (m_cameraTarget == null)
            return;

        m_mainCamera.transform.position = m_cameraTarget.transform.position + m_deltaPos;
        m_mainCamera.transform.rotation = m_rotate;
    }
}
