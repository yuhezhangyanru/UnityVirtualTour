using UnityEngine;
using System.Collections;
/// <summary>
/// function:绑定在摄像机上用来跟随空箱子
/// </summary>
public class FollowPlayer : MonoBehaviour
{
    /// <summary>
    /// 每次 俯仰程度改变的幅度 基础值
    /// </summary>
    public const float HEIGHT_MOVE_SUB_BASE = 0.1f;//06f;
    /// <summary>
    /// 相机的最小距离
    /// </summary>
    public const float DISTANCE_MIN = 2.0f;

    /// <summary>
    /// 相机的最大距离
    /// </summary>
    public const float DISTANCE_MAX = 50;

    public const float HEIGHT_MIN = 0.5f;

    /// <summary>
    /// 镜头远近每次调整的大小
    /// </summary>
    public const float MOVE_DISTANCE_SUB = 0.1f;

    /// <summary>
    /// 以下是关于镜头跟随的一些参数
    /// </summary>
    private Transform player;
    /// <summary>
    /// 相机距离玩家的距离
    /// </summary>
    public float distance = 2.0f;
    // the height we want the camera to be above the target
    /// <summary>
    /// 相机跟随玩家的高度
    /// </summary>
    public float height = 0.5f;// 2.0f;
                               // How much we 
    public float heightDamping = 2.0f;
    public float rotationDamping = 3.0f;

    private Vector3 offsetPosition;//位置偏移
    private int MouseWheelSensitivity = 10;
    private int maxCamFov = 90;
    private int minCamFov = 10;
    public float rotateSpeed = 2;
    float wantedRotationAngle;
    float wantedHeight;
    bool isOriginalwanted = true;

    // Use this for initialization
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").transform;
        wantedRotationAngle = player.eulerAngles.y;
        wantedHeight = player.position.y + height;
    }

    // Update is called once per frame
    void Update()
    {
        // Early out if we don't have a target
        if (!player)
            return;

        if (Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float fov = Camera.main.fieldOfView;
            fov += Input.GetAxis("Mouse ScrollWheel") * MouseWheelSensitivity;
            fov = Mathf.Clamp(fov, minCamFov, maxCamFov);
            Camera.main.fieldOfView = fov;

        }
        else if (Input.GetMouseButton(1))
        {
            isOriginalwanted = false;
            transform.RotateAround(player.position, player.up, rotateSpeed * Input.GetAxis("Mouse X"));
            Vector3 originalPos = transform.position;
            Quaternion originalRotation = transform.rotation;
            transform.RotateAround(player.position, transform.right, -rotateSpeed * Input.GetAxis("Mouse Y"));


            float x = transform.eulerAngles.x;
            if (x < 10 || x > 80)
            {
                transform.position = originalPos;

                transform.rotation = originalRotation;
            }
        }

        if (isOriginalwanted)
        {

            // Calculate the current rotation angles
            wantedHeight = player.position.y + height;
            wantedRotationAngle = player.eulerAngles.y;
            // Debug.Log(wantedRotationAngle + "+++" + wantedHeight);
            float currentRotationAngle = transform.eulerAngles.y;
            float currentHeight = transform.position.y;

            // Damp the rotation around the y-axis
            currentRotationAngle = Mathf.LerpAngle(currentRotationAngle, wantedRotationAngle, rotationDamping * Time.deltaTime);

            // Damp the height
            currentHeight = Mathf.Lerp(currentHeight, wantedHeight, heightDamping * Time.deltaTime);

            // Convert the angle into a rotation
            Quaternion currentRotation = Quaternion.Euler(0, currentRotationAngle, 0);

            // Set the position of the camera on the x-z plane to:
            // distance meters behind the target
            transform.position = player.position;
            transform.position -= currentRotation * Vector3.forward * distance;

            // Set the height of the camera

            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
            // Always look at the target
            transform.LookAt(player);

        }
        else
        {
            float currentHeight = transform.position.y;
            Quaternion rotation = Quaternion.LookRotation(player.position - transform.position);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 6.0f);
            transform.position = player.position;
            transform.position -= transform.rotation * Vector3.forward * distance;
            transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
            transform.LookAt(player);
        }
        RaycastHit hit;
        if (Physics.Linecast(player.position + Vector3.up, transform.position, out hit))
        {
            string name = hit.collider.gameObject.tag;
            if (name != "MainCamera")
            {
                float currentDistance = Vector3.Distance(hit.point, player.position);//如果射线碰撞的不是相机，那么就取得射线碰撞点到玩家的距离
                if (currentDistance < distance)//如果射线碰撞点小于玩家与相机本来的距离，就说明角色身后是有东西，为了避免穿墙，就把相机位置拉近
                {
                    transform.position = hit.point;
                }
            }
        }
    }
}