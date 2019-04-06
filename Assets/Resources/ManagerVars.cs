using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//用于创建一个unity临时菜单
//[CreateAssetMenu(menuName="CreatManagerVarsContainer")]
public class ManagerVars : ScriptableObject
{
    /// <summary>
    /// 返回Resources函数加载的ManagerVarsContainer文件
    /// </summary>
    /// <returns></returns>
    public static ManagerVars GetManagerVars()
    {
        //文件中含有改函数定义的所有公用元素
        return Resources.Load<ManagerVars>("ManagerVarsContainer");
    }
    /// <summary>
    /// 存放随机背景图片四张
    /// </summary>
    public List<Sprite> bgThemeSpriteList = new List<Sprite>();

    public List<Sprite> platformThemeSpriteList = new List<Sprite>();
    /// <summary>
    /// 用于存放人物
    /// </summary>
    public GameObject characterPre;
    /// <summary>
    /// 用于存放跳板平台
    /// </summary>
    public GameObject normalPlatformPre;
    public List<GameObject> commonPlatformGroup = new List<GameObject>();
    public List<GameObject> grassPlatformGroup = new List<GameObject>();
    public List<GameObject> winterPlatformGroup = new List<GameObject>();
    public GameObject spikePlatformLeft;
    public GameObject spikePlatformRight;
    /// <summary>
    /// 用于存放每个跳板变动的X值和Y值
    /// </summary>
    public float nextXPos = 0.554f, nextYPos = 0.645f;
}
