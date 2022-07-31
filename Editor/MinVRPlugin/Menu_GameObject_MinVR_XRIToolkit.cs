using UnityEditor;
using UnityEngine;
using System;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.Rendering;
using UnityEngine.UI;
using UnityEngine.EventSystems;


// disable warnings about unused functions because these editor menu functions can look to the compiler
// as though they are never called
#pragma warning disable IDE0051

namespace IVLab.MinVR3.XRIToolkit
{

    public class Menu_GameObject_MinVR_XRIToolkit : MonoBehaviour
    {
        [MenuItem("GameObject/MinVR Interaction/Unity's XR Interaction Toolkit/XR Ray Interactor (MinVR-Based)", false, MenuHelpers.mvriItemPriority)]
        public static void CreateInteractionXRRayInteractor(MenuCommand command)
        {
            MenuHelpers.CreateVREngineIfNeeded();
            MenuHelpers.CreateRoomSpaceOriginIfNeeded();
            CreateXRInteractionManagerIfNeeded();

            var go = MenuHelpers.CreateAndPlaceGameObject("XR Ray Interactor (MinVR-Based)", command.context as GameObject,
                new Type[] { typeof(MinVRBasedController), typeof(XRRayInteractor),
                typeof(LineRenderer), typeof(XRInteractorLineVisual) });
            LineRenderer lineRenderer = go.GetComponent<LineRenderer>();

            // Copied from XR Interaction Toolkit so this line renderer will be setup the exact same way
            var materials = new Material[1];
            materials[0] = AssetDatabase.GetBuiltinExtraResource<Material>("Default-Line.mat");
            lineRenderer.materials = materials;
            lineRenderer.loop = false;
            lineRenderer.widthMultiplier = 0.005f;
            lineRenderer.startColor = Color.blue;
            lineRenderer.endColor = Color.blue;
            lineRenderer.numCornerVertices = 4;
            lineRenderer.numCapVertices = 4;
            lineRenderer.shadowCastingMode = ShadowCastingMode.Off;
            lineRenderer.receiveShadows = false;
            lineRenderer.useWorldSpace = true;
            lineRenderer.sortingOrder = 5;
        }


        [MenuItem("GameObject/MinVR Interaction/Unity's XR Interaction Toolkit/XR Direct Interactor (MinVR-Based)", false, MenuHelpers.mvriItemPriority)]
        public static void CreateInteractionXRDirectInteractor(MenuCommand command)
        {
            MenuHelpers.CreateVREngineIfNeeded();
            MenuHelpers.CreateRoomSpaceOriginIfNeeded();
            CreateXRInteractionManagerIfNeeded();


            var go = MenuHelpers.CreateAndPlaceGameObject("XR Direct Interactor (MinVR-Based)", command.context as GameObject,
                new Type[] { typeof(MinVRBasedController), typeof(XRDirectInteractor),
                typeof(SphereCollider) });
            var collider = go.GetComponent<SphereCollider>();


            // Copied from Unity's XR Interaction Toolkit so this collider will be setup the exact same way
            collider.isTrigger = true;

            var scaleFactor = 0f;
            var lossyScale = collider.transform.lossyScale;

            for (var axis = 0; axis < 3; ++axis)
                scaleFactor = Mathf.Max(scaleFactor, Mathf.Abs(lossyScale[axis]));

            collider.radius = !Mathf.Approximately(scaleFactor, 0f) ? 0.1f / scaleFactor : 0f;
        }


        [MenuItem("GameObject/MinVR Interaction/Unity's XR Interaction Toolkit/UI Canvas (with TrackedDeviceGraphicRaycasterMinVR)", false, MenuHelpers.mvriItemPriority)]
        public static void CreateInteractionUICanvas(MenuCommand command)
        {
            MenuHelpers.CreateVREngineIfNeeded();
            MenuHelpers.CreateRoomSpaceOriginIfNeeded();
            CreateXRInteractionManagerIfNeeded();
            CreateXRUIInputModuleIfNeeded();

            var go = MenuHelpers.CreateAndPlaceGameObject("UI Canvas", command.context as GameObject,
                new Type[] { typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster),
                typeof(TrackedDeviceGraphicRaycasterMinVR)});

            var canvas = go.GetComponent<Canvas>();
            canvas.renderMode = RenderMode.WorldSpace;
        }


        public static void CreateXRInteractionManagerIfNeeded()
        {
            var xrInteractionMgr = FindObjectOfType<XRInteractionManager>();
            if (xrInteractionMgr == null) {
                MenuHelpers.CreateAndPlaceGameObject("XR Interaction Manager", null, typeof(XRInteractionManager));
            }
        }


        public static void CreateXRUIInputModuleIfNeeded()
        {
            GameObject xruiInputModuleGO = null;
            var xruiInputModule = FindObjectOfType<XRUIInputModule>();
            if (xruiInputModule != null) {
                xruiInputModuleGO = xruiInputModule.gameObject;
            } else {
                xruiInputModuleGO = MenuHelpers.CreateAndPlaceGameObject("XR UI Input Module", null, typeof(XRUIInputModule));
            }

            // XR UI Input Module has two special requirements:
            // - an EventSystem must also exist
            // - the StandAloneInputModule must not exist

            var eventSystem = FindObjectOfType<EventSystem>();
            if (eventSystem == null) {
                xruiInputModuleGO.AddComponent<EventSystem>();
            }

            var standaloneInputModule = FindObjectOfType<StandaloneInputModule>();
            if (standaloneInputModule != null) {
                DestroyImmediate(standaloneInputModule);
            }
        }


    } // end class

} // end namespace

#pragma warning restore IDE0051       
