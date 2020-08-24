using UnityEngine;

public class Billboard : MonoBehaviour
{
    private void OnEnable()
    {
        transform.LookAt(BillboardTarget.target);
    }
}