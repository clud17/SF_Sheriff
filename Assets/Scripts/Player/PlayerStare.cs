using UnityEngine;

/// <summary>
/// 캐릭터가 유저의 마우스를 따라 팔과 고개를 움직이는 역할을 하는 스크립트입니다.
/// made by 이용진
/// </summary>
public class PlayerStare : MonoBehaviour
{
    public Transform[] parts;
    public float[] retouchAngle;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        retouchAngle = new float[] {0,90};
    }
    void LateUpdate()
    {
        // 마우스 월드 좌표 가져오기
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // 좌우 반전 처리 (몸통 기준)
        if (mouseWorldPos.x < transform.position.x)
            transform.localScale = new Vector3(-0.55f, 0.55f, 0.55f); // 좌측 반전
        else
            transform.localScale = new Vector3(0.55f, 0.55f, 0.55f);  // 우측 정상

        // 각 파츠 회전 처리
        for (int i = 0; i < retouchAngle.Length; i++)
        {
            Vector3 dir = mouseWorldPos - parts[i].position;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            if (transform.localScale.x < 0)
                angle += 180f - retouchAngle[i];
            else
            {
                angle+= retouchAngle[i];
            }

            parts[i].rotation = Quaternion.Euler(0, 0, angle);
        }
    }

}
