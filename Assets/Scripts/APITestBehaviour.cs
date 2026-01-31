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
            CallLotsOfUnityEngineAPIs();
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
    
    public void CallLotsOfUnityEngineAPIs()
    {
        // ===== Debug =====
        Debug.Log("UnityEngine API Sampler");
        Debug.LogWarning("Warning log");
        Debug.LogError("Error log");
        Debug.DrawLine(Vector3.zero, Vector3.one, Color.red, 1f);

        // ===== Time =====
        float deltaTime = Time.deltaTime;
        float time = Time.time;
        int frameCount = Time.frameCount;
        float timeScale = Time.timeScale;

        // ===== Mathf =====
        float a = Mathf.Sin(time);
        float b = Mathf.Cos(time);
        float c = Mathf.Clamp(a, -1f, 1f);
        float d = Mathf.Lerp(0f, 10f, 0.5f);
        float e = Mathf.Abs(-10f);
        int f = Mathf.RoundToInt(3.6f);

        // ===== Random =====
        float randFloat = Random.Range(0f, 1f);
        int randInt = Random.Range(0, 10);
        Vector3 randInsideSphere = Random.insideUnitSphere;

        // ===== Input =====
        bool mouseDown = Input.GetMouseButton(0);
        Vector3 mousePos = Input.mousePosition;
        float horizontal = Input.GetAxis("Horizontal");

        // ===== Screen =====
        int screenWidth = Screen.width;
        int screenHeight = Screen.height;
        bool isFullscreen = Screen.fullScreen;

        // ===== Application =====
        string dataPath = Application.dataPath;
        string persistentPath = Application.persistentDataPath;
        bool isPlaying = Application.isPlaying;
        RuntimePlatform platform = Application.platform;

        // ===== Transform =====
        Transform t = transform;
        Vector3 position = t.position;
        Quaternion rotation = t.rotation;
        t.Translate(Vector3.forward * deltaTime);
        t.Rotate(Vector3.up, 30f * deltaTime);

        // ===== GameObject =====
        GameObject go = gameObject;
        bool activeSelf = go.activeSelf;
        go.SetActive(true);
        string tag = go.tag;
        CompareTag("Player");

        // ===== Component =====
        Rigidbody rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(Vector3.up * 5f, ForceMode.Impulse);
            rb.velocity = Vector3.ClampMagnitude(rb.velocity, 10f);
        }

        // ===== Physics =====
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 10f))
        {
            Debug.Log("Hit object: " + hit.collider.name);
        }

        // ===== Camera =====
        Camera mainCam = Camera.main;
        if (mainCam != null)
        {
            Vector3 viewportPos = mainCam.WorldToViewportPoint(transform.position);
            Vector3 screenPos = mainCam.WorldToScreenPoint(transform.position);
        }

        // ===== Color =====
        Color color = Color.Lerp(Color.red, Color.blue, 0.5f);

        // ===== Vector / Quaternion =====
        Vector3 v = Vector3.Lerp(Vector3.zero, Vector3.one, 0.3f);
        float dot = Vector3.Dot(Vector3.up, Vector3.forward);
        Quaternion q = Quaternion.Euler(0f, 90f, 0f);

        // ===== Object =====
        Object self = this;
        string name = self.name;
        bool destroyed = self == null;
    }
}