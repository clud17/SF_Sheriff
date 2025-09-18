using UnityEngine;
using UnityEngine.InputSystem;

public class BasicNPC : MonoBehaviour
{
    // NPC가 플레이어와 나누는 주 대화 데이터
    [SerializeField] private Dialogue mainDialogueData;

    // 플레이어가 NPC의 상호작용 범위 내에 있는지 추적하는 변수
    private bool isPlayerInRange = false;
    // 현재 대화가 진행 중인지 확인하는 변수
    private bool isDialogueActive = false;

    private void Update()
    {
        // 플레이어가 범위 안에 있고, F키를 누르며, 현재 대화 중이 아닐 때만 대화 시작
        if (isPlayerInRange && Keyboard.current.fKey.wasPressedThisFrame && !isDialogueActive)
        {
            StartDialogue();
        }
    }

    private void StartDialogue()
    {
        // 대화 시작 상태로 전환
        isDialogueActive = true;

        // DialogueManager를 통해 주 대화 시작. 대화가 끝나면 호출될 콜백 함수를 전달
        DialogueManager.Instance.StartDialogue(mainDialogueData, transform, OnDialogueEnd);
    }

    // 대화가 끝났을 때 DialogueManager에서 호출해 줄 콜백 함수
    private void OnDialogueEnd()
    {
        // 대화 종료 상태로 전환
        isDialogueActive = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 트리거에 들어온 오브젝트의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어가 범위에 들어왔음을 기록
            isPlayerInRange = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 트리거에서 나간 오브젝트의 태그가 "Player"인지 확인
        if (other.CompareTag("Player"))
        {
            // 플레이어가 범위에서 벗어났음을 기록
            isPlayerInRange = false;
        }
    }
}