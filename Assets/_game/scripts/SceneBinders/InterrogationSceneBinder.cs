using UnityEngine;
using Yarn.Unity;

public class InterrogationSceneBinder : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private VariableStorageBehaviour variableStorage;
    [SerializeField] private PlayerManager playerManager;
    [SerializeField] private ComplianceForm complianceForm;

    private void Start()
    {
        YarnManager.Instance.RegisterSceneDependencies(
            dialogueRunner,
            variableStorage,
            playerManager,
            complianceForm
        );
    }
}