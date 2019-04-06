using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlatformGroupType
{
    Grass,
    Winter
}
public class PlayformSpawner : MonoBehaviour
{
    /// <summary>
    /// 游戏对象 public声明会显示在unity脚本中
    /// </summary>
    public Vector3 startSpawnPos;
    /// <summary>
    /// 生成平台数量
    /// </summary>
    private int spawnPlatformCount;
    private ManagerVars vars;
    /// <summary>
    /// 平台的生成位置
    /// </summary>
    private Vector3 platfromSpawnPosition;
    /// <summary>
    /// 是否向左边生成，反之朝右生成
    /// </summary>
    private bool isLeftSpawn = false;
    /// <summary>
    /// 选择的平台图片
    /// </summary>
    private Sprite selectPlatformSprite;
    /// <summary>
    /// 组合平台的类型
    /// </summary>
    private PlatformGroupType groupType;
    /// <summary>
    /// 钉子组合平台是否生成在左边，反之右边
    /// </summary>
    private bool spikeSpawnLeft = false;
    /// <summary>
    /// 钉子方向平台的位置
    /// </summary>
    private Vector3 spikeDirPlatformPos;
    /// <summary>
    /// 生成钉子平台之后需要在钉子方向生成的平台数量
    /// </summary>
    private int afterSpawnSpikeSpawnCount;
    /// <summary>
    /// 钉子平台是否生成的方法
    /// </summary>
    private bool isSpawnSpike;
    private void Awake()
    {
        //设置监听
        EventCenter.AddListener(EventDefine.DecidePath, DecidePath);
        vars = ManagerVars.GetManagerVars();
    }
    private void OnDestroy()
    {
        //删除监听
        EventCenter.RemoveListener(EventDefine.DecidePath, DecidePath);
    }
    private void Start()
    {
        RandomPlatformTheme();
        platfromSpawnPosition = startSpawnPos;
        for (int i = 0; i < 5; i++)
        {
            spawnPlatformCount = 5;
            DecidePath();
        }
        //生成人物
        GameObject go = Instantiate(vars.characterPre);
        go.transform.position = new Vector3(0, -1.8f, 0);
    }
    /// <summary>
    /// 随机平台主题
    /// </summary>
    private void RandomPlatformTheme()
    {
        int ran = Random.Range(0, vars.platformThemeSpriteList.Count);
        selectPlatformSprite = vars.platformThemeSpriteList[ran];
        if (ran == 2)
        {
            groupType = PlatformGroupType.Winter;
        }
        else
        {
            groupType = PlatformGroupType.Grass;
        }
    }
    /// <summary>
    /// 确定路径
    /// </summary>
    private void DecidePath()
    {
        //Debug.Log(spawnPlatformCount + "DecidePath");
        if (isSpawnSpike)
        {
            AfterSpawnSpike();
            return;
        }
        if (spawnPlatformCount > 0)
        {
            spawnPlatformCount--;
        }
        else
        {
            //反转生成方向
            isLeftSpawn = !isLeftSpawn;
            spawnPlatformCount = Random.Range(1, 4);
        }
        SpawnPlatform();
    }
    /// <summary>
    /// 生成平台
    /// </summary>
    private void SpawnPlatform()
    {
        //确定障碍物所在的方向
        int ranObstacleDir = Random.Range(0, 2);
        //生成单个平台
        if (spawnPlatformCount >= 1)
        {
            SpawnNormalPlatform(ranObstacleDir);
        }
        //最后一个在拐角处，生成组合平台
        else if (spawnPlatformCount == 0)
        {
            int ran = Random.Range(0, 3);
            //生成通用组合平台
            if (ran == 0)
            {
                SpawnCommonPlatformGroup(ranObstacleDir);
            }
            //生成主题组合平台
            else if (ran == 1)
            {
                switch (groupType)
                {
                    case PlatformGroupType.Grass:
                        SpawnGrassPlatformGroup(ranObstacleDir);
                        break;
                    case PlatformGroupType.Winter:
                        SpawnWinterPlatformGroup(ranObstacleDir);
                        break;
                    default:
                        break;
                }
            }
            //生成钉子组合平台
            else
            {
                int value = -1;
                if (isLeftSpawn)
                {
                    value = 0;//生成右边的
                }
                else
                {
                    value = 1;//生成左边的
                }
                SpawnSpikePlatform(value);

                isSpawnSpike = true;
                afterSpawnSpikeSpawnCount = 4;
                if (spikeSpawnLeft)//钉子生成在左边
                {
                    spikeDirPlatformPos = new Vector3(platfromSpawnPosition.x - 1.65f,
                        platfromSpawnPosition.y + vars.nextYPos, 0);
                }
                else
                {
                    spikeDirPlatformPos = new Vector3(platfromSpawnPosition.x + 1.65f,
                        platfromSpawnPosition.y + vars.nextYPos, 0);
                }
            }
            //SpawnNormalPlatform
        }
       if (isLeftSpawn)//向左生成
        {
            platfromSpawnPosition = new Vector3(platfromSpawnPosition.x - vars.nextXPos,
                platfromSpawnPosition.y + vars.nextYPos,0);
        }
        else
        {
            platfromSpawnPosition = new Vector3(platfromSpawnPosition.x + vars.nextXPos,
                platfromSpawnPosition.y + vars.nextYPos, 0);
        }
    }
    /// <summary>
    /// 生成普通平台（单个）
    /// </summary>
    private void SpawnNormalPlatform(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetNormalPlatform();
        go.transform.position = platfromSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成通用组合平台
    /// </summary>
    private void SpawnCommonPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetCommonPlatformGroup();
        //修改平台所在的位置
        go.transform.position = platfromSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成草地组合平台
    /// </summary>
    private void SpawnGrassPlatformGroup(int ranObstacleDir)
    {
        GameObject go = ObjectPool.Instance.GetGrassPlatformGroup();
        go.transform.position = platfromSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成冬季组合平台
    /// </summary>
    private void SpawnWinterPlatformGroup(int ranObstacleDir)
    {
        //实例化一个游戏对象 Instantiate实例化
        GameObject go = ObjectPool.Instance.GetWinterPlatformGroup();
        go.transform.position = platfromSpawnPosition;
        go.GetComponent<PlatformScript>().Init(selectPlatformSprite, ranObstacleDir);
        go.SetActive(true);
    }
    /// <summary>
    /// 生成钉子组合平台
    /// </summary>
    /// <param name="dir"></param>
    private void SpawnSpikePlatform(int dir)
    {
        GameObject temp = null;
        if (dir == 0)
        {
            spikeSpawnLeft = false;
            temp = ObjectPool.Instance.GetRightSpikePlatformGroup();
        }
        else
        {
            spikeSpawnLeft = true;
            temp = ObjectPool.Instance.GetLeftSpikePlatformGroup();
        }
        temp.transform.position = platfromSpawnPosition;
        temp.GetComponent<PlatformScript>().Init(selectPlatformSprite, dir);
        temp.SetActive(true);
    }
    /// <summary>
    /// 生成钉子平台之后需要生成的平台
    /// 包括钉子方向，也包括原来的方向
    /// </summary>
    private void AfterSpawnSpike()
    {
        if (afterSpawnSpikeSpawnCount > 0)
        {
            afterSpawnSpikeSpawnCount--;
            for (int i = 0; i < 2; i++)
            {
                GameObject temp = ObjectPool.Instance.GetNormalPlatform();
                if (i == 0)//生成原来方向的平台
                {
                    temp.transform.position = platfromSpawnPosition;
                    //如果钉子在左边，原来路径向右边生成
                    if (spikeSpawnLeft)
                    {
                        platfromSpawnPosition = new Vector3(platfromSpawnPosition.x + vars.nextXPos,
                            platfromSpawnPosition.y + vars.nextYPos, 0);
                    }
                    else
                    {
                        platfromSpawnPosition = new Vector3(platfromSpawnPosition.x - vars.nextXPos,
                            platfromSpawnPosition.y + vars.nextYPos, 0);
                    }
                }
                else//生成钉子方向的平台
                {
                    temp.transform.position = spikeDirPlatformPos;
                    if (spikeSpawnLeft)
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x - vars.nextXPos,
                            spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                    else
                    {
                        spikeDirPlatformPos = new Vector3(spikeDirPlatformPos.x + vars.nextXPos,
                            spikeDirPlatformPos.y + vars.nextYPos, 0);
                    }
                }
                temp.GetComponent<PlatformScript>().Init(selectPlatformSprite, 1);
                temp.SetActive(true);
            }
        }
        else
        {
            isSpawnSpike = false;
            DecidePath();
        }
    }
}
