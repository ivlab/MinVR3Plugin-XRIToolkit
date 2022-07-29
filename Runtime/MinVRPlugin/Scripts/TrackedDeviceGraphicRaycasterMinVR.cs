using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.XR.Interaction.Toolkit.UI;

namespace IVLab.MinVR3.XRIToolkit
{
    /// <summary>
    /// Wrapper around the TrackedDeviceGraphicsRaycaster from Unity's XR Interaction Toolkit that makes it possible to
    /// disable raycasting when the <c>SharedToken</c> inputFocusToken is already held by some other widget or
    /// interactive technique. The effect is that any Unity canvases that use this raycaster are prevented from
    /// activating when a MinVR interaction is active, making it possible to use MinVR's notion of input focus together
    /// with UnityUI canavas and more nicely integrate the two styles of 3D interaction in the same application.
    /// </summary>
    [AddComponentMenu("MinVR/Interaction/Tracked Device Graphic Raycaster MinVR", 11)]
    public class TrackedDeviceGraphicRaycasterMinVR : TrackedDeviceGraphicRaycaster
    {
        [Header("MinVR Settings")]

        [Tooltip("[Optional] If set, raycasts will only be conducted if the token is available (i.e., not already held by someone else).  This has the effect of preventing any Unity canvases that use this raycaster from activating if a MinVR interactive technique has taken focus.")]
        public SharedToken inputFocusToken;

        public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
        {
            if ((inputFocusToken == null) || (inputFocusToken.RequestToken(this))) {
                base.Raycast(eventData, resultAppendList);
                inputFocusToken?.ReleaseToken(this);
            }
        }
    }
}
