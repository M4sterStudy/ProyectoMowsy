using UnityEngine;

public class ShowHideObject : MonoBehaviour
{
    // Function to show a GameObject
    public void ShowObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(true);
        }
        else
        {
            Debug.LogError("The GameObject to show is null.");
        }
    }

    // Function to hide a GameObject
    public void HideObject(GameObject obj)
    {
        if (obj != null)
        {
            obj.SetActive(false);
        }
        else
        {
            Debug.LogError("The GameObject to hide is null.");
        }
    }
}
