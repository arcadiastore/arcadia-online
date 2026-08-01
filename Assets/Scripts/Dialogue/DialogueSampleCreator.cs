using UnityEngine;
using System.Collections.Generic;
using System.IO;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ArcadiaOnline.Dialogue
{
    /// <summary>
    /// Auto-create sample dialogues.
    /// Attach ke GameObject lalu klik checkbox.
    /// </summary>
    public class DialogueSampleCreator : MonoBehaviour
    {
        [Header("Create Samples")]
        [SerializeField] private bool createSamples;

        [Header("NPC Settings")]
        [SerializeField] private string npcName = "Village Chief";

        void OnValidate()
        {
            if (createSamples)
            {
                createSamples = false;
                CreateAllSamples();
            }
        }

        /// <summary>
        /// Create semua sample dialogues.
        /// </summary>
        private void CreateAllSamples()
        {
            // Create folder
            string folderPath = "Assets/Resources/Dialogues";
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Debug.Log($"[DialogueSample] Created folder: {folderPath}");
            }

            // Create sample dialogues
            CreateVillageChiefDialogue();
            CreateShopkeeperDialogue();
            CreateQuestGiverDialogue();
            CreateElderDialogue();

            Debug.Log("[DialogueSample] All sample dialogues created!");
        }

        /// <summary>
        /// Village Chief dialogue.
        /// </summary>
        private void CreateVillageChiefDialogue()
        {
            DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
            dialogue.dialogueID = "village_chief_intro";
            dialogue.npcName = "Village Chief";
            dialogue.requiredLevel = 1;

            dialogue.lines = new List<DialogueLine>();

            // Line 1
            DialogueLine line1 = new DialogueLine();
            line1.speakerName = "Village Chief";
            line1.text = "Selamat datang di Arcadia Village, petualang muda!";
            line1.type = DialogueType.Normal;
            dialogue.lines.Add(line1);

            // Line 2
            DialogueLine line2 = new DialogueLine();
            line2.speakerName = "Village Chief";
            line2.text = "Desa kita sedang dalam bahaya. Monster-monster dari hutan semakin berani menyerang!";
            line2.type = DialogueType.Normal;
            dialogue.lines.Add(line2);

            // Line 3
            DialogueLine line3 = new DialogueLine();
            line3.speakerName = "Village Chief";
            line3.text = "Apakah kamu bersedia membantu kami?";
            line3.type = DialogueType.Choice;
            line3.choices = new List<DialogueChoice>();

            DialogueChoice choice1 = new DialogueChoice();
            choice1.choiceText = "Tentu, saya akan membantu!";
            choice1.nextDialogueID = "village_chief_quest_accept";
            choice1.triggerEvent = "quest_accept";
            line3.choices.Add(choice1);

            DialogueChoice choice2 = new DialogueChoice();
            choice2.choiceText = "Maaf, saya belum siap.";
            choice2.nextDialogueID = "village_chief_quest_decline";
            choice2.triggerEvent = "";
            line3.choices.Add(choice2);

            dialogue.lines.Add(line3);

            // Line 4 (after accept)
            DialogueLine line4 = new DialogueLine();
            line4.speakerName = "Village Chief";
            line4.text = "Terima kasih! Pergilah ke Green Forest dan kalahkan 5 Slime. Hadiahnya akan sepadan!";
            line4.type = DialogueType.Normal;
            line4.triggerEvent = "quest_kill_slime";
            dialogue.lines.Add(line4);

            // Save
            SaveDialogue(dialogue, "VillageChief_Intro");
        }

        /// <summary>
        /// Shopkeeper dialogue.
        /// </summary>
        private void CreateShopkeeperDialogue()
        {
            DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
            dialogue.dialogueID = "shopkeeper_greeting";
            dialogue.npcName = "Shopkeeper";
            dialogue.requiredLevel = 1;

            dialogue.lines = new List<DialogueLine>();

            // Line 1
            DialogueLine line1 = new DialogueLine();
            line1.speakerName = "Shopkeeper";
            line1.text = "Hei, selamat datang di tokoku! Ada yang bisa kubantu?";
            line1.type = DialogueType.Choice;
            line1.choices = new List<DialogueChoice>();

            DialogueChoice choice1 = new DialogueChoice();
            choice1.choiceText = "Lihat barang dagangan";
            choice1.nextDialogueID = "";
            choice1.triggerEvent = "open_shop";
            line1.choices.Add(choice1);

            DialogueChoice choice2 = new DialogueChoice();
            choice2.choiceText = "Tidak, terima kasih";
            choice2.nextDialogueID = "";
            choice2.triggerEvent = "";
            line1.choices.Add(choice2);

            dialogue.lines.Add(line1);

            // Save
            SaveDialogue(dialogue, "Shopkeeper_Greeting");
        }

        /// <summary>
        /// Quest giver dialogue.
        /// </summary>
        private void CreateQuestGiverDialogue()
        {
            DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
            dialogue.dialogueID = "quest_giver_hunter";
            dialogue.npcName = "Hunter";
            dialogue.requiredLevel = 5;

            dialogue.lines = new List<DialogueLine>();

            // Line 1
            DialogueLine line1 = new DialogueLine();
            line1.speakerName = "Hunter";
            line1.text = "Kau terlihat seperti petualang berpengalaman...";
            line1.type = DialogueType.Normal;
            dialogue.lines.Add(line1);

            // Line 2
            DialogueLine line2 = new DialogueLine();
            line2.speakerName = "Hunter";
            line2.text = "Aku punya misi berbahaya. Di dalam hutan, ada Wolf Alpha yang sangat kuat.";
            line2.type = DialogueType.Normal;
            dialogue.lines.Add(line2);

            // Line 3
            DialogueLine line3 = new DialogueLine();
            line3.speakerName = "Hunter";
            line3.text = "Kalahkan dia, dan aku akan memberimu senjata langka!";
            line3.type = DialogueType.Choice;
            line3.choices = new List<DialogueChoice>();

            DialogueChoice choice1 = new DialogueChoice();
            choice1.choiceText = "Saya terima tantangan itu!";
            choice1.nextDialogueID = "quest_hunter_accept";
            choice1.triggerEvent = "quest_hunter_accept";
            line3.choices.Add(choice1);

            DialogueChoice choice2 = new DialogueChoice();
            choice2.choiceText = "Saya belum cukup kuat...";
            choice2.nextDialogueID = "";
            choice2.triggerEvent = "";
            line3.choices.Add(choice2);

            dialogue.lines.Add(line3);

            // Save
            SaveDialogue(dialogue, "QuestGiver_Hunter");
        }

        /// <summary>
        /// Elder dialogue.
        /// </summary>
        private void CreateElderDialogue()
        {
            DialogueData dialogue = ScriptableObject.CreateInstance<DialogueData>();
            dialogue.dialogueID = "elder_wisdom";
            dialogue.npcName = "Village Elder";
            dialogue.requiredLevel = 1;

            dialogue.lines = new List<DialogueLine>();

            // Line 1
            DialogueLine line1 = new DialogueLine();
            line1.speakerName = "Village Elder";
            line1.text = "Ah, petualang baru... Dengarkan nasihatku.";
            line1.type = DialogueType.Normal;
            dialogue.lines.Add(line1);

            // Line 2
            DialogueLine line2 = new DialogueLine();
            line2.speakerName = "Village Elder";
            line2.text = "Di Arcadia, kekuatan bukan segalanya. Persahabatan dan strategi juga penting.";
            line2.type = DialogueType.Normal;
            dialogue.lines.Add(line2);

            // Line 3
            DialogueLine line3 = new DialogueLine();
            line3.speakerName = "Village Elder";
            line3.text = "Gunakan equipment yang tepat, dan jangan lupa untuk mengisi HP-mu!";
            line3.type = DialogueType.Normal;
            dialogue.lines.Add(line3);

            // Line 4
            DialogueLine line4 = new DialogueLine();
            line4.speakerName = "Village Elder";
            line4.text = "Sekarang pergilah, dan buktikan keberanianmu!";
            line4.type = DialogueType.Normal;
            line4.triggerEvent = "tutorial_complete";
            dialogue.lines.Add(line4);

            // Save
            SaveDialogue(dialogue, "Elder_Wisdom");
        }

        /// <summary>
        /// Save dialogue as ScriptableObject asset.
        /// </summary>
        private void SaveDialogue(DialogueData dialogue, string fileName)
        {
#if UNITY_EDITOR
            string path = $"Assets/Resources/Dialogues/{fileName}.asset";
            AssetDatabase.CreateAsset(dialogue, path);
            Debug.Log($"[DialogueSample] Created: {path}");
#else
            Debug.Log($"[DialogueSample] {fileName} created (runtime mode)");
#endif
        }
    }
}
