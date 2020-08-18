using UnityEngine;

public class BillboardUpdate : MonoBehaviour
{
    private void OnEnable()
    {
        transform.LookAt(BillboardTarget.target);
    }

    private void Update()
    {
        transform.LookAt(BillboardTarget.target);
    }
}