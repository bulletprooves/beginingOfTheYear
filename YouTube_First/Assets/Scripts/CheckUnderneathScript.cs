using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUnderneathScript : MonoBehaviour
{
    // 주인공 발 아래를 확인 하는 코드!!! 점프나 발걸음 소리 낼 때 필요합니당

    new AudioSource audio;
    public AudioClip[] stepDefaultSound;

    private GameObject uiSignal; // 키보드 버튼 눌렀을 때 생기는 발걸음 소리를 ui버튼 눌렀을 때도 나게 하기 위해 ui 오브젝트에 접근
    UserInterfaceManagerScript uiScript;

    public bool isOnGround; // 땅위에 있는지 확인
    public float delayStepFrequency, delayStepFrequency_backUp; // 발걸음 소리 빈도 필드, 숫자가 작을 수록 빈번한 발 소리, 0보다 커야 함


    void Start()
    {
        audio = GetComponent<AudioSource>();
        uiSignal = GameObject.FindGameObjectWithTag("Canvas"); // 태그로 캔버스 오브젝트를 찾은 다음, 버튼이 눌렸는지 신호를 확인 할 셈
        uiScript = uiSignal.GetComponent<UserInterfaceManagerScript>(); // 요렇게 써야 캔버스 오브젝트 안에 있는 유저인터페이스매니저 스크립트를 가져올 수 있어용
        delayStepFrequency_backUp = delayStepFrequency;
    }

    public void PlayStepSound()
    {
        int rand = Random.Range(0, 4);
        audio.PlayOneShot(stepDefaultSound[rand]);
    }

    void FixedUpdate()
    {
        if (isOnGround)
        {
            if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.RightArrow) || (uiScript.leftSignalForUnderneathScript || uiScript.rightSignalForUnderneathScript))
            {
                if (delayStepFrequency > 0)
                {
                    delayStepFrequency -= Time.deltaTime;
                    if (delayStepFrequency <= 0)
                    {
                        PlayStepSound();
                        delayStepFrequency = delayStepFrequency_backUp;
                    }
                }
            }
            else
                delayStepFrequency = delayStepFrequency_backUp;
        }
    }

    void Update()
    {
    
    }

    // private void OnTriggerEnter(Collider other) // 트리거엔터 요거는 점프 후 바닥에 착지할 때 효과를 위한 메서드

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag.Equals("Ground"))
        {
            isOnGround = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag.Equals("Ground"))
        {
            isOnGround = false;
        }
    }
}
