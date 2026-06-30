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
    Coroutine dialogueCoroutine;

    void OnEnable()
    {
        if (dialogueText == null) return;
        if (dialogues == null || dialogues.Length == 0) return;

        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
        }

        dialogueCoroutine = StartCoroutine(PlayDialogues());
    }

    void OnDisable()
    {
        if (dialogueCoroutine != null)
        {
            StopCoroutine(dialogueCoroutine);
            dialogueCoroutine = null;
        }

        if (dialogueText != null)
        {
            dialogueText.text = "";
        }
    }

    IEnumerator PlayDialogues()
    {
        while (true)
        {
            DialogueData current = dialogues[currentIndex];

            dialogueText.fontSize = current.fontSize;
            dialogueText.text = "";

            string currentText = "";

            foreach (char letter in current.message)
            {
                currentText += letter;
                dialogueText.SetText(currentText);

                yield return new WaitForSecondsRealtime(current.typingSpeed);
            }

            yield return new WaitForSecondsRealtime(current.stayDuration);

            currentIndex++;

            if (currentIndex >= dialogues.Length)
            {
                if (loop)
                {
                    currentIndex = 0;
                }
                else
                {
                    dialogueCoroutine = null;
                    yield break;
                }
            }
        }
    }
}
