using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewSuspect", menuName = "Game/Suspect Data")]
public class SuspectData : ScriptableObject
{
    [Header("Identity")]
    public string caseID;
    public string displayName;

    public string ArrestingOfficer;
    public string CitizenshipTier;

    [Header("Dialogue")]
    public string yarnStartNode;

    [Header("Visuals")]
    public Sprite portrait;
}
