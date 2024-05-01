using UnityEngine;

namespace View.UI
{
    public class UIRootView : MonoBehaviour
    {
        [SerializeField] private Transform _gamePanelTransform;
        
        public Transform UIGameOverlayPanelTransform => _gamePanelTransform;
    }
}