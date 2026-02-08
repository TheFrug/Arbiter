using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Yarn.Unity;

public class InfractionForm : MonoBehaviour
{
    #region Enums

    public enum ViolationType
    {
        PublicDisorder,
        EconomicNonCompliance,
        IdeologicalDeviance,
        AdministrativeObstruction
    }

    public enum SeverityLevel
    {
        Minor,
        Standard,
        Aggravated
    }

    #endregion

    #region Data Struct

    [Serializable]
    public struct InfractionFormData
    {
        public string caseID;
        public string suspectName;
        public ViolationType violation;
        public SeverityLevel severity;
        public bool stamped;
    }

    #endregion

    #region Inspector References

    [Header("Header Data")]
    [SerializeField] private string caseID;

    [Header("Identity")]
    [SerializeField] private Button nameFillButton;
    [SerializeField] private TMP_Text nameDisplayText;

    [Header("Violation Toggles")]
    [SerializeField] private Toggle publicDisorderToggle;
    [SerializeField] private Toggle economicToggle;
    [SerializeField] private Toggle ideologicalToggle;
    [SerializeField] private Toggle administrativeToggle;

    [Header("Severity Toggles")]
    [SerializeField] private Toggle minorToggle;
    [SerializeField] private Toggle standardToggle;
    [SerializeField] private Toggle aggravatedToggle;

    [Header("Stamp")]
    [SerializeField] private Button stampButton;
    [SerializeField] private Image stampVisual;

    [Header("Form Toggle")]
    [SerializeField] private Button toggleButton;
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private float slideSpeed = 6f;
    [SerializeField] private Vector2 hiddenPosition;
    [SerializeField] private Vector2 visiblePosition;

    #endregion

    #region Internal State

    private string revealedName = null;
    private string suspectName = null;
    private ViolationType? selectedViolation = null;
    private SeverityLevel? selectedSeverity = null;
    private bool stampApplied = false;

    private bool isVisible = false;
    private bool isSliding = false;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        // Disable name button initially
        nameFillButton.interactable = false;
        nameDisplayText.text = "";

        nameFillButton.onClick.AddListener(OnNameButtonClicked);

        // Violation listeners
        publicDisorderToggle.onValueChanged.AddListener((val) =>
            OnViolationSelected(val, ViolationType.PublicDisorder));

        economicToggle.onValueChanged.AddListener((val) =>
            OnViolationSelected(val, ViolationType.EconomicNonCompliance));

        ideologicalToggle.onValueChanged.AddListener((val) =>
            OnViolationSelected(val, ViolationType.IdeologicalDeviance));

        administrativeToggle.onValueChanged.AddListener((val) =>
            OnViolationSelected(val, ViolationType.AdministrativeObstruction));

        // Severity listeners
        minorToggle.onValueChanged.AddListener((val) =>
            OnSeveritySelected(val, SeverityLevel.Minor));

        standardToggle.onValueChanged.AddListener((val) =>
            OnSeveritySelected(val, SeverityLevel.Standard));

        aggravatedToggle.onValueChanged.AddListener((val) =>
            OnSeveritySelected(val, SeverityLevel.Aggravated));

        stampButton.onClick.AddListener(OnStampClicked);

        if (toggleButton != null)
            toggleButton.onClick.AddListener(ToggleForm);

        if (stampVisual != null)
            stampVisual.enabled = false;
    }

    private void Start()
    {
        if (rectTransform != null)
            rectTransform.anchoredPosition = hiddenPosition;
    }

    #endregion

    #region Yarn Integration

    /// <summary>
    /// Called by YarnManager to set the suspect name and enable the button.
    /// </summary>
    public void RevealNameFromYarn(string name)
    {
        revealedName = name;
        nameFillButton.interactable = true;
    }

    #endregion

    #region Name Logic

    private void OnNameButtonClicked()
    {
        if (string.IsNullOrEmpty(revealedName))
            return;

        // Fill the TMP_Text with the revealed name
        suspectName = revealedName;
        nameDisplayText.text = revealedName;

        // Disable the button so it can't be clicked again
        nameFillButton.interactable = false;
    }

    #endregion

    #region Violation & Severity

    private void OnViolationSelected(bool isOn, ViolationType type)
    {
        if (!isOn) return;
        selectedViolation = type;
    }

    private void OnSeveritySelected(bool isOn, SeverityLevel level)
    {
        if (!isOn) return;
        selectedSeverity = level;
    }

    #endregion

    #region Submission

    private void OnStampClicked()
    {
        stampApplied = true;

        if (stampVisual != null)
            stampVisual.enabled = true;

        if (IsFormComplete())
            SubmitForm();
        else
            Debug.LogWarning("Form incomplete.");
    }

    private bool IsFormComplete()
    {
        return
            !string.IsNullOrEmpty(suspectName) &&
            selectedViolation.HasValue &&
            selectedSeverity.HasValue;
    }

    private void SubmitForm()
    {
        InfractionFormData data = new InfractionFormData
        {
            caseID = caseID,
            suspectName = suspectName,
            violation = selectedViolation.Value,
            severity = selectedSeverity.Value,
            stamped = stampApplied
        };

        Debug.Log("FORM SUBMITTED:");
        Debug.Log(JsonUtility.ToJson(data, true));

        ResetForm();
    }

    #endregion

    #region Slide Logic

    public void ToggleForm()
    {
        if (isSliding) return;
        StartCoroutine(SlideForm(!isVisible));
    }

    private IEnumerator SlideForm(bool show)
    {
        isSliding = true;

        Vector2 start = rectTransform.anchoredPosition;
        Vector2 target = show ? visiblePosition : hiddenPosition;

        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime * slideSpeed;
            rectTransform.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        rectTransform.anchoredPosition = target;
        isVisible = show;
        isSliding = false;
    }

    #endregion

    #region Reset

    private void ResetForm()
    {
        revealedName = null;
        suspectName = null;
        selectedViolation = null;
        selectedSeverity = null;
        stampApplied = false;

        nameDisplayText.text = "";
        nameFillButton.interactable = false;

        publicDisorderToggle.isOn = false;
        economicToggle.isOn = false;
        ideologicalToggle.isOn = false;
        administrativeToggle.isOn = false;

        minorToggle.isOn = false;
        standardToggle.isOn = false;
        aggravatedToggle.isOn = false;

        if (stampVisual != null)
            stampVisual.enabled = false;
    }

    #endregion
}
