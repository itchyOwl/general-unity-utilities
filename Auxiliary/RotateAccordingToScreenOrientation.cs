using UnityEngine;

namespace ItchyOwl.Auxiliary
{
    public class RotateAccordingToScreenOrientation : MonoBehaviour
    {
        void Update()
        {
            switch (Screen.orientation)
            {
                case ScreenOrientation.LandscapeRight:
                    transform.localRotation = Quaternion.Euler(0, 0, 0);
                    break;
                case ScreenOrientation.Portrait:
                    transform.localRotation = Quaternion.Euler(0, 0, 90);
                    break;
                case ScreenOrientation.LandscapeLeft:
                    transform.localRotation = Quaternion.Euler(0, 0, 180);
                    break;
                case ScreenOrientation.PortraitUpsideDown:
                    transform.localRotation = Quaternion.Euler(0, 0, 270);
                    break;
                default:
                    transform.localRotation = Quaternion.identity;
                    break;
            }
        }
    }
}

