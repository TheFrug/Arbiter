using System.Collections.Generic;
using UnityEngine;

public class InterviewManager : MonoBehaviour
{
    [Header("Subject Pools")]
    [SerializeField] private List<SubjectData> routineSubjects;
    [SerializeField] private List<SubjectData> storySubjects;

    [Header("Daily Settings")]
    [SerializeField] private int interviewsPerShift = 3;

    [Header("Options")]
    [SerializeField] private bool randomizeQueue = true; // toggle randomization

    private Queue<SubjectData> dailyQueue = new Queue<SubjectData>();

    private void Awake()
    {
        PrepareDailyQueue();
    }

    private void PrepareDailyQueue()
    {
        dailyQueue.Clear();

        List<SubjectData> workingPool = new List<SubjectData>(routineSubjects);

        // Select one story subject (always first)
        if (storySubjects.Count > 0)
        {
            SubjectData storySubject;
            if (randomizeQueue)
            {
                int storyIndex = Random.Range(0, storySubjects.Count);
                storySubject = storySubjects[storyIndex];
            }
            else
            {
                storySubject = storySubjects[0];
            }
            dailyQueue.Enqueue(storySubject);
        }

        // Fill remaining slots with routine subjects
        int remaining = interviewsPerShift - dailyQueue.Count;

        for (int i = 0; i < remaining; i++)
        {
            if (workingPool.Count == 0)
                break;

            SubjectData selected;
            if (randomizeQueue)
            {
                int index = Random.Range(0, workingPool.Count);
                selected = workingPool[index];
                workingPool.RemoveAt(index);
            }
            else
            {
                selected = workingPool[0];
                workingPool.RemoveAt(0);
            }

            dailyQueue.Enqueue(selected);
        }

        // Optional: shuffle entire queue if randomizeQueue is true
        if (randomizeQueue)
        {
            List<SubjectData> shuffleList = new List<SubjectData>(dailyQueue);
            dailyQueue.Clear();

            while (shuffleList.Count > 0)
            {
                int index = Random.Range(0, shuffleList.Count);
                dailyQueue.Enqueue(shuffleList[index]);
                shuffleList.RemoveAt(index);
            }
        }
    }

    public SubjectData GetNextSubject()
    {
        if (dailyQueue.Count == 0)
            return null;

        return dailyQueue.Dequeue();
    }
}