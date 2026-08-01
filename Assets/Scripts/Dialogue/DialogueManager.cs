using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

namespace ArcadiaOnline.Dialogue
{
    /// <summary>
    /// Manager untuk dialogue system.
    /// </summary>
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("UI References")]
        [SerializeField] private GameObject dialoguePanel;
        [SerializeField] private Text speakerNameText;
        [SerializeField] private Text dialogueText;
        [SerializeField] private Image portraitImage;
        [SerializeField] private GameObject portraitPanel;
        [SerializeField] private Button continueButton;
        [SerializeField] private GameObject choicePanel;
        [SerializeField] private Transform choiceButtonParent;
        [SerializeField] private GameObject choiceButtonPrefab;

        [Header("Typewriter Settings")]
        [SerializeField] private float typewriterSpeed = 0.05f;
        [SerializeField] private bool useTypewriter = true;

        // State
        private DialogueData currentDialogue;
        private int currentLineIndex;
        private bool isDialogueActive;
        private bool isTyping;
        private string fullText;

        // Player reference
        private MonoBehaviour playerController;

        // Events
        public System.Action<string> OnDialogueEvent;
        public System.Action OnDialogueEnd;

        /// <summary>
        /// Set UI references from DialogueUI.
        /// </summary>
        public void SetUIReferences(GameObject panel, Text speaker, Text dialogue,
            Image portrait, GameObject portraitPnl, Button continueBtn,
            GameObject choicePnl, Transform choiceParent)
        {
            dialoguePanel = panel;
            speakerNameText = speaker;
            dialogueText = dialogue;
            portraitImage = portrait;
            portraitPanel = portraitPnl;
            continueButton = continueBtn;
            choicePanel = choicePnl;
            choiceButtonParent = choiceParent;
        }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            // Setup continue button
            if (continueButton != null)
            {
                continueButton.onClick.AddListener(OnContinueClicked);
            }

            // Find player controller (any script with "PlayerController" in name)
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                // Try to find player controller
                MonoBehaviour[] scripts = player.GetComponents<MonoBehaviour>();
                foreach (MonoBehaviour script in scripts)
                {
                    if (script.GetType().Name.Contains("PlayerController"))
                    {
                        playerController = script;
                        break;
                    }
                }
            }

            // Hide dialogue panel
            HideDialogue();
        }

        void Update()
        {
            if (!isDialogueActive) return;

            // Skip typewriter dengan klik
            if (isTyping && Input.GetMouseButtonDown(0))
            {
                SkipTypewriter();
                return;
            }

            // Continue dengan klik atau Enter
            if (!isTyping && (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Return)))
            {
                OnContinueClicked();
            }
        }

        /// <summary>
        /// Mulai dialogue.
        /// </summary>
        public void StartDialogue(DialogueData dialogue)
        {
            if (dialogue == null) return;

            currentDialogue = dialogue;
            currentLineIndex = 0;
            isDialogueActive = true;

            // Disable player movement
            if (playerController != null)
            {
                playerController.enabled = false;
            }

            // Show first line
            ShowLine(currentLineIndex);

            Debug.Log($"[Dialogue] Started: {dialogue.dialogueID}");
        }

        /// <summary>
        /// Show specific line.
        /// </summary>
        private void ShowLine(int index)
        {
            if (currentDialogue == null || index >= currentDialogue.GetLineCount())
            {
                EndDialogue();
                return;
            }

            DialogueLine line = currentDialogue.GetLine(index);
            if (line == null)
            {
                EndDialogue();
                return;
            }

            // Show dialogue panel
            ShowDialogue();

            // Set speaker name
            if (speakerNameText != null)
            {
                speakerNameText.text = line.speakerName;
            }

            // Set portrait
            if (portraitPanel != null)
            {
                if (line.portrait != null)
                {
                    portraitPanel.SetActive(true);
                    if (portraitImage != null)
                    {
                        portraitImage.sprite = line.portrait;
                    }
                }
                else
                {
                    portraitPanel.SetActive(false);
                }
            }

            // Set dialogue text
            fullText = line.text;

            if (useTypewriter)
            {
                // Start typewriter effect
                isTyping = true;
                if (dialogueText != null)
                {
                    dialogueText.text = "";
                    StartCoroutine(TypewriterEffect());
                }
            }
            else
            {
                if (dialogueText != null)
                {
                    dialogueText.text = fullText;
                }
            }

            // Handle choices
            if (line.type == DialogueType.Choice && line.choices != null && line.choices.Count > 0)
            {
                ShowChoices(line.choices);
            }
            else
            {
                HideChoices();
            }

            // Trigger event
            if (!string.IsNullOrEmpty(line.triggerEvent))
            {
                OnDialogueEvent?.Invoke(line.triggerEvent);
            }
        }

        /// <summary>
        /// Typewriter effect.
        /// </summary>
        private System.Collections.IEnumerator TypewriterEffect()
        {
            for (int i = 0; i <= fullText.Length; i++)
            {
                dialogueText.text = fullText.Substring(0, i);
                yield return new WaitForSeconds(typewriterSpeed);
            }

            isTyping = false;
        }

        /// <summary>
        /// Skip typewriter.
        /// </summary>
        private void SkipTypewriter()
        {
            StopAllCoroutines();
            if (dialogueText != null)
            {
                dialogueText.text = fullText;
            }
            isTyping = false;
        }

        /// <summary>
        /// Show choices.
        /// </summary>
        private void ShowChoices(List<DialogueChoice> choices)
        {
            if (choicePanel == null || choiceButtonPrefab == null || choiceButtonParent == null)
                return;

            // Clear existing choices
            foreach (Transform child in choiceButtonParent)
            {
                Destroy(child.gameObject);
            }

            // Show choice panel
            choicePanel.SetActive(true);

            // Create choice buttons
            foreach (DialogueChoice choice in choices)
            {
                GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceButtonParent);
                Button button = buttonObj.GetComponent<Button>();
                Text buttonText = buttonObj.GetComponentInChildren<Text>();

                if (buttonText != null)
                {
                    buttonText.text = choice.choiceText;
                }

                if (button != null)
                {
                    string nextID = choice.nextDialogueID;
                    string triggerEvent = choice.triggerEvent;

                    button.onClick.AddListener(() =>
                    {
                        // Hide choices
                        choicePanel.SetActive(false);

                        // Trigger event
                        if (!string.IsNullOrEmpty(triggerEvent))
                        {
                            OnDialogueEvent?.Invoke(triggerEvent);
                        }

                        // Go to next dialogue
                        if (!string.IsNullOrEmpty(nextID))
                        {
                            // TODO: Load next dialogue by ID
                            Debug.Log($"[Dialogue] Next: {nextID}");
                        }

                        // Continue
                        ContinueDialogue();
                    });
                }
            }
        }

        /// <summary>
        /// Hide choices.
        /// </summary>
        private void HideChoices()
        {
            if (choicePanel != null)
            {
                choicePanel.SetActive(false);
            }

            // Clear buttons
            if (choiceButtonParent != null)
            {
                foreach (Transform child in choiceButtonParent)
                {
                    Destroy(child.gameObject);
                }
            }
        }

        /// <summary>
        /// Continue to next line.
        /// </summary>
        public void ContinueDialogue()
        {
            if (isTyping)
            {
                SkipTypewriter();
                return;
            }

            currentLineIndex++;
            ShowLine(currentLineIndex);
        }

        /// <summary>
        /// End dialogue.
        /// </summary>
        public void EndDialogue()
        {
            isDialogueActive = false;
            currentDialogue = null;
            currentLineIndex = 0;

            // Enable player movement
            if (playerController != null)
            {
                playerController.enabled = true;
            }

            // Hide dialogue panel
            HideDialogue();

            // Callback
            OnDialogueEnd?.Invoke();

            Debug.Log("[Dialogue] Ended");
        }

        /// <summary>
        /// Show dialogue panel.
        /// </summary>
        private void ShowDialogue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(true);
            }
        }

        /// <summary>
        /// Hide dialogue panel.
        /// </summary>
        private void HideDialogue()
        {
            if (dialoguePanel != null)
            {
                dialoguePanel.SetActive(false);
            }

            HideChoices();
        }

        /// <summary>
        /// Cek apakah dialogue sedang aktif.
        /// </summary>
        public bool IsDialogueActive()
        {
            return isDialogueActive;
        }

        /// <summary>
        /// On continue button clicked.
        /// </summary>
        private void OnContinueClicked()
        {
            ContinueDialogue();
        }
    }
}
