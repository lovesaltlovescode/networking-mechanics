using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Mould : MonoBehaviour
{
    public SkinnedMeshRenderer skinMeshRend;
    XRGrabInteractable interactable;

    int mouldPhase;
    void Awake()
    {
        interactable = GetComponent<XRGrabInteractable>();

        interactable.onActivate.AddListener(Animate);
    }

    void Start()
    {
        mouldPhase = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Animate(XRBaseInteractor interactor)
    {
        switch(mouldPhase)
        {
            case 0:
                StartCoroutine("Shape", mouldPhase);
                mouldPhase++;
                Debug.Log("case 0 reached");
                break;
            case 1:
                StartCoroutine("Shape", mouldPhase);
                mouldPhase++;
                StopCoroutine("Shape");
                break;
        }
    }

    IEnumerator Shape(int index)
    {
        while (skinMeshRend.GetBlendShapeWeight(index) < 100)
        {
            float bsWeight = Mathf.Lerp(0, 100, 75 * Time.deltaTime);
            skinMeshRend.SetBlendShapeWeight(index, bsWeight);
            yield return null;
            Debug.Log("ran coroutine loop");
        }
    }
}
