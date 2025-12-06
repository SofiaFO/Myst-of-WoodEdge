using UnityEngine;

public class ActivateObjectOnAnimEnd : MonoBehaviour
{
    public GameObject objectToActivate;

    // Esse método será chamado no último frame da animação
    public void ActivateAndDisableSelf()
    {
        if (objectToActivate != null)
            objectToActivate.SetActive(true);

        gameObject.SetActive(false);
    }
}
