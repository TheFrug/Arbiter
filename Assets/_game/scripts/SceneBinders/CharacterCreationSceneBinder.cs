using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class CharacterCreationSceneBinder : MonoBehaviour
{
    [SerializeField] private DialogueRunner dialogueRunner;
    [SerializeField] private VariableStorageBehaviour variableStorage;
    [SerializeField] private InfractionForm infractionForm;
    [SerializeField] private PlayerManager playerManager;

    private void Start()
    {
        YarnManager.Instance.RegisterSceneDependencies(
            dialogueRunner,
            variableStorage,
            infractionForm,
            playerManager
        );
    }
}