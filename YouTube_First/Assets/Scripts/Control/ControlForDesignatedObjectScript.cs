using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlForDesignatedObjectScript : MonoBehaviour
{
    Rigidbody rb;
    // SpriteRenderer sr; (�÷��̾�� 2d ��������Ʈ�� �ʿ��� �ÿ� ����)
    Animator anim;

    [SerializeField]
    float speed; // ������Ʈ �̵� �ӵ�
    float acceleration; // ������Ʈ ���ӵ�
    public bool isObjMovingByInput; // ����Ű �Է¿� ���� �����̴��� Ȯ���ϱ� ���� �ο�
    private bool isSpeedVelocityStabilizing; // ���ǵ� ���ν�Ƽ�� ����ȭ�� �ϰ� �ִ°� Ȯ���ϱ� ���� �ο�
    public float stableBrake, stableBrake_BackUp; // �극��ũ �ӵ� (�ν����Ϳ��� ���� ����, �� 0���� ū ��������)
    public bool changeDirection; // ������Ʈ�� ������ �ٲ���� Ȯ���ϴ� �ο�
    public float accelPower; // ���� �ٲ� �� ���� ��� �� ���ϰ� �����̴� ��, speed ���� ���� Ŀ����
    float delayAccelChangeDirection; // ���ӵ� ���ϴ� �ð� ()
    public float maxSpeedVelocity_x; // x���� �ְ� �ӵ� ����
    public float currentVelocity_x; // ���� x������ �ӵ� (Ȯ�ο�)
    public bool isRight; // �ʱⰪ�� �������� �����ִ� �ͷ� �ϰ���. isRight = true;

    void Awake()
    {
        
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // sr = GetComponent<SpriteRenderer>(); (2d �̿� �ÿ� ����)
        anim = GetComponent<Animator>();

        changeDirection = false;
        isObjMovingByInput = false;
        isSpeedVelocityStabilizing = false;
        stableBrake_BackUp = stableBrake; // ������ �����̺�극��ũ ���� ���� ����
        delayAccelChangeDirection = 0.25f;
        isRight = true;
    }

    private void SpeedVelocityLimit() // �̵��ӵ� ���� �޼���(�Լ�)
    {
        if (rb.velocity.x > maxSpeedVelocity_x)
        {
            rb.velocity = new Vector3(maxSpeedVelocity_x, rb.velocity.y, rb.velocity.z);
        }
        if (rb.velocity.x < (maxSpeedVelocity_x * -1))
        {
            rb.velocity = new Vector3((maxSpeedVelocity_x * -1), rb.velocity.y, rb.velocity.z);
        }
    }

    void FixedUpdate() // �Ƚ��������Ʈ, ������ �ð��� ���� �������� �����ϹǷ� ��Ģ���� ������ ����������, ���õǴ� ������ ���ɼ� ����. (�� ���� Ȧ�� �ؾ��ϴ� ��ư, ������ �ð� ����)
    {
        currentVelocity_x = rb.velocity.x; // ���� x������ �ӵ� (Ȯ�ο�)
        if (isObjMovingByInput)
        {
            SpeedVelocityLimit(); // ���� �ӵ� �ѱ��� �ʰ� �� �����Ӹ��� SpeedVelocityLimit �޼��带 ȣ��
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isObjMovingByInput = true; // ������Ʈ�� �Է¿� ���� �����̴� ��. �ο� : ��
            isSpeedVelocityStabilizing = false; // �Է¿� ���� �����̴� ���̱� ������ ����ȭ�� : ����          
            if (changeDirection) // ������ �ٲ����?
                rb.AddForce(accelPower * -1, 0, 0, ForceMode.Force); // ���� �ؾ��ϹǷ� ���� �ٲ��� ��, ���� ��� ���� ������ �̵�
            else
                rb.AddForce(-1f * speed, 0, 0, ForceMode.Force); // �⺻ �ӵ�, �ν����Ϳ��� speed�� ���� ����
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            isObjMovingByInput = true;
            isSpeedVelocityStabilizing = false;
            if (changeDirection)
                rb.AddForce(accelPower, 0, 0, ForceMode.Force);
            else
                rb.AddForce(1f * speed, 0, 0, ForceMode.Force);
        }
    }

    void Update() // ������Ʈ �Լ�, ��� ������ ���� �����ϱ� ������ ���õǴ� ������ ����. (�ܹ����� �Է��� �޼��� Ȥ�� ��ư)
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // ����
        {
            if (isRight)
            {
                isRight = false;
                if (!(changeDirection))
                    changeDirection = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // ������
        {
            if (!(isRight))
            {
                isRight = true;
                if (!(changeDirection))
                    changeDirection = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) // ����Ű���� ���� ���� �� �Ͼ �ڵ�
        {
            isObjMovingByInput = false; // �Է¿� ���� �����̴� ������Ʈ�� : ����
            isSpeedVelocityStabilizing = true; // ���ǵ座�ν�Ƽ����ȭ : ��
        }

        if (isObjMovingByInput) // �̲������� ����(���º����¡)�� �ٽ� �����̸� �극��ũ ���� �ʱⰪ����
            stableBrake = stableBrake_BackUp;
        if (isSpeedVelocityStabilizing) // ���ǵ座�ν�Ƽ ����ȭ�� ���� ��, �극��ũ �ʵ�(����)�� �̿��Ͽ� �̲����� ����
        {
            if (stableBrake > 0f)
            {
                stableBrake -= Time.deltaTime;
                rb.velocity = new Vector3((isRight) ? stableBrake : (stableBrake * -1), rb.velocity.y, rb.velocity.z); // ���׿����ڷ� ��� ����(��, ��)���� �̲���������
                if (stableBrake <= 0f)
                {
                    stableBrake = stableBrake_BackUp;
                    isSpeedVelocityStabilizing = false;
                }
            }
        }

        if (changeDirection)
        {
            if (delayAccelChangeDirection > 0f)
            {
                delayAccelChangeDirection -= Time.deltaTime;
                if (delayAccelChangeDirection <= 0f)
                {
                    delayAccelChangeDirection = 0.25f;
                    changeDirection = false;
                }
            }
        }
    }

}
