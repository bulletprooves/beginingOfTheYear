using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // �����������̽� ����� �� �� �����ؾ� �Դ�

public class UserInterfaceManagerScript : MonoBehaviour
{
    public ControlForDesignatedObjectScript player; // Ŭ���� �ȿ� �ִ� ����� �����ؾ��Դ�!!!, ���� ���� ���̷�Ű�� �ִ� ���ΰ� ������Ʈ�� �ν����ͷ� �巡���� �ֽñ� �ٶ���

    public bool leftSignalForUnderneathScript, rightSignalForUnderneathScript;

    private void Awake()
    {
        leftSignalForUnderneathScript = false;
        rightSignalForUnderneathScript = false;
    }

    public void LeftArrowButtonDown()
    {
        player.leftButton = true;
        leftSignalForUnderneathScript = true;
    }
    public void LeftArrowButtonUp()
    {
        player.leftButton = false;
        leftSignalForUnderneathScript = false;
        player.leftRightButtonUpSignal = true;
    }
    public void RightArrowButtonDown()
    {
        player.rightButton = true;
        rightSignalForUnderneathScript = true;
    }
    public void RightArrowButtonUp()
    {
        player.rightButton = false;
        rightSignalForUnderneathScript = false;
        player.leftRightButtonUpSignal = true;
    }
}
