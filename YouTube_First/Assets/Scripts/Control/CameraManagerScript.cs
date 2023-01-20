using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManagerScript : MonoBehaviour
{
    // experience but, use for camera position. perhaps
    // [SerializeField]
    public Transform target; // Ÿ�� ������Ʈ (���̷�Ű���� �ν������� ������Ʈ�� �巡���Ͽ� �ű�� ��)
    public Vector3 offset; // Ÿ�� ������Ʈ�κ��� �������ִ� ī�޶��� ��ġ
    public float smoothDamp; // ������ ī�޶��� ������ �ӵ�? ���ڰ� Ŀ�� ���� �ƾƸ� ���� �����̴� ī�޶�, ���ڰ� ������ �����ϰ� �����̴� ī�޶�

    void FixedUpdate()
    {
        Vector3 dirPos = target.position + offset; // Ÿ�� ������Ʈ�κ��� ī�޶� �������ִ� �Ÿ� ����
        Vector3 smoothPos = Vector3.Lerp(transform.position, dirPos, smoothDamp); //���� ������, linear interpolation ����Ƽ���� �����ϴ� Lerp�Լ� �̿� (������ ���� : transform.position��  dirPos�� �Ÿ��� smoothDamp�� ��ġ�� ������)
        transform.position = smoothPos; // transform.position(ī�޶��� ��ġ)�� smoothPos�� �̵� ��. �� �ڵ尡 �Ƚ��� ������Ʈ �Լ� �ȿ� �����ϱ� �� �����Ӹ��� ī�޶� �ε巴�� �����̴� ȿ��
        transform.LookAt(target); // rotation�� �����̸� �ȵǴϱ� LookAt�Լ��� �ڵ�.
    }

    void Update()
    {
        
    }
}
