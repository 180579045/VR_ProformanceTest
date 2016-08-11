using UnityEngine;
using System.Collections;
using UnityEditor;

public class VRbuildSeting{
    [MenuItem("VR build Setting/build GearVR")]
    static void BuildGearVRSetting()
    {
        PlayerSettings.virtualRealitySupported = true;
        Cardboard cardboard = (Cardboard)GameObject.FindGameObjectWithTag("Player").GetComponent<Cardboard>();
        CardboardHead cardboardHead = cardboard.gameObject.transform.GetChild(0).gameObject.GetComponent<CardboardHead>();
        cardboard.VRModeEnabled = false;
        cardboardHead.trackPosition = false;
        cardboardHead.trackRotation = false;
        EditorUtility.SetDirty(cardboard);
        EditorUtility.SetDirty(cardboardHead);
    }


    [MenuItem("VR build Setting/build Cardboard")]
    static void BuildCardboardSetting()
    {
        PlayerSettings.virtualRealitySupported = false;
        Cardboard cardboard = (Cardboard)GameObject.FindGameObjectWithTag("Player").GetComponent<Cardboard>();
        CardboardHead cardboardHead = cardboard.gameObject.transform.GetChild(0).gameObject.GetComponent<CardboardHead>();
        cardboard.VRModeEnabled = true;
        cardboardHead.trackPosition = true;
        cardboardHead.trackRotation = true;
        EditorUtility.SetDirty(cardboard);
        EditorUtility.SetDirty(cardboardHead);
    }

}
