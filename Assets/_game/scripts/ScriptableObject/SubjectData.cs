using UnityEngine;

[CreateAssetMenu(fileName = "NewSubject", menuName = "Game/Subject Data")]
public class SubjectData : ScriptableObject
{
    [Header("Identity")]
    public string InterviewID;
    public string Date;
    public string displayName;
    public string occupation;
    public string IdentificationCode;

    [Header("Correct Answers")]
    public string correctName;
    public string correctOccupation;
    public int correctLoyalty; // 1 = Low, 2 = Mid, 3 = High
    public bool[] correctBehaviorFlags;

    [Header("Dialogue")]
    public string yarnStartNode;

    [Header("Visuals")]
    public Sprite portrait;
}