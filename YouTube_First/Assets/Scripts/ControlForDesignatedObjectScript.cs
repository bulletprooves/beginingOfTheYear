using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlForDesignatedObjectScript : MonoBehaviour
{
    // �Ϸ��α�
    // ���� ������Ʈ�� �ܺ� ���ο� ���� transform�� �̵��ؾ� �Ѵٸ� ���� Ŭ���� ���� �ִ� �Ϻ��ڵ�, Ư�� transform �� ���������� �ǵ帮�� ���� ���� Ȯ���ؾ� ��.
    // �Է¿� ���� �������� ������ٵ��� velocity�� �߱� ������ ū ������ ���� �� ����

    Rigidbody rb;
    // SpriteRenderer sr; (�÷��̾�� 2d ��������Ʈ�� �ʿ��� �ÿ� ���, ���� ���߿� �Ǽ��縮�� Ŀ���͸���¡�� ���� ��쿡�� �ڽ� ������Ʈ�� Ŭ������ ��ӽ�Ű�� ���� ����)
    Animator anim;

    public bool leftRightButtonUpSignal; // Ű���� ��ư���� ���� ���� ���� ui��ư ���� ���� ���� ���� �۵� ����� �޶�,  isSpeedVelocityStabilizing ������ critical section (�Ӱ迵��) ������ �ذ��ϱ� ���� ����.
    public bool leftButton, rightButton; // static(����) �ʵ�(����)�� Ŭ���� �ٷ� ���� ����Ǳ� ������, ���� �� ��ũ��Ʈ�� ���� �ִ� ������Ʈ�� ���۵Ǳ⵵ ���� �ٸ� 
                                         //��ũ��Ʈ���� ������ �� ����. �ʿ��ϴٸ� public static bool �� Ÿ���� �ٲ��ְ� UI�Ŵ��� ��ũ��Ʈ�ּ� ���� ������ ���� ����� ��.
                                         //�߰� + ��ư�� ���ÿ� ������ ���� Exception(����) �� �ڵ� �ؾ��� �� ����. �ؾ��Ѵٸ� else if �� �ڵ带 �߰��ؾ� ��.

    [SerializeField]
    float speed; // ������Ʈ �̵� �ӵ�
    float acceleration; // ������Ʈ ���ӵ�
    bool isObjMovingByInput; // ����Ű �Է¿� ���� �����̴��� Ȯ���ϱ� ���� �ο�
    bool isSpeedVelocityStabilizing; // ���ǵ� ���ν�Ƽ�� ����ȭ�� �ϰ� �ִ°� Ȯ���ϱ� ���� �ο�
    public float stableBrake, stableBrake_backUp; // �극��ũ �ӵ� (�ν����Ϳ��� ���� ����, �� 0���� ū ��������)
    public bool changeDirection; // ������Ʈ�� ������ �ٲ���� Ȯ���ϴ� �ο�
    public float accelPower; // ���� �ٲ� �� ���� ��� �� ���ϰ� �����̴� ��, speed ���� ���� Ŀ����
    float delayAccelChangeDirection; // ���ӵ� ���ϴ� �ð� (accelPower�� �ڵ尡 ����Ǵ� ���� ª�� ������)
    public float maxSpeedVelocity_x; // x���� �ְ� �ӵ� ����
    public float currentVelocity_x; // ���� x������ �ӵ� (Ȯ�ο�)
    float delayAbsoluteZeroSpeed; // ���ǵ尡 0 �� �Ǳ����� ������ (������Ʈ�Լ��� �Ƚ��������Ʈ�Լ��� �Բ� ���Ǵ� �Ӱ迵��(critical section)�ڵ� ���̷� �Ͼ�� �ణ�� ������ �����ϱ� ���ظ���.)
    public bool isRight; // �ʱⰪ�� �������� �����ִ� �ͷ� �ϰ���. isRight = true;
    public bool isWalking; // �����̴� �� ����?

    void Awake()
    {
        leftRightButtonUpSignal = false;
        leftButton = false;
        rightButton = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // sr = GetComponent<SpriteRenderer>(); // 2d �̿� �ÿ� ���� �׸��� (2�� �̻�)�ڽİ�ü ��������Ʈ�� �����Ͼ��Ѵٸ� �Ʒ��ڵ� ����
        // SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>(true) // �� �ڵ带 �ʿ��� �޼��忡 ���ְ� for(int i = 0; i < rends.Length; i++) ó�� �ݺ������� �����ϸ� ����. ����) for(int i = 0; i < rends.Length; i++) rends.color = new Color(1, 1, 1, 0.5f); ������ �ϸ� �������� ��������Ʈ ���� �� ����
        anim = GetComponent<Animator>();

        changeDirection = false;
        isObjMovingByInput = false;
        isSpeedVelocityStabilizing = false;
        stableBrake_backUp = stableBrake; // ������ �����̺�극��ũ ���� ���� ����
        delayAccelChangeDirection = 0.25f;
        delayAbsoluteZeroSpeed = 0f;
        isRight = true;
        isWalking = false;
    }

    private void SpeedVelocityLimit() // �̵��ӵ� ���� �޼���(�Լ�)
    {
        if (rb.velocity.x > maxSpeedVelocity_x)        
            rb.velocity = new Vector3(maxSpeedVelocity_x, rb.velocity.y, rb.velocity.z);        
        else if (rb.velocity.x < (maxSpeedVelocity_x * -1))        
            rb.velocity = new Vector3((maxSpeedVelocity_x * -1), rb.velocity.y, rb.velocity.z);        
    }

    private void SpeedAbsoluteZero()
    {
        delayAbsoluteZeroSpeed = 2.5f; // ��ư �ȴ����� ������ ���� �� ������ �ð��� �� �Ǹ� ���� ������ �ְ� ������ִ� �ڵ�, ���� �� �� �Ʒ� @@@speedStable@@@ ǥ���� �ڵ�� ���� ����
        if (delayAbsoluteZeroSpeed > 0f)
        {
            delayAbsoluteZeroSpeed -= Time.deltaTime;
            if (delayAbsoluteZeroSpeed <= 0f)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
    }

    void CharacterHorizontalFlip() // Ⱦ���� �¿� ���� ��Ű�� ������ �Լ�
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1; // x�� -1�� ���ϸ� �¿� ������ ������
        transform.localScale = scale;
    }

    void FixedUpdate() // �Ƚ��������Ʈ, ������ �ð��� ���� �������� �����ϹǷ� ��Ģ���� ������ ����������, ���õǴ� ������ ���ɼ� ����. (�� ���� Ȧ�� �ؾ��ϴ� ��ư, ������ �ð� ����)
    {
        currentVelocity_x = rb.velocity.x; // ���� x������ �ӵ� (Ȯ�ο�)
        if (isObjMovingByInput)
        {
            SpeedVelocityLimit(); // ���� �ӵ� �ѱ��� �ʰ� �� �����Ӹ��� SpeedVelocityLimit �޼��带 ȣ��
        }


        // �Ʒ� �ܶ��� delayAccelChangeDirection�� @@@delayAccel@@@ Ȯ��
        if (Input.GetKey(KeyCode.LeftArrow) || leftButton)
        {
            isObjMovingByInput = true; // ������Ʈ�� �Է¿� ���� �����̴� ��. �ο� : ��
            isSpeedVelocityStabilizing = false; // �Է¿� ���� �����̴� ���̱� ������ ����ȭ�� : ����          
            if (changeDirection) // ������ �ٲ����?
                rb.AddForce(accelPower * -1, 0, 0, ForceMode.Force); // ���� �ؾ��ϹǷ� ���� �ٲ��� ��, ���� ��� ���� ������ �̵�
            else
                rb.AddForce(-1f * speed, 0, 0, ForceMode.Force); // �⺻ �ӵ�, �ν����Ϳ��� speed�� ���� ����
        }
        else if (Input.GetKey(KeyCode.RightArrow) || rightButton)
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
        anim.SetBool("isWalking", isWalking); // �ִϸ������� ��� ����

        if (Input.GetKeyDown(KeyCode.LeftArrow) || leftButton) // ����
        {
            isWalking = true; // ��ư ���� ������ �ȴ� ��~
            if (isRight)
            {
                isRight = false;
                if (!(changeDirection))
                    changeDirection = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || rightButton) // ������
        {
            isWalking = true;
            if (!(isRight))
            {
                isRight = true;
                if (!(changeDirection))
                    changeDirection = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || (leftRightButtonUpSignal)) // ����Ű���� ���� ���� �� �Ͼ �ڵ�
        {
            isObjMovingByInput = false; // �Է¿� ���� �����̴� ������Ʈ�� : ����
            isSpeedVelocityStabilizing = true; // ���ǵ座�ν�Ƽ����ȭ : �� ( %%% ��� �� �κ��� critical section (�Ӱ迵��) �̱� ������ ������ �� �ش� ������ �����ϴ� �ڵ嵵 �Բ� ������)

            SpeedAbsoluteZero(); // ���ǵ� 0 ���� �����

            isWalking = false; // �ȴ� �� : ����
        }

        if (isObjMovingByInput) // �̲������� ����(���º����¡)�� �ٽ� �����̸� �극��ũ ���� �ʱⰪ����
            stableBrake = stableBrake_backUp;

        if (isSpeedVelocityStabilizing) // @@@speedStable@@@ ���ǵ座�ν�Ƽ ����ȭ�� ���� ��, �극��ũ �ʵ�(����)�� �̿��Ͽ� �̲����� ����
        {            
            if (stableBrake > 0f)
            {
                stableBrake -= Time.deltaTime;
                rb.velocity = new Vector3((isRight) ? stableBrake : (stableBrake * -1), rb.velocity.y, rb.velocity.z); // ���׿����ڷ� ��� ����(��, ��)���� �̲���������
                if (stableBrake <= 0f)
                {
                    stableBrake = stableBrake_backUp;
                    isSpeedVelocityStabilizing = false;
                    leftRightButtonUpSignal = false; // % critical section
                }
            }
        }

        if (changeDirection) // @@@delayAccel@@@ ������ �ٲ������ delayAccelChangeDirection�� ª�� ������ �ð��� ��
        {
            if (delayAccelChangeDirection > 0f)
            {
                delayAccelChangeDirection -= Time.deltaTime;
                if (delayAccelChangeDirection <= 0f)
                {
                    delayAccelChangeDirection = 0.25f;
                    changeDirection = false;
                    CharacterHorizontalFlip();
                }
            }
        }
    }

}
