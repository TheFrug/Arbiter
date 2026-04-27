using System.Collections.Generic;
using UnityEngine;

public class ComplianceResultsManager : MonoBehaviour
{
    public static ComplianceResultsManager Instance;

    [Header("All Subjects")]
    [SerializeField] private List<SubjectData> allSubjects;

    private List<ComplianceForm.ComplianceFormData> submittedForms = new List<ComplianceForm.ComplianceFormData>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RegisterForm(ComplianceForm.ComplianceFormData data)
    {
        submittedForms.Add(data);
    }

    public List<ComplianceForm.ComplianceFormData> GetAllForms()
    {
        return submittedForms;
    }

    public void ClearResults()
    {
        submittedForms.Clear();
    }

    // ================= EVALUATION =================

    public class EvaluationResult
    {
        public string interviewID;

        public int score;
        public int maxScore;

        public bool nameCorrect;
        public bool occupationCorrect;
        public bool loyaltyCorrect;
    }

    private SubjectData GetSubject(string interviewID)
    {
        return allSubjects.Find(s => s.InterviewID == interviewID);
    }

    public List<EvaluationResult> EvaluateAll()
    {
        List<EvaluationResult> results = new List<EvaluationResult>();

        foreach (var form in submittedForms)
        {
            var subject = GetSubject(form.interviewID);
            if (subject == null) continue;

            int flagCount = Mathf.Min(form.behaviorFlags.Length, subject.correctBehaviorFlags.Length);
            int score = 0;
            int max = 3 + flagCount;

            // Strict string comparison to prevent "False" results from minor formatting
            bool nameMatch = string.Equals(form.subjectName?.Trim(), subject.correctName?.Trim(), System.StringComparison.Ordinal);
            bool occMatch = string.Equals(form.occupation?.Trim(), subject.correctOccupation?.Trim(), System.StringComparison.Ordinal);
            bool loyaltyMatch = (form.loyaltyRating == subject.correctLoyalty);

            if (nameMatch) score++;
            if (occMatch) score++;
            if (loyaltyMatch) score++;

            // Behavior flags
            for (int i = 0; i < flagCount; i++)
            {
                if (form.behaviorFlags[i] == subject.correctBehaviorFlags[i])
                {
                    score++;
                }
            }

            results.Add(new EvaluationResult
            {
                interviewID = form.interviewID,
                score = score,
                maxScore = max,
                nameCorrect = nameMatch,
                occupationCorrect = occMatch,
                loyaltyCorrect = loyaltyMatch
            });
        }

        return results;
    }
}