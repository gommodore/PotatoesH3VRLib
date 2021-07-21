using UnityEngine;

namespace H3VRUtils.Mapping
{
/// https://creativecommons.org/publicdomain/zero/1.0/
/// License: (CC0 1.0 Universal) You 're free to use these game assets in any project, 
/// personal or commercial. There's no need to ask permission before using these.
/// Giving attribution is not required, but is greatly appreciated!
/// The original author is Garret Polk
/// https://bitbucket.org/GarretPolk/h3vr-performance-utils/src/main/



/// <summary>
/// Turn on/off Behaviours (script components) based on 
/// whether Unity occlusion culls it.
/// https://blogs.unity3d.com/2016/12/20/unitytips-particlesystem-performance-culling/
/// </summary>
public class ComponentCulling : MonoBehaviour
{
    [Tooltip("If this sphere is seen, or the camera is inside, the sources are enabled.")]
    public float cullingRadius = 10;

    // Unity's culling group
    CullingGroup _mCullingGroup;

    [Tooltip("The Behaviours we want to enable/disable")]
    public Behaviour[] sources;

    // Used for culling initialization
    UnityEngine.Camera _cameraMain;

    void Update()
    {
        // NOTE : performance on this kinda sucks as
        // calling Camera.main is notoriously heavy.
        // I'd like to see a better mechanism exist to
        // message/event scripts when the scene is ready
        // to play.
        // 
        // Wait until we have a valid camera/player
        if (_cameraMain == null)
        {
            _cameraMain = UnityEngine.Camera.main;

            if (_cameraMain != null)
            {
                // Ready
                Init();
            }
        }
    }

    void Init()
    {
        // Hook the Unity occlusion culling system and set a callback function
        if (_mCullingGroup == null)
        {
            _mCullingGroup = new CullingGroup();
            _mCullingGroup.targetCamera = UnityEngine.Camera.main;
            _mCullingGroup.SetBoundingSpheres(new[] { new BoundingSphere(transform.position, cullingRadius) });
            _mCullingGroup.SetBoundingSphereCount(1);
            _mCullingGroup.onStateChanged += OnStateChanged;

            // We need to start in a culled state
            EnableComponent(_mCullingGroup.IsVisible(0));
        }

        _mCullingGroup.enabled = true;
    }

    void OnEnable()
    {
        if (_mCullingGroup != null)
        {
            _mCullingGroup.enabled = true;
            EnableComponent(true);
        }
    }

    void OnDisable()
    {
        if (_mCullingGroup != null)
        {
            _mCullingGroup.enabled = false;
            EnableComponent(false);
        }
    }

    /// <summary>
    /// Release memory
    /// </summary>
    void OnDestroy()
    {
        if (_mCullingGroup != null)
            _mCullingGroup.Dispose();
    }

    /// <summary>
    /// Callback from Unity's occlusion culling. State changed.
    /// </summary>
    /// <param name="sphere"></param>
    void OnStateChanged(CullingGroupEvent sphere)
    {
        EnableComponent(sphere.isVisible);
    }

    /// <summary>
    /// Enable/disable the components
    /// </summary>
    /// <param name="enable"></param>
    void EnableComponent(bool enable)
    {
        if (sources != null)
        {
            for (int i = sources.Length - 1; i > -1; --i)
            {
                sources[i].enabled = enable;
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (enabled)
        {
            // Draw gizmos to show the culling sphere.
            Color col = Color.yellow;
            if (_mCullingGroup != null && !_mCullingGroup.IsVisible(0))
                col = Color.gray;

            Gizmos.color = col;
            Gizmos.DrawWireSphere(transform.position, cullingRadius);
        }
    }
}
}