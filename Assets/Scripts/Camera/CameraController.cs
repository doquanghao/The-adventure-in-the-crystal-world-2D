using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    //Đại diện cho mục tiêu mà đoạn mã sẽ theo dõi.
    Transform target;

    //Đại diện cho vận tốc hiện tại, được khởi tạo là zero.
    Vector3 velocity = Vector3.zero;

    // giới hạn giá trị của smoothTime trong khoảng từ 0 đến 1.
    [Range(0,1)]

    // Một biến public được hiển thị trong trình soạn thảo Unity để điều chỉnh thời gian mượt mà. Attribut 
    public float smoothTime;

    public Vector3 positionOffset;

    [Header("Axis Limitation")]
    public Vector2 XLimit;
    public Vector2 YLimit;


    private void Awake()
    {
        //Tìm GameObject có tag "Player" và gán transform của nó vào biến target.
       target= GameObject.FindGameObjectWithTag("Player").transform; 
    }

    private void LateUpdate(){

        // Lấy vị trí của mục tiêu.
        Vector3 targetPosition = target.position+positionOffset;

        targetPosition = new Vector3(Mathf.Clamp(targetPosition.x,XLimit.x,XLimit.y),Mathf.Clamp(targetPosition.y,YLimit.x,YLimit.y),-10);
        //Sử dụng Vector3.SmoothDamp để di chuyển mượt mà vị trí hiện tại của đối tượng về phía vị trí mục tiêu
        //Tham số ref velocity được sử dụng để làm mịn chuyển động, và biến smoothTime xác định tốc độ thích ứng của chuyển động với các thay đổi.
        transform.position=Vector3.SmoothDamp(transform.position,targetPosition,ref velocity,smoothTime);
    }
}
