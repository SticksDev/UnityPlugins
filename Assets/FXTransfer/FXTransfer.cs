using System.Threading;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

public class FXTransfer : EditorWindow
{
    // Create menuItems

    [MenuItem("FXTransfer/Open")]
    private static void HelloWorld()
    {
        Debug.Log("FXTransfer 1.0.0 Starting. Calling gui show");

        // Show the gui
        GetWindow(typeof(FXTransfer));
    }

    private Vector2 mainScrollPos;
    private AnimatorController inAnimator;
    private AnimatorController copyController;

    // Animator Values
    private AnimatorControllerLayer[] inAnimatorLayers;
    private AnimatorControllerParameter[] inAnimatorParams;

    // Label values
    private string labelValue = "Awaiting User Input";
    private string pbValue = "";


    private void Transfer() 
    {
        labelValue = "Processing started...";
        
        // First, we need to get all of the layers/prams of the inAnimator
        inAnimatorLayers = inAnimator.layers;
        inAnimatorParams = inAnimator.parameters;

        // Create new loadingbar
        EditorUtility.DisplayProgressBar("FXTransfer (Working)", "Copying layers...", 0.0f);

        // Secound, we need to copy the layers and params to the copyController
        foreach (AnimatorControllerLayer inLayer in inAnimatorLayers)
        {
            if (inLayer.stateMachine == null || inLayer.name == null)
            {
                Debug.LogWarning("Warning: Unknown layer found (missing stateMachine or name). Skipping.");
                continue;
            }

            pbValue = $"Processing layer: {inLayer.name}";

            // Show the progress for the current foreach
            EditorUtility.DisplayProgressBar("FXTransfer (Working)", pbValue, 0.5f);
            
            AnimatorControllerLayer copyLayer = new AnimatorControllerLayer();

            copyLayer.name = inLayer.name;
            copyLayer.stateMachine = inLayer.stateMachine;
            copyLayer.defaultWeight = inLayer.defaultWeight; // Always has a value

            copyController.AddLayer(copyLayer);
        }

        EditorUtility.DisplayProgressBar("FXTransfer (Working)", "Copying parameters...", 0.5f);

        // Same for the paramters
        foreach (AnimatorControllerParameter inParam in inAnimatorParams)
        {
            if (inParam.name == null)
            {
                Debug.LogWarning("Warning: UnknownParameter has no name. Skipping.");
                continue;
            }

            pbValue = $"Processing parameter: {inParam.name}";

            EditorUtility.DisplayProgressBar("FXTransfer (Working)", pbValue, 0.5f);

            AnimatorControllerParameter copyParam = new AnimatorControllerParameter();
            
            copyParam.name = inParam.name;
            copyParam.type = inParam.type; // Always has a value

            copyController.AddParameter(copyParam);
        }

        EditorUtility.ClearProgressBar();
        labelValue = "Processing finished.";

        EditorUtility.DisplayDialog("FXTransfer", "Processing finished.", "OK");
    }

    private void OnGUI()
    {
        mainScrollPos = GUILayout.BeginScrollView(mainScrollPos);
        GUILayout.Space(10);
        // Load the file named logo into a graphic
        Texture2D logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/FXTransfer/Assets/logo.png", typeof(Texture2D));
        GUI.DrawTexture(new Rect((Screen.width / 2) - (logo.width / 2), (Screen.height / 11) - (logo.height / 11), logo.width, logo.height), logo);
        GUILayout.Space(135);
        GUI.skin.label.fontSize = 12;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("by: SticksDev");
        GUILayout.Space(10);
        inAnimator = (AnimatorController)EditorGUILayout.ObjectField("Input Animator", inAnimator, typeof(AnimatorController), true);
        GUILayout.Space(10);
        copyController = (AnimatorController)EditorGUILayout.ObjectField("Output Animator", copyController, typeof(AnimatorController), true);

        // Go button login
        if (inAnimator == null || copyController == null)
        {
            EditorGUILayout.HelpBox("Please select your Input/Output Animator", MessageType.Warning);
        }
        else if (inAnimator == copyController)
        {
            EditorGUILayout.HelpBox("Input and Output Animator can't be the same", MessageType.Warning);
        }


        if (GUILayout.Button("Transfer!") && inAnimator != null && copyController != null && inAnimator != copyController)
        {
            // Ask for confirmation 
            if (EditorUtility.DisplayDialog("FXTransfer", "Are you sure you want to transfer the animator?\nTHIS WILL NOT OVERWRITE, THIS WILL ADD. ONLY DO THIS ONCE OR YOU MAY HAVE TO START OVER.", "Yes", "No"))
            {
                Transfer();
            }
        }

        GUILayout.Space(5);
        GUILayout.Label(labelValue);
        // Paint
        GUILayout.EndScrollView();
    }
}
