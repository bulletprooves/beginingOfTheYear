using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{
    // experience but, use for camera position. perhaps
    // [SerializeField]
    public Transform target; // 타겟 오브젝트 (하이러키에서 인스펙터의 컴포넌트로 드래그하여 옮기면 됨)
    public Vector3 offset; // 타겟 오브젝트로부터 떨어져있는 카메라의 위치
    public float smoothDamp; // 보간될 카메라의 움직임 속도? 숫자가 커질 수록 맥아리 없이 움직이는 카메라, 숫자가 작으면 딱딱하게 움직이는 카메라

    void FixedUpdate()
    {
        Vector3 dirPos = target.position + offset; // 타겟 오브젝트로부터 카메라가 떨어져있는 거리 보정
        Vector3 smoothPos = Vector3.Lerp(transform.position, dirPos, smoothDamp); //선형 보간법, linear interpolation 유니티에서 지원하는 Lerp함수 이용 (간략한 설명 : transform.position과  dirPos의 거리를 smoothDamp의 수치로 보간함)
        transform.position = smoothPos; // transform.position(카메라의 위치)를 smoothPos로 이동 함. 이 코드가 픽스드 업데이트 함수 안에 있으니까 매 프레임마다 카메라가 부드럽게 움직이는 효과
        transform.LookAt(target); // rotation은 움직이면 안되니까 LookAt함수로 코딩.
    }

    void Update()
    {
        
    }
}
