using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PlayerController : MonoBehaviour
{
    public Transform rayDown, rayLeft, rayRight;
    public LayerMask platformLayer, obstacleLayer;
    /// <summary>
    /// 是否向左移动，反之向右
    /// </summary>
    private bool isMoveLeft = false;
    /// <summary>
    /// 防止玩家一直单击，一直单击就不响应
    /// </summary>
    private bool isJumping = false;
    private Vector3 nextPlatformLeft, nextPlatformRight;
    private ManagerVars vars;
    /// <summary>
    /// 用于判断游戏结束
    /// </summary>
    private Rigidbody2D my_Body;
    private SpriteRenderer spriteRenderer;
    private void Awake()
    {
        vars = ManagerVars.GetManagerVars();
        spriteRenderer = GetComponent<SpriteRenderer>();
        my_Body = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (!GameManager.Instance.IsGameStarted || GameManager.Instance.IsGameOver)
            return;
        //--1--鼠标事件触发之后，被重复调用了
        if (Input.GetMouseButtonDown(0)&&!isJumping)
        {
            Debug.Log("单击游戏");
            //鼠标点击之后再广播这个方法
            EventCenter.Broadcast(EventDefine.DecidePath);
            isJumping = true;
            Vector3 mousePos = Input.mousePosition;
            //点击的是左边屏幕
            if (mousePos.x <= Screen.width / 2)
            {
                isMoveLeft = true;
            }
            //点击的是右边屏幕
            else if (mousePos.x > Screen.width / 2)
            {
                isMoveLeft = false;
            }
            Jump();
        }
        //游戏结束判断
        if (my_Body.velocity.y < 0 && IsRayPlatform() == false && GameManager.Instance.IsGameOver == false)
        {
            Debug.Log("结束");
            spriteRenderer.sortingLayerName = "Default";
            GetComponent<BoxCollider2D>().enabled = false;
            GameManager.Instance.IsGameOver = true;
            //调用结束面板
            //TODO
        }
        if (isJumping && IsRayObstacle() && !GameManager.Instance.IsGameOver)
        {

        }
    }
    /// <summary>
    /// 是否检测到平台
    /// </summary>
    /// <returns></returns>
    private bool IsRayPlatform()
    {
        //--2--射线打不到下一个平台，而出现自动掉落的情况
        //Physics2D.Raycast(Vector2 origin, Vector2 direction, float distance, int layerMask);
        //1、origin：射线投射的原点
        //2、direction：射线投射的方向
        //3、distance：射线的长度
        //4、layerMask：射线只会投射到layerMask层的碰撞体（注意此int参数的写法：1 << 层数）
        RaycastHit2D hit = Physics2D.Raycast(rayDown.position, Vector2.down, 1f, platformLayer);
        Debug.Log("hit:" + hit);
        Debug.Log("hit.collider:" + hit.collider);
        if (hit.collider != null)
        {
            Debug.Log("hit.collider.tag:" + hit.collider.tag);
            if (hit.collider.tag == "Platform")
                return true;
        }
        return false;
    }
    /// <summary>
    /// 是否检测到障碍物
    /// </summary>
    /// <returns></returns>
    private bool IsRayObstacle()
    {
        RaycastHit2D leftHit = Physics2D.Raycast(rayLeft.position, Vector2.left, obstacleLayer);
        RaycastHit2D rightHit = Physics2D.Raycast(rayRight.position, Vector2.right, obstacleLayer);
        if (leftHit.collider != null)
        {
            if (leftHit.collider.tag == "Obstacle")
                return true;
        }
        if (rightHit.collider != null)
        {
            if (rightHit.collider.tag == "Obstacle")
                return true;
        }
        return false;
    }
    private void Jump()
    {
        if (isMoveLeft)
        {
            //角色面向左边
            transform.localScale = new Vector3(-1, 1, 1);
            //X向左移动0.2秒
            transform.DOMoveX(nextPlatformLeft.x, 0.2f);
            //Y向左移动0.15秒
            transform.DOMoveY(nextPlatformLeft.y + 0.8f, 0.15f);
        }
        else
        {
            //角色面向右边
            transform.localScale = Vector3.one;//(1,1,1)
            transform.DOMoveX(nextPlatformRight.x, 0.2f);
            transform.DOMoveY(nextPlatformRight.y + 0.8f, 0.15f);
        }
    }
    /// <summary>
    /// 碰撞检测到后获取到后面两个平台的可能性
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        //当碰撞到平台Platform的时候才触发此方法
        if (collision.tag == "Platform")
        {
            //落到平台上之后就可以再次响应玩家的请求
            isJumping = false;
            //获取到当前平台的位置
            Vector3 currentPlatformPos = collision.gameObject.transform.position;
            nextPlatformLeft = new Vector3(currentPlatformPos.x - vars.nextXPos,
                currentPlatformPos.y + vars.nextYPos, 0);
            nextPlatformRight = new Vector3(currentPlatformPos.x + vars.nextXPos,
                currentPlatformPos.y + vars.nextYPos, 0);
        }
    }
}
