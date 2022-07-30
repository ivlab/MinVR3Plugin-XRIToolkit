using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IVLab.MinVR3;

public class UIPanels : MonoBehaviour
{
    void Start()
    {

        Canvas[] childCanvases = GetComponentsInChildren<Canvas>();
        foreach (var canvas in childCanvases) {

            // all panels get the same setting, either render in world space or as screen space overlay
            canvas.renderMode = ConfigVal.Get("UI_PANELS_IN_WORLD_SPACE", false) ?
                RenderMode.WorldSpace : RenderMode.ScreenSpaceOverlay;

            // apply the same scale to all panels as well.
            // default to (1,1,1), which is correct for screen space overlay.
            // for world space, this needs to be scaled down dramatically, like scale by 0.005 or so.
            canvas.transform.localScale = ConfigVal.Get("UI_PANELS_SCALE", Vector3.one);

            // the position and rotation can be set on a canvas-by-canvas basis.
            // notice the key for the configval lookup includes the name of the game object.
            string goName = canvas.gameObject.name.ToUpper().Replace(" ", "_");
            canvas.transform.position = ConfigVal.Get(goName + "_POSITION", Vector3.zero);
            canvas.transform.rotation = Quaternion.Euler(ConfigVal.Get(goName + "_ROTATIONANGLES", Vector3.zero));
        }
    }

}
