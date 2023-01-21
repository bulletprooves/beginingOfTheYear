using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckUnderneathScript : MonoBehaviour
{
    // ���ΰ� �� �Ʒ��� Ȯ�� �ϴ� �ڵ�!!! ������ �߰��� �Ҹ� �� �� �ʿ��մϴ�

    new AudioSource audio;
    public AudioClip[] stepDefaultSound;

    private GameObject uiSignal; // Ű���� ��ư ������ �� ����� �߰��� �Ҹ��� ui��ư ������ ���� ���� �ϱ� ���� ui ������Ʈ�� ����
    UserInterfaceManagerScript uiScript;

    public bool isOnGround; // ������ �ִ��� Ȯ��
    public float delayStepFrequency, delayStepFrequency_backUp; // �߰��� �Ҹ� �� �ʵ�, ���ڰ� ���� ���� ����� �� �Ҹ�, 0���� Ŀ�� ��


    void Start()
    {
        audio = GetComponent<AudioSource>();
        uiSignal = GameObject.FindGameObjectWithTag("Canvas"); // �±׷� ĵ���� ������Ʈ�� ã�� ����, ��ư�� ���ȴ��� ��ȣ�� Ȯ�� �� ��
        uiScript = uiSignal.GetComponent<UserInterfaceManagerScript>(); // �䷸�� ��� ĵ���� ������Ʈ �ȿ� �ִ� �����������̽��Ŵ��� ��ũ��Ʈ�� ������ �� �־��
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

    // private void OnTriggerEnter(Collider other) // Ʈ���ſ��� ��Ŵ� ���� �� �ٴڿ� ������ �� ȿ���� ���� �޼���

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
