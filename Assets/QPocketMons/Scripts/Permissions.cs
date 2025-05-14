using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.XR.ARFoundation;

public class Permissions : MonoBehaviour
{
    const string k_SceneUnderstandingCoarsePermission = "android.permission.SCENE_UNDERSTANDING_COARSE";
    const string k_HandTrackingPermission = "android.permission.HAND_TRACKING";
    const string k_EyeTrckingCoarsePermission = "android.permission.EYE_TRACKING_COARSE";
    const string k_EyeTrackingFinePermission = "android.permission.EYE_TRACKING_FINE";

    [SerializeField]
    ARPlaneManager m_ARPlaneManager;

    [SerializeField]
    ARFaceManager m_ARFaceManager;

#if UNITY_ANDROID
    void Start()
    {
        var permissions = new List<string>();

        // add permissions that have not been granted by the user
        if (!Permission.HasUserAuthorizedPermission(k_SceneUnderstandingCoarsePermission))
        {
            permissions.Add(k_SceneUnderstandingCoarsePermission);
        }
        else
        {
            // enable the AR Plane Manager component if permission is already granted
            m_ARPlaneManager.enabled = true;
        }

        if (!Permission.HasUserAuthorizedPermission(k_HandTrackingPermission))
        {
            permissions.Add(k_HandTrackingPermission);
        }

        if (!Permission.HasUserAuthorizedPermission(k_EyeTrackingFinePermission))
        {
            permissions.Add(k_EyeTrackingFinePermission);
        }
        else
        {
            // enable the AR Face Manager component if permission is already granted
        //    m_ARFaceManager.enabled = true;
        }

        if (!Permission.HasUserAuthorizedPermission(k_EyeTrckingCoarsePermission))
        {
            permissions.Add(k_EyeTrckingCoarsePermission);
        }

        // setup callbacks to be called depending on whether permission is granted
        var callbacks = new PermissionCallbacks();
        callbacks.PermissionDenied += OnPermissionDenied;
        callbacks.PermissionGranted += OnPermissionGranted;

        Permission.RequestUserPermissions(permissions.ToArray(), callbacks);
    }

    void OnPermissionDenied(string permission)
    {
        // handle denied permission
    }

    void OnPermissionGranted(string permission)
    {
        // enable the corresponding AR Manager component after required permission is granted
        if (permission == k_SceneUnderstandingCoarsePermission)
        {
            m_ARPlaneManager.enabled = true;
        }
        else if (permission == k_EyeTrckingCoarsePermission)
        {
        //    m_ARFaceManager.enabled = true;
        }
    }
#endif // UNITY_ANDROID
}
