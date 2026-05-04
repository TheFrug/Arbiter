using UnityEngine;
using Yarn.Unity;

public class InterrogationSceneBinder : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private VariableStorageBehaviour variableStorage;

    // We do not need to assign this via Inspector; we fetch the persistent instance
    [SerializeField] private PlayerManager playerManager;

    [SerializeField] private ComplianceForm complianceForm;
    [SerializeField] private PortraitManager portraitManager;

    private void Start()
    {
        // Fetch the persistent PlayerManager instance if it is not assigned
        if (playerManager == null && PlayerManager.Instance != null)
        {
            playerManager = PlayerManager.Instance;
        }

        if (YarnManager.Instance != null)
        {
            YarnManager.Instance.RegisterSceneDependencies(
                dialogueRunner,
                variableStorage,
                playerManager,
                complianceForm,
                portraitManager
            );
        }
        else
        {
            Debug.LogError("YarnManager Instance is not set up. Make sure the YarnManager is created before binding dependencies.");
        }
    }
}