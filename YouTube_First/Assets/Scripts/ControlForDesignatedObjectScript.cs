using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlForDesignatedObjectScript : MonoBehaviour
{
    // 일러두기
    // 만약 오브젝트가 외부 요인에 의해 transform을 이동해야 한다면 현재 클래스 내에 있는 일부코드, 특히 transform 을 직접적으로 건드리는 것을 같이 확인해야 함.
    // 입력에 의한 움직임은 리지드바디의 velocity로 했기 때문에 큰 문제는 없을 거 같음

    Rigidbody rb;
    // SpriteRenderer sr; (플레이어에게 2d 스프라이트가 필요할 시에 사용, 만약 나중에 악세사리나 커스터마이징을 원할 경우에는 자식 오브젝트의 클래스를 상속시키는 것이 좋음)
    Animator anim;

    public bool leftRightButtonUpSignal; // 키보드 버튼에서 손을 땠을 때랑 ui버튼 에서 손을 땟을 때는 작동 방식이 달라서,  isSpeedVelocityStabilizing 변수의 critical section (임계영역) 문제를 해결하기 위해 만듬.
    public bool leftButton, rightButton; // static(정적) 필드(변수)는 클래스 바로 옆에 적재되기 때문에, 현재 이 스크립트를 갖고 있는 오브젝트가 시작되기도 전에 다른 
                                         //스크립트에서 참조할 수 있음. 필요하다면 public static bool 로 타입을 바꿔주고 UI매니저 스크립트애서 직접 변수를 수정 해줘야 함.
                                         //추가 + 버튼이 동시에 눌렸을 때의 Exception(예외) 을 코딩 해야할 거 같음. 해야한다면 else if 로 코드를 추가해야 함.

    [SerializeField]
    float speed; // 오브젝트 이동 속도
    float acceleration; // 오브젝트 가속도
    bool isObjMovingByInput; // 방향키 입력에 의해 움직이는지 확인하기 위한 부울
    bool isSpeedVelocityStabilizing; // 스피드 벨로시티가 안정화를 하고 있는가 확인하기 위한 부울
    public float stableBrake, stableBrake_backUp; // 브레이크 속도 (인스펙터에서 수정 가능, 꼭 0보다 큰 수여야함)
    public bool changeDirection; // 오브젝트가 방향을 바꿨는지 확인하는 부울
    public float accelPower; // 방향 바꿀 때 아주 잠깐 더 강하게 움직이는 힘, speed 보단 값이 커야함
    float delayAccelChangeDirection; // 가속도 가하는 시간 (accelPower의 코드가 수행되는 아주 짧은 딜레이)
    public float maxSpeedVelocity_x; // x방향 최고 속도 제한
    public float currentVelocity_x; // 현재 x방향의 속도 (확인용)
    float delayAbsoluteZeroSpeed; // 스피드가 0 이 되기위한 딜레이 (업데이트함수와 픽스드업데이트함수에 함께 사용되는 임계영역(critical section)코드 차이로 일어나는 약간의 오차를 수정하기 위해만듬.)
    public bool isRight; // 초기값은 오른쪽을 보고있는 것로 하겠음. isRight = true;
    public bool isWalking; // 움직이는 중 인지?

    void Awake()
    {
        leftRightButtonUpSignal = false;
        leftButton = false;
        rightButton = false;
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        // sr = GetComponent<SpriteRenderer>(); // 2d 이용 시에 개봉 그리고 (2개 이상)자식개체 스프라이트에 접근하애한다면 아래코드 참조
        // SpriteRenderer[] rends = GetComponentsInChildren<SpriteRenderer>(true) // 이 코드를 필요한 메서드에 써주고 for(int i = 0; i < rends.Length; i++) 처럼 반복문으로 접근하면 편함. 예시) for(int i = 0; i < rends.Length; i++) rends.color = new Color(1, 1, 1, 0.5f); 식으로 하면 반투명한 스프라이트 만들 수 있음
        anim = GetComponent<Animator>();

        changeDirection = false;
        isObjMovingByInput = false;
        isSpeedVelocityStabilizing = false;
        stableBrake_backUp = stableBrake; // 기존의 스테이블브레이크 값을 따로 저장
        delayAccelChangeDirection = 0.25f;
        delayAbsoluteZeroSpeed = 0f;
        isRight = true;
        isWalking = false;
    }

    private void SpeedVelocityLimit() // 이동속도 제한 메서드(함수)
    {
        if (rb.velocity.x > maxSpeedVelocity_x)        
            rb.velocity = new Vector3(maxSpeedVelocity_x, rb.velocity.y, rb.velocity.z);        
        else if (rb.velocity.x < (maxSpeedVelocity_x * -1))        
            rb.velocity = new Vector3((maxSpeedVelocity_x * -1), rb.velocity.y, rb.velocity.z);        
    }

    private void SpeedAbsoluteZero()
    {
        delayAbsoluteZeroSpeed = 2.5f; // 버튼 안누르고 가만히 있을 때 딜레이 시간이 다 되면 완전 가만히 있게 만들어주는 코드, 수정 할 때 아래 @@@speedStable@@@ 표시한 코드랑 같이 보자
        if (delayAbsoluteZeroSpeed > 0f)
        {
            delayAbsoluteZeroSpeed -= Time.deltaTime;
            if (delayAbsoluteZeroSpeed <= 0f)
            {
                rb.velocity = new Vector3(0, rb.velocity.y, rb.velocity.z);
            }
        }
    }

    void CharacterHorizontalFlip() // 횡으로 좌우 반전 시키는 간단한 함수
    {
        Vector3 scale = transform.localScale;
        scale.x *= -1; // x에 -1을 곱하면 좌우 반전이 되지용
        transform.localScale = scale;
    }

    void FixedUpdate() // 픽스드업데이트, 고정된 시간에 맞춰 프레임을 보강하므로 규칙적인 수행이 가능하지만, 무시되는 연산의 가능성 있음. (꾹 눌러 홀딩 해야하는 버튼, 일정한 시간 연산)
    {
        currentVelocity_x = rb.velocity.x; // 현재 x방향의 속도 (확인용)
        if (isObjMovingByInput)
        {
            SpeedVelocityLimit(); // 제한 속도 넘기지 않게 매 프레임마다 SpeedVelocityLimit 메서드를 호출
        }


        // 아래 단락의 delayAccelChangeDirection는 @@@delayAccel@@@ 확인
        if (Input.GetKey(KeyCode.LeftArrow) || leftButton)
        {
            isObjMovingByInput = true; // 오브젝트가 입력에 의해 움직이는 중. 부울 : 참
            isSpeedVelocityStabilizing = false; // 입력에 의해 움직이는 중이기 때문에 안정화는 : 거짓          
            if (changeDirection) // 방향이 바뀌었나?
                rb.AddForce(accelPower * -1, 0, 0, ForceMode.Force); // 급턴 해야하므로 방향 바꿨을 때, 아주 잠깐 강한 힘으로 이동
            else
                rb.AddForce(-1f * speed, 0, 0, ForceMode.Force); // 기본 속도, 인스펙터에서 speed로 수정 가능
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

    void Update() // 업데이트 함수, 모든 프레임 마다 실행하기 때문에 무시되는 수행이 없음. (단발적인 입력의 메서드 혹은 버튼)
    {
        anim.SetBool("isWalking", isWalking); // 애니메이터의 노드 변경

        if (Input.GetKeyDown(KeyCode.LeftArrow) || leftButton) // 왼쪽
        {
            isWalking = true; // 버튼 눌려 있으면 걷는 중~
            if (isRight)
            {
                isRight = false;
                if (!(changeDirection))
                    changeDirection = true;
            }
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) || rightButton) // 오른쪽
        {
            isWalking = true;
            if (!(isRight))
            {
                isRight = true;
                if (!(changeDirection))
                    changeDirection = true;
            }
        }

        if (Input.GetKeyUp(KeyCode.LeftArrow) || Input.GetKeyUp(KeyCode.RightArrow) || (leftRightButtonUpSignal)) // 방향키에서 손을 땠을 때 일어날 코드
        {
            isObjMovingByInput = false; // 입력에 의해 움직이는 오브젝트는 : 거짓
            isSpeedVelocityStabilizing = true; // 스피드벨로시티안정화 : 참 ( %%% 사실 이 부분이 critical section (임계영역) 이기 때문에 수정할 때 해당 변수가 관여하는 코드도 함께 봐야함)

            SpeedAbsoluteZero(); // 스피드 0 으로 만들기

            isWalking = false; // 걷는 중 : 거짓
        }

        if (isObjMovingByInput) // 미끄러지는 도중(스태블라이징)에 다시 움직이면 브레이크 값은 초기값으로
            stableBrake = stableBrake_backUp;

        if (isSpeedVelocityStabilizing) // @@@speedStable@@@ 스피드벨로시티 안정화가 참일 때, 브레이크 필드(변수)를 이용하여 미끄러짐 구현
        {            
            if (stableBrake > 0f)
            {
                stableBrake -= Time.deltaTime;
                rb.velocity = new Vector3((isRight) ? stableBrake : (stableBrake * -1), rb.velocity.y, rb.velocity.z); // 삼항연산자로 어느 방향(좌, 우)으로 미끄러지는지
                if (stableBrake <= 0f)
                {
                    stableBrake = stableBrake_backUp;
                    isSpeedVelocityStabilizing = false;
                    leftRightButtonUpSignal = false; // % critical section
                }
            }
        }

        if (changeDirection) // @@@delayAccel@@@ 방향이 바뀌었으면 delayAccelChangeDirection에 짧은 딜레이 시간을 줌
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
