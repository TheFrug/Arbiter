using UnityEngine;
using Yarn.Unity;

public class YarnManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private InfractionForm infractionForm;

    // --- Yarn Commands ---
    // Usage in Yarn: <<RevealName "Darion Vale" YarnManager>>
    [YarnCommand("RevealName")]
    public void RevealName(string name)
    {
        if (infractionForm == null)
        {
            Debug.LogWarning("YarnManager: InfractionForm reference missing!");
            return;
        }

        // Call the correct method in InfractionForm
        infractionForm.RevealNameFromYarn(name);
    }
}
