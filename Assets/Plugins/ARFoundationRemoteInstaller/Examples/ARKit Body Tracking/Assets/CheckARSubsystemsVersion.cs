using UnityEngine;


public class CheckARSubsystemsVersion : MonoBehaviour {
    void Awake() {
        if (!AR_SUBSYSTEMS_4_0_1_OR_NEWER) {
            Debug.LogError("Please install AR Foundation >= 4.0.2 to enable Body Tracking.");
        }
    }

    static bool AR_SUBSYSTEMS_4_0_1_OR_NEWER {
        get {
            return
            #if AR_SUBSYSTEMS_4_0_1_OR_NEWER
                true;
            #else
                false;
            #endif
        }
    }
}
