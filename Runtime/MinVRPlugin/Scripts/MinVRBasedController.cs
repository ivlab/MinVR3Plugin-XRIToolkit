
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

namespace IVLab.MinVR3.XRIToolkit
{

    /// <summary>
    /// This class inherits from the XRBaseController class in Unity's XR Interaction Toolkit to create a
    /// simple controller that can be driven by MinVR VREvents.  In this way, it is (nearly) a plug-in
    /// replacement for the XR Controller (Device-based) and XR Controller (Input Action-based) objects
    /// that ship with the XR Interaction Toolkit.  This is useful when you wish to utilize some of the
    /// functionality provided by Unity's XR Interaction Toolkit but you are using a custom device in a
    /// CAVE or other VR configuration that receives input from VRPN or some other input device that
    /// MinVR knows how to handle, but Unity does not.
    ///
    /// The XR controller defined here is (so far) much simpler than the commercial controllers, which
    /// have a multitude of buttons, triggers, and pads on them.  If that functionality is needed, it
    /// is not hard to extend this class to set more of the XRBaseController's input state based on
    /// VREvents received.
    /// </summary>
    [AddComponentMenu("XR/XR Controller (MinVR-based)", 11)]
    public class MinVRBasedController : XRBaseController, IVREventListener
    {

        [Header("MinVR Inputs")]

        [Tooltip("Controller position event.")]
        public VREventPrototypeVector3 m_PositionEvent = new VREventPrototypeVector3();

        [Tooltip("Controller rotation event.")]
        public VREventPrototypeQuaternion m_RotationEvent = new VREventPrototypeQuaternion();

        [Tooltip("Button down event to use for Canvas UI interaction.")]
        public VREventPrototype m_UIButtonDownEvent = new VREventPrototype();

        [Tooltip("Button down event to use for Canvas UI interaction.")]
        public VREventPrototype m_UIButtonUpEvent = new VREventPrototype();


        protected override void OnEnable()
        {
            base.OnEnable();
            StartListening();
        }


        protected override void OnDisable()
        {
            base.OnDisable();
            StopListening();
        }


        /// <inheritdoc />
        protected override void UpdateTrackingInput(XRControllerState controllerState)
        {
            // usually this can be done inside OnEnable, but Unity must initialize these XRControllers before
            // the VREngine is initialized.
            if ((enabled) && (!m_Listening)) {
                StartListening();
            }

            base.UpdateTrackingInput(controllerState);
            if (controllerState == null)
                return;

            bool trackingPosition = (m_PositionEvent.GetEventName() != "");
            bool trackingRotation = (m_RotationEvent.GetEventName() != "");

            // Update inputTrackingState
            controllerState.inputTrackingState = InputTrackingState.None;
            if (trackingPosition) {
                controllerState.inputTrackingState = controllerState.inputTrackingState | InputTrackingState.Position;
            }
            if (trackingRotation) {
                controllerState.inputTrackingState = controllerState.inputTrackingState | InputTrackingState.Rotation;
            }

            // Update position
            if (trackingPosition) {
                controllerState.position = m_Position;
            }

            // Update rotation
            if (trackingRotation) {
                controllerState.rotation = m_Rotation;
            }
        }


        /// <inheritdoc />
        protected override void UpdateInput(XRControllerState controllerState)
        {
            // usually this can be done inside OnEnable, but Unity must initialize these XRControllers before
            // the VREngine is initialized.
            if ((enabled) && (!m_Listening)) {
                StartListening();
            }

            base.UpdateInput(controllerState);
            if (controllerState == null)
                return;

            controllerState.ResetFrameDependentStates();
            controllerState.uiPressInteractionState.SetFrameState(m_UIBtnDown);
        }

        public void OnVREvent(VREvent vrEvent)
        {
            if (enabled) {
                if (vrEvent.Matches(m_PositionEvent)) {
                    m_Position = vrEvent.GetData<Vector3>();
                } else if (vrEvent.Matches(m_RotationEvent)) {
                    m_Rotation = vrEvent.GetData<Quaternion>();
                } else if (vrEvent.Matches(m_UIButtonDownEvent)) {
                    m_UIBtnDown = true;
                } else if (vrEvent.Matches(m_UIButtonUpEvent)) {
                    m_UIBtnDown = false;
                }
            }
        }

        public void StartListening()
        {
            if ((VREngine.instance != null) && (VREngine.instance.eventManager != null)) {
                VREngine.Instance.eventManager.AddEventListener(this, VREventManager.DefaultListenerPriority - 1);
                m_Listening = true;
            }
        }

        public void StopListening()
        {
            VREngine.Instance?.eventManager?.RemoveEventListener(this);
            m_Listening = false;
        }

        bool m_Listening = false;
        bool m_UIBtnDown = false;
        Vector3 m_Position = new Vector3();
        Quaternion m_Rotation = new Quaternion();
    }
}
