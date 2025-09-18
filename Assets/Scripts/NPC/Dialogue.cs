using UnityEngine;
using System;

[CreateAssetMenu(fileName = "New Dialogue", menuName = "Dialogue/Dialogue Data")]
public class Dialogue : ScriptableObject
{
    [Serializable]
    public class DialogueStep
    {
        [TextArea(3, 10)] // 인스펙터 창에서 여러 줄 입력 가능
        public string text; // 대사
        public float typingSpeed = 0.05f; // 글자 출력 속도

        // 여기에 말풍선 배경 스프라이트 변수 추가
        public Sprite speechBubbleSprite; 
    }


    public DialogueStep[] dialogueSteps;
    private int currentStepIndex = 0;


    // 다음 대사 단계
    public DialogueStep GetNextDialogue()
    {
        if (currentStepIndex >= dialogueSteps.Length)
        {
            return null;  //대화 끝일때
        }
        return dialogueSteps[currentStepIndex++];
    }

    // 대화 단계를 초기화
    public void ResetDialogue()
    {
        currentStepIndex = 0;
    }

    // 대화가 끝났는지 확인
    public bool IsDialogueComplete()
    {
        return currentStepIndex >= dialogueSteps.Length;
    }
}