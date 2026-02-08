using UnityEngine;
using Yarn.Unity;

public class YarnManager : MonoBehaviour
{
    [SerializeField] private InfractionForm infractionForm;

    [YarnCommand("RevealName")]
    public void RevealName(string name)
    {
        if (infractionForm != null)
            infractionForm.RevealNameFromYarn(name);
        else
            Debug.LogWarning("InfractionForm not assigned in YarnManager.");
    }
}
