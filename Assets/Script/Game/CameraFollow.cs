using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 相机跟踪方法
/// </summary>
public class CameraFollow : MonoBehaviour
{
    private Transform target;
    private Vector3 offset;
    private Vector2 velocity;
    // Update is called once per frame
    private void Update()
    {
        if (target == null && GameObject.FindGameObjectWithTag("Player") != null)
        {
            //只调用一次
            target = GameObject.FindGameObjectWithTag("Player").transform;
            //给偏移量赋值
            offset = target.position - transform.position;
        }
    }
    //这是相机自动调用的方法
    private void FixedUpdate(){
        if (target != null)
        {
            //平滑缓冲，相机不是僵硬的移动而是做减速缓冲运动到指定位置
            //相机平滑缓冲，velocity无用但必须存在
            //velocity（相对缓冲减速，看起来很屌很复杂，事实上我们根本不用理睬他，它只是用作一个承载变量，我们只要默认赋值其实就是0就可以了）。
            float posX = Mathf.SmoothDamp(transform.position.x,
                target.position.x - offset.x, ref velocity.x, 0.05f);
            float posY = Mathf.SmoothDamp(transform.position.y,
                target.position.y - offset.y, ref velocity.y, 0.05f);
            //让摄像机只往上移
            if (posY > transform.position.y)
                transform.position = new Vector3(posX, posY, transform.position.z);
        }
    }
}
