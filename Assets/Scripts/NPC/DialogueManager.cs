using System.Collections;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.UI;
using UnityEngine.InputSystem; // InputSystem 사용을 위해 추가

public class DialogueManager : MonoBehaviour
{
    private static DialogueManager _instance;
    public static DialogueManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<DialogueManager>();
                if (_instance == null)
                {
                    Debug.LogError("DialogueManager 인스턴스 없음");
                }
            }
            return _instance;
        }
    }

    [Header("UI 연결")]
    [SerializeField] private GameObject speechBubblePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private Image speechBubbleImage;
    // Next Button 변수 삭제됨

    private Dialogue currentDialogue;
    private Transform speakingObjectTransform;
    private Action onDialogueEndCallback;
    private IEnumerator typeSentenceCoroutine;

    // 현재 대화가 활성화되었는지 확인하는 변수
    private bool isDialogueActive = false;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    // 매 프레임마다 호출되어 입력 감지
    private void Update()
    {
        // 대화창이 활성화된 상태에서만 입력 처리
        if (isDialogueActive)
        {
            // F키, 스페이스바, 또는 마우스 왼쪽 버튼 입력 감지
            if (Keyboard.current.fKey.wasPressedThisFrame ||
                Keyboard.current.spaceKey.wasPressedThisFrame ||
                Mouse.current.leftButton.wasPressedThisFrame)
            {
                DisplayNextSentence();
            }
        }
    }

    public void StartDialogue(Dialogue dialogueData, Transform speakerTransform, Action onEnd = null)
    {
        currentDialogue = dialogueData;
        speakingObjectTransform = speakerTransform;
        onDialogueEndCallback = onEnd;

        currentDialogue.ResetDialogue();
        speechBubblePanel.SetActive(true);
        isDialogueActive = true; // 대화 활성화 상태로 변경

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (typeSentenceCoroutine != null)
        {
            StopCoroutine(typeSentenceCoroutine);
        }

        if (currentDialogue.IsDialogueComplete())
        {
            EndDialogue();
            return;
        }

        Dialogue.DialogueStep step = currentDialogue.GetNextDialogue();
        typeSentenceCoroutine = TypeSentence(step);
        StartCoroutine(typeSentenceCoroutine);
    }

    private IEnumerator TypeSentence(Dialogue.DialogueStep step)
    {
        if (step.speechBubbleSprite != null)
        {
            speechBubbleImage.sprite = step.speechBubbleSprite;
        }

        dialogueText.text = "";

        foreach (char letter in step.text.ToCharArray())
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(step.typingSpeed);
        }
    }

    private void EndDialogue()
    {
        speechBubblePanel.SetActive(false);
        isDialogueActive = false; // 대화 비활성화 상태로 변경
        onDialogueEndCallback?.Invoke();
    }
}