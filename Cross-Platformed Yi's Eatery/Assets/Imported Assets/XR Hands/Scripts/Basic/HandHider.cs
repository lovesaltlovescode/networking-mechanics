using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HandHider : MonoBehaviour
{
    public GameObject handMesh = null;
    public XRInteractorLineVisual rayVisual = null;
    [SerializeField] private XRRayInteractor rayInteractor = null;
    [SerializeField] private XRDirectInteractor directInteractor = null;

    private void OnEnable()
    {
        if (rayInteractor != null)
        {
            rayInteractor.onSelectEnter.AddListener(Hide);
            rayInteractor.onSelectExit.AddListener(Show);
        } 
        else if(directInteractor != null)
        {
            directInteractor.onSelectEnter.AddListener(Hide);
            directInteractor.onSelectExit.AddListener(Show);
        }
    }

    private void OnDisable()
    {
        if (rayInteractor != null)
        {
            rayInteractor.onSelectEnter.RemoveListener(Hide);
            rayInteractor.onSelectExit.RemoveListener(Show);
        }
        else if (directInteractor != null)
        {
            directInteractor.onSelectEnter.RemoveListener(Hide);
            directInteractor.onSelectExit.RemoveListener(Show);
        }
    }

    private void Show(XRBaseInteractable interactable)
    {
        ToggleRay(true);
        ToggleDirectInteractor(true);
    }

    private void Hide(XRBaseInteractable interactable)
    {
        ToggleRay(false);
        ToggleDirectInteractor(false);
    }

    public void ToggleRay(bool enable, bool onlyVisual = true)
    {
        if (rayInteractor != null && !onlyVisual && !rayInteractor.isSelectActive)
            rayInteractor.enabled = enable;

        if (rayVisual != null)
            rayVisual.enabled = enable;

        if (handMesh != null)
            handMesh.SetActive(enable);
    }

    public void ToggleDirectInteractor(bool enable, bool onlyVisual = true)
    {
        if (directInteractor != null && !onlyVisual && !directInteractor.isSelectActive)
            directInteractor.enabled = enable;

        if (handMesh != null)
            handMesh.SetActive(enable);
    }
}
