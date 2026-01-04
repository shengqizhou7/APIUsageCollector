using UnityEngine;

/// <summary>
/// 测试脚本：触发各类UnityEngine接口调用，用于验证统计工具
/// </summary>
public class APITestBehaviour : MonoBehaviour
{
    public GameObject testCube;
    private Transform cubeTransform;

    private void Start()
    {
        // Debug接口
        Debug.Log("===== 开始API测试 =====");
        Debug.LogWarning("这是警告日志");
        Debug.LogError("这是错误日志");

        // Application接口
        Debug.Log("应用名称：" + Application.productName);
        Debug.Log("数据路径：" + Application.dataPath);

        // Time接口
        Debug.Log("当前时间：" + Time.time);
        Debug.Log("帧率：" + Time.frameCount);

        // GameObject接口
        if (testCube == null) testCube = new GameObject("TestCube");
        cubeTransform = testCube.transform;
        testCube.name = "API_Test_Cube";
        testCube.SetActive(true);

        // Transform接口
        cubeTransform.position = new Vector3(1, 2, 3);
        cubeTransform.Translate(Vector3.forward);

        // Vector3接口
        float distance = Vector3.Distance(cubeTransform.position, Vector3.zero);
        Debug.Log("距离原点：" + distance);
    }

    private void Update()
    {
        // Input接口
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("按下空格键");
            cubeTransform.Rotate(Vector3.up, 10f);
        }

        // // 每60帧触发一次Time接口
        // if (Time.frameCount % 60 == 0)
        // {
        //     Debug.Log("每60帧触发：" + Time.frameCount);
        // }
    }

    // private void OnGUI()
    // {
    //     // GUI接口
    //     if (GUI.Button(new Rect(10, 10, 200, 50), "点击测试"))
    //     {
    //         Debug.Log("点击GUI按钮");
    //         Application.CaptureScreenshot("test_screenshot.png");
    //     }
    //     GUI.Label(new Rect(10, 70, 300, 30), "测试中：" + Time.time);
    // }

    private void OnApplicationQuit()
    {
        Debug.Log("===== 结束API测试 =====");
        Destroy(testCube);
    }
}