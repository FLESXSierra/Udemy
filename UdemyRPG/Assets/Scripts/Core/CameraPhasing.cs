using UnityEngine;

namespace RPG.Core
{
    public class CameraPhasing : MonoBehaviour
    {
        void Update()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }

}