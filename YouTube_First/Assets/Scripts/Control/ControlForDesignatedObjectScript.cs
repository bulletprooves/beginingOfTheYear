using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlForDesignatedObjectScript : MonoBehaviour
{
    Rigidbody rb;
    // SpriteRenderer sr; (플레이어에게 2d 스프라이트가 필요할 시에 개봉)
    Animator anim;

    [SerializeField]
    float speed; // 오브젝트 이동 속도
    float acceleration; // 오브젝트 가속도
    public bool isObjMovingByInput; // 방향키 입력에 의해 움직이는지 확인하기 위한 부울
    private bool isSpeedVelocityStabilizing; // 스피드 벨로시티가 안정화를 하고 있는가 확인하기 위한 부울
    public float stableBrake, stableBrake_BackUp; // 브레이크 속도 (인스펙터에서 수정 가능, 꼭 0보다 큰 수여야함)
    public bool changeDirection; // 오브젝트가 방향을 바꿨는지 확인하는 부울
    public float accelPower; // 방향 바꿀 때 아주 잠깐 더 강하게 움직이는 힘, speed 보단 값이 커야함
    float delayAccelChangeDirection; // 가속도 가하는 시간 ()
    public float maxSpeedVelocity_x; // x방향 최고 속도 제한
    public float currentVelocity_x; // 현재 x방향의 속도 (확인용)
    public bool isRight; // 초기값은 오른쪽을 보고있는 것로 하겠음. isRight = true;

    void Awake()
    {
        
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // sr = GetComponent<SpriteRenderer>(); (2d 이용 시에 개봉)
        anim = GetComponent<Animator>();

        changeDirection = false;
        isObjMovingByInput = false;
        isSpeedVelocityStabilizing = false;
        stableBrake_BackUp = stableBrake; // 기존의 스테이블브레이크 값을 따로 저장
        delayAccelChangeDirection = 0.25f;
        isRight = true;
    }

    private void SpeedVelocityLimit() // 이동속도 제한 메서드(함수)
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

    void FixedUpdate() // 픽스드업데이트, 고정된 시간에 맞춰 프레임을 보강하므로 규칙적인 수행이 가능하지만, 무시되는 연산의 가능성 있음. (꾹 눌러 홀딩 해야하는 버튼, 일정한 시간 연산)
    {
        currentVelocity_x = rb.velocity.x; // 현재 x방향의 속도 (확인용)
        if (isObjMovingByInput)
        {
            SpeedVelocityLimit(); // 제한 속도 넘기지 않게 매 프레임마다 SpeedVelocityLimit 메서드를 호출
        }

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isObjMovingByInput = true; // 오브젝트가 입력에 의해 움직이는 중. 부울 : 참
            isSpeedVelocityStabilizing = false; // 입력에 의해 움직이는 중이기 때문에 안정화는 : 거짓          
            if (changeDirection) // 방향이 바뀌었나?
                rb.AddForce(accelPower * -1, 0, 0, ForceMode.Force); // 급턴 해야하므로 방향 바꿨을 때, 아주 잠깐 강한 힘으로 이동
            else
                rb.AddForce(-1f * speed, 0, 0, ForceMode.Force); // 기본 속도, 인스펙터에서 speed로 수정 가능
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

    void Update() // 업데이트 함수, 모든 프레임 마다 실행하기 때문에 무시되는 수행이 없음. (단발적인 입력의 메서드 혹은 버튼)
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow)) // 왼쪽
        {
            if (isRight)
            {
                isRight = false;
                if (!(changeDirection))
                    changeDirection = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow)) // 오른쪽
        {
            if (!(isRight))
            {
                isRight = true;
                if (!(changeDirection))
                    changeDirection = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow)) // 방향키에서 손을 땠을 때 일어날 코드
        {
            isObjMovingByInput = false; // 입력에 의해 움직이는 오브젝트는 : 거짓
            isSpeedVelocityStabilizing = true; // 스피드벨로시티안정화 : 참
        }

        if (isObjMovingByInput) // 미끄러지는 도중(스태블라이징)에 다시 움직이면 브레이크 값은 초기값으로
            stableBrake = stableBrake_BackUp;
        if (isSpeedVelocityStabilizing) // 스피드벨로시티 안정화가 참일 때, 브레이크 필드(변수)를 이용하여 미끄러짐 구현
        {
            if (stableBrake > 0f)
            {
                stableBrake -= Time.deltaTime;
                rb.velocity = new Vector3((isRight) ? stableBrake : (stableBrake * -1), rb.velocity.y, rb.velocity.z); // 삼항연산자로 어느 방향(좌, 우)으로 미끄러지는지
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
