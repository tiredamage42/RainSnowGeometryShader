/*
    script to be able to move the camera around using the w,a,s,d keys
    and look around using the arrow keys
*/

using UnityEngine;
public class FreeCamera : MonoBehaviour
{
    public float moveSpeed = 5;
    public float moveSpeedUp = 20;
    public float turnSpeed = 75;

    float GetAxis (KeyCode pos, KeyCode neg) {
        float r = 0;
        if (Input.GetKey(pos)) r += 1;
        if (Input.GetKey(neg)) r -= 1;
        return r;
    }    
    float ClampAngle(float angle, float min, float max) {
        // remap from [0, 360] to [-180, 180]
        return Mathf.Clamp(((angle + 180f) % 360f - 180f), min, max);
    }
    void Update () {
        // handle rotation
        float tSpeed = turnSpeed * Time.deltaTime;
        Vector3 angles = transform.rotation.eulerAngles;

        // clamp up down look, so we cant do somersaults
        angles.x = ClampAngle(angles.x + GetAxis (KeyCode.UpArrow, KeyCode.DownArrow) * tSpeed, -89, 89);
        angles.y += GetAxis (KeyCode.RightArrow, KeyCode.LeftArrow) * tSpeed;
        angles.z = 0;
        transform.rotation = Quaternion.Euler(angles);
        
        // handle movmeent
        Vector3 side = transform.right * GetAxis (KeyCode.D, KeyCode.A);
        Vector3 upDown = transform.up * GetAxis (KeyCode.E, KeyCode.Q);
        Vector3 fwd = transform.forward * GetAxis (KeyCode.W, KeyCode.S);

        float mSpeed = (Input.GetKey(KeyCode.LeftShift) ? moveSpeedUp : moveSpeed) * Time.deltaTime;
        transform.position += (side + upDown + fwd) * mSpeed;
    }        
}