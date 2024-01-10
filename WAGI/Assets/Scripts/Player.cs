using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool isTouchTop;
    public bool isTouchBottom;
    public bool isTouchLeft;
    public bool isTouchRight;

    public int life; // 목숨
    public int score; // 점수

    public float speed;
    public int power;
    public int maxPower;
    public int boom;
    public int maxBoom;

    public float maxShotDelay; // 총알 재장전 쿨타임
    public float curShotDelay; // 총알 재장전 쿨타임

    public GameManager gameManager;
    public ObjectManager objectManager;
    public bool isHit;
    public bool isBoomTime;


    public GameObject bulletObjA; // 총알 오브젝트A
    public GameObject bulletObjB; // 총알 오브젝트B
    public GameObject boomEffect;
    
    public GameObject[] followers;

    public bool[] joyControl;
    public bool isControl;
    public bool isButtonA;
    public bool isButtonB;
    public bool isRespawnTime;

    Animator anim;
    SpriteRenderer spriteRenderer;
    void Awake()
    {
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable() // 죽었을 때 무적상태
    {
        Unbeatable();
        Invoke("Unbeatable", 3);
    }

    void Unbeatable()
    {
        isRespawnTime = !isRespawnTime;
        if (isRespawnTime)
        {
            spriteRenderer.color = new Color(1, 1, 1, 0.5f);
        }
        else
        {
            spriteRenderer.color = new Color(1, 1, 1, 1);
        }
    }
    void Update()
    {
        Move();
        Fire();
        Boom();
        Reload();
    }

    public void JoyPanel(int type)
    {
        for(int index = 0; index < 9; index++)
        {
            joyControl[index] = (index == type);
        }
    }

    public void JoyDown()
    {
        isControl = true;
    }
    public void JoyUp()
    {
        isControl = false;
    }
    void Move() //이동
    {
        //#.Keyboard Control Value
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        //#.Joy Control Value
        if (joyControl[0]) { h = -1; v = 1; }
        if (joyControl[1]) { h = 0; v = 1; }
        if (joyControl[2]) { h = 1; v = 1; }
        if (joyControl[3]) { h = -1; v = 0; }
        if (joyControl[4]) { h = 0; v = 0; }
        if (joyControl[5]) { h = 1; v = 0; }
        if (joyControl[6]) { h = -1; v = -1; }
        if (joyControl[7]) { h = 0; v = -1; }
        if (joyControl[8]) { h = 1; v = -1; }



        if ((isTouchRight && h == 1) || (isTouchLeft && h == -1) || !isControl)
            h = 0;
        if ((isTouchTop && v == 1) || (isTouchBottom && v == -1) || !isControl)
            v = 0;

        Vector3 curPos = transform.position;
        Vector3 nextPos = new Vector3(h, v, 0) * speed * Time.deltaTime;

        transform.position = curPos + nextPos;

        if (Input.GetButtonDown("Horizontal") ||
            Input.GetButtonUp("Horizontal"))
        {
            anim.SetInteger("Input", (int)h);
        }
    }
    public void ButtonADown()
    {
        isButtonA = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.AttackA);
    }

    public void ButtonAUp()
    {
        isButtonA = false;
    }

    public void ButtonBDown()
    {
        isButtonB = true;
    }


    void Fire()
    {
        /*if (!Input.GetButton("Fire1")) // 클릭 시 발사 제어
        {
            return;
        }*/

        if (!isButtonA)
            return;

        //#.발사 재장전 쿨타임
        if (curShotDelay < maxShotDelay)
        {
            return;
        }

        switch (power)
        {
            case 1:
                // Power One
                GameObject bullet = objectManager.MakeObj("BulletPlayerA");
                bullet.transform.position = transform.position;

                Rigidbody2D rigid = bullet.GetComponent<Rigidbody2D>();
                rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            case 2:
                // Power two
                GameObject bulletR = objectManager.MakeObj("BulletPlayerA");
                bulletR.transform.position = transform.position + Vector3.right * 0.1f;
                
                GameObject bulletL = objectManager.MakeObj("BulletPlayerA");
                bulletL.transform.position = transform.position + Vector3.left * 0.1f;

                Rigidbody2D rigidR = bulletR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidL = bulletL.GetComponent<Rigidbody2D>();
                rigidR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
            default:
                // power three,four,five,six - maxPower 6으로 변경함
                GameObject bulletRR = objectManager.MakeObj("BulletPlayerA");
                bulletRR.transform.position = transform.position + Vector3.right * 0.2f;

                GameObject bulletCC = objectManager.MakeObj("BulletPlayerB");
                bulletCC.transform.position = transform.position;

                GameObject bulletLL = objectManager.MakeObj("BulletPlayerA");
                bulletLL.transform.position = transform.position + Vector3.left * 0.2f;
                
                Rigidbody2D rigidRR = bulletRR.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidCC = bulletCC.GetComponent<Rigidbody2D>();
                Rigidbody2D rigidLL = bulletLL.GetComponent<Rigidbody2D>();
                
                rigidRR.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidCC.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                rigidLL.AddForce(Vector2.up * 10, ForceMode2D.Impulse);
                break;
        }
        //#.딜레이 초기화
        curShotDelay = 0; 
    }

    //#.총알 재장전
    void Reload()
    {
        curShotDelay += Time.deltaTime;
    }

    void Boom()
    {
        /*if (!Input.GetButton("Fire2"))
        {
            return;
        }*/


        if (!isButtonB)
            return;

        if (isBoomTime)
        {
            return;
        }
        if (boom == 0)
        {
            return;
        }
        boom--;
        isBoomTime = true;
        AudioManager.instance.PlaySfx(AudioManager.Sfx.AttackB);
        gameManager.UpdateBoomIcon(boom);
        boomEffect.SetActive(true);
        Invoke("OffBoomEffect", 4f);
        GameObject[] enemiesL = objectManager.GetPool("EnemyL");
        GameObject[] enemiesM = objectManager.GetPool("EnemyM");
        GameObject[] enemiesS = objectManager.GetPool("EnemyS");
        
        for (int index = 0; index < enemiesL.Length; index++)
        {
            if (enemiesL[index].activeSelf)
            {
                Enemy enemyLogic = enemiesL[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        
        for (int index = 0; index < enemiesM.Length; index++)
        {
            if (enemiesM[index].activeSelf)
            {
                Enemy enemyLogic = enemiesM[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }
        for (int index = 0; index < enemiesS.Length; index++)
        {
            if (enemiesS[index].activeSelf)
            {
                Enemy enemyLogic = enemiesS[index].GetComponent<Enemy>();
                enemyLogic.OnHit(1000);
            }
        }

        GameObject[] bulletsA = objectManager.GetPool("BulletEnemyA");
        GameObject[] bulletsB = objectManager.GetPool("BulletEnemyB");
        for (int index = 0; index < bulletsA.Length; index++)
        {
            if (bulletsA[index].activeSelf)
            {
                bulletsA[index].SetActive(false);
            }
        }
        for (int index = 0; index < bulletsB.Length; index++)
        {
            if (bulletsB[index].activeSelf)
            {
                bulletsB[index].SetActive(false);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Border")
        {
            switch (collision.gameObject.name)
            {
                case "Top":
                    isTouchTop = true;
                    break;
                case "Bottom":
                    isTouchBottom = true;
                    break;
                case "Left":
                    isTouchLeft = true;
                    break;
                case "Right":
                    isTouchRight = true;
                    break;
            }
        }
        else if (collision.gameObject.tag == "Enemy" || collision.gameObject.tag == "EnemyBullet")
        {
            //#.무적상태인 경우
            if (isRespawnTime)
                return;

            //#.맞았을 경우
            if (isHit)
                return;

            isHit = true;

            life--;
            AudioManager.instance.PlaySfx(AudioManager.Sfx.Dead);
            gameManager.UpdateLifeIcon(life);
            gameManager.CallExplosion(transform.position, "P");

            if (life == 0)
            {
                gameManager.GameOver();
            }
            else
            {
                gameManager.RespawnPlayer();
            }

            gameObject.SetActive(false);
            collision.gameObject.SetActive(false);
        }
        else if (collision.gameObject.tag == "Item")
        {
            Item item = collision.gameObject.GetComponent<Item>();
            switch (item.type)
            {
                case "Coin":
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.EarnItem);
                    score += 1000;
                    break;
                case "Power":
                    AudioManager.instance.PlaySfx(AudioManager.Sfx.EarnItem);
                    if (power == maxPower)
                        score += 500;
                    else{
                        power++;
                        AddFollower();
                    }
                    break;
                case "Boom":
                     AudioManager.instance.PlaySfx(AudioManager.Sfx.EarnItem);
                    if (boom == maxBoom)
                        score += 500;
                    else
                    {
                        boom++;
                        gameManager.UpdateBoomIcon(boom);
                    }
                    break;
            }
            collision.gameObject.SetActive(false);
        }
    }

    //#.보조무기 장착
    void AddFollower()
    {
        if(power ==2)
            followers[0].SetActive(true);
        if(power ==4)
            followers[1].SetActive(true);
        if(power ==6)
            followers[2].SetActive(true);
    }


    void OffBoomEffect()
    {
        boomEffect.SetActive(false);
        isBoomTime = false;
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Border") {
            switch(collision.gameObject.name) {
                case "Top":
                    isTouchTop = false;
                    break;
                case "Bottom":
                    isTouchBottom = false;
                    break;
                case "Left":
                    isTouchLeft = false;
                    break;
                case "Right":
                    isTouchRight = false;
                    break;
            }
        }
    }
   
}
