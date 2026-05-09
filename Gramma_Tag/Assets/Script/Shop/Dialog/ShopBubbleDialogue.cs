using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class ShopBubbleDialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueData
    {
        [TextArea(2, 5)]
        public string message;

        [Header("Animation Settings")]
        public float typingSpeed = 0.05f;

        [Header("Stay Duration")]
        public float stayDuration = 5f;

        [Header("Font")]
        public float fontSize = 36f;
    }

    [Header("UI")]
    public TextMeshProUGUI dialogueText;

    [Header("Dialogue List")]
    public DialogueData[] dialogues;

    [Header("Loop")]
    public bool loop = true;

    int currentIndex = 0;

    void Start()
    {
        if (dialogueText != null)
        {
            StartCoroutine(PlayDialogues());
        }
    }

    IEnumerator PlayDialogues()
    {
        while (true)
        {
            if (dialogues.Length == 0)
                yield break;

            DialogueData current = dialogues[currentIndex];

            // 🔥 APPLY FONT SIZE
            dialogueText.fontSize = current.fontSize;

            // 🔥 CLEAR TEXT
            dialogueText.text = "";

            // 🔥 TYPEWRITER EFFECT
            string currentText = "";

            foreach (char letter in current.message)
            {
                currentText += letter;

                dialogueText.SetText(currentText);

                yield return new WaitForSecondsRealtime(current.typingSpeed);
            }

            // 🔥 WAIT AFTER COMPLETE
            yield return new WaitForSecondsRealtime(current.stayDuration);

            // 🔥 NEXT
            currentIndex++;

            // 🔥 LOOP
            if (currentIndex >= dialogues.Length)
            {
                if (loop)
                {
                    currentIndex = 0;
                }
                else
                {
                    yield break;
                }
            }
        }
    }
}
