using UnityEngine;

// PlayerController는 플레이어 캐릭터로서 Player 게임 오브젝트를 제어한다.
public class PlayerController : MonoBehaviour
{
  public AudioClip deathClip; // 사망시 재생할 오디오 클립
  public float jumpForce = 700f; // 점프 힘
  public float m_fRepositionSpeed = 2f; // 제자리로 돌아가는 속도

  private int jumpCount = 0; // 누적 점프 횟수
  private bool isGrounded = false; // 바닥에 닿았는지 나타냄
  private bool isDead = false; // 사망 상태

  private Rigidbody2D playerRigidbody; // 사용할 리지드바디 컴포넌트
  private Animator animator; // 사용할 애니메이터 컴포넌트
  private AudioSource playerAudio; // 사용할 오디오 소스 컴포넌트

  private void Start()
  {
    playerRigidbody = GetComponent<Rigidbody2D>();
    animator = GetComponent<Animator>();
    playerAudio = GetComponent<AudioSource>();
    // 초기화
  }

  private void Update()
  {
    if (isDead == true) return;

    if (Input.GetMouseButtonDown(0) == true && jumpCount < 2)
    {
      jumpCount++;
      playerRigidbody.velocity = Vector2.zero;
      playerRigidbody.AddForce(new Vector2(0, jumpForce));
      playerAudio.Play();
    }
    else if (Input.GetMouseButtonUp(0) == true && playerRigidbody.velocity.y > 0)
    {
      Vector2 v2Player = playerRigidbody.velocity;

      v2Player.y *= 0.5f;
      playerRigidbody.velocity = v2Player;
    }

    animator.SetBool("Grounded", isGrounded);
    // 사용자 입력을 감지하고 점프하는 처리

    float fXDelta = transform.position.x - (-6);
    if (Mathf.Abs(fXDelta) > Mathf.Epsilon)
    {
      Vector3 v3Position = transform.position;
      if (Mathf.Abs(fXDelta) < 0.05)
      {
        v3Position.x = -6;
        transform.position = v3Position;
      }
      else if (fXDelta > 0) transform.Translate(Vector3.left * m_fRepositionSpeed * Time.deltaTime);
      else transform.Translate(Vector3.right * m_fRepositionSpeed * Time.deltaTime);
      //transform.position = v3Position;
    }
  }

  private void Die()
  {
    animator.SetTrigger("Die");
    playerAudio.clip = deathClip;
    playerAudio.Play();

    playerRigidbody.velocity = Vector2.zero;
    isDead = true;

    GameManager.instance.OnPlayerDead();
  }

  private void OnTriggerEnter2D(Collider2D other)
  {
    if (other.tag == "Dead" && isDead == false)
    {
      Die();
    }
  }

  private void OnCollisionEnter2D(Collision2D collision)
  {
    if (collision.contacts[0].normal.y > 0.7f)
    {
      isGrounded = true;
      jumpCount = 0;
    }
    // 바닥에 닿았음을 감지하는 처리
  }

  private void OnCollisionExit2D(Collision2D collision)
  {
    isGrounded = false;
    // 바닥에서 벗어났음을 감지하는 처리
  }
}