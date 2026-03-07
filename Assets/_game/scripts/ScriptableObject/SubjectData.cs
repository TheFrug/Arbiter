using UnityEngine;

[CreateAssetMenu(fileName = "NewSubject", menuName = "Game/Subject Data")]
public class SubjectData : ScriptableObject
{
    [Header("Identity")]
    public string InterviewID;
    public string Date;
    public string displayName;
    public string occupation;

    [Header("Dialogue")]
    public string yarnStartNode;

    [Header("Visuals")]
    public Sprite portrait;
}