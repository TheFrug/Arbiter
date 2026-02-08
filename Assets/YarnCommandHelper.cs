using UnityEngine;
using Yarn.Unity;
using System;

public class YarnCommandHandler : MonoBehaviour
{
    [SerializeField] private InfractionForm infractionForm;

    private void Awake()
    {
        var dialogueRunner = FindObjectOfType<DialogueRunner>();

        if (dialogueRunner == null)
        {
            Debug.LogError("No DialogueRunner found in scene!");
            return;
        }

        // Register this instance as a handler
        dialogueRunner.AddCommandHandler("RevealName", RevealName);
    }

    // Must have this exact signature
    private void RevealName(string[] parameters)
    {
        if (parameters == null || parameters.Length < 1)
        {
            Debug.LogWarning("RevealName requires a string parameter!");
            return;
        }

        string name = parameters[0];

        if (infractionForm != null)
        {
            infractionForm.OnYarnRevealName(name);
        }
    }
}
