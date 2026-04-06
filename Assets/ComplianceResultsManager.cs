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

            // Ensure arrays are same length for comparison
            int flagCount = Mathf.Min(form.behaviorFlags.Length, subject.correctBehaviorFlags.Length);

            int score = 0;
            int max = 3 + flagCount; // 3 base points (name, occupation, loyalty) + behavior flags

            bool nameCorrect = form.subjectName == subject.correctName;
            bool occupationCorrect = form.occupation == subject.correctOccupation;
            bool loyaltyCorrect = form.loyaltyRating == subject.correctLoyalty;

            if (nameCorrect) score++;
            if (occupationCorrect) score++;
            if (loyaltyCorrect) score++;

            // Behavior flags comparison
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
                nameCorrect = nameCorrect,
                occupationCorrect = occupationCorrect,
                loyaltyCorrect = loyaltyCorrect
            });

            // Debug log for rapid testing
            Debug.Log($"Evaluated {form.interviewID}: Name({nameCorrect}), Occ({occupationCorrect}), Loyalty({loyaltyCorrect}), Score({score}/{max})");
        }

        return results;
    }
}