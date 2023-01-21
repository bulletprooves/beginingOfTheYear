using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; // 유저인터페이스 사용할 때 꼭 기입해야 함당

public class UserInterfaceManagerScript : MonoBehaviour
{
    public ControlForDesignatedObjectScript player; // 클래스 안에 있는 멤버에 접근해야함당!!!, 현재 씬의 하이러키에 있는 주인공 오브젝트를 인스펙터로 드래그해 주시기 바람당

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
