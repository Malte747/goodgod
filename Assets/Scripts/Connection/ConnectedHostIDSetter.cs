using TMPro;
using UnityEngine;

public class ConnectedHostIDSetter : MonoBehaviour
{
    private void Start()
    {
        if(TryGetComponent(out TextMeshProUGUI tmp))
        tmp.text = ConnectionManager.GetHostHex();
       
    }
}
