using TMPro;
using UnityEngine;

namespace View.Game.BuildPoint
{
    public class BuildPointView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void SetText(string text)
        {
            _text.text = text;
        }
    }
}