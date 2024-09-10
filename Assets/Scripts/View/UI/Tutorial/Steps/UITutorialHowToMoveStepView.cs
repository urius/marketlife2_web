using TMPro;
using UnityEngine;

namespace View.UI.Tutorial.Steps
{
    public class UITutorialHowToMoveStepView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _messageText;

        public void SetMessageText(string text)
        {
            _messageText.text = text;
        }
    }
}