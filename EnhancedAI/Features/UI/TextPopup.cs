using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnhancedAI.Features.UI
{
    public class TextPopup
    {
        public readonly GameObject GameObject;
        private readonly TextMeshProUGUI _text;

        private float _verticalPosition = -150f;
        private float _margin = 20f;
        private float _fontSize = 16f;


        public TextPopup(string name)
        {
            GameObject = new GameObject(name);
            var containerRectTransform = GameObject.AddComponent<RectTransform>();
            containerRectTransform.SetParent(GameObject.Find("PopupRoot").transform);
            containerRectTransform.anchorMin = new Vector2(.5f, .5f);
            containerRectTransform.anchorMax = new Vector2(.5f, .5f);
            containerRectTransform.anchoredPosition = new Vector2(0f, _verticalPosition);
            var background = GameObject.AddComponent<Image>();
            background.color = new Color(0f,0f,0f);

            var textGo = new GameObject("Text");
            var textRectTransform = textGo.AddComponent<RectTransform>();
            textRectTransform.SetParent(containerRectTransform);
            textRectTransform.anchorMin = new Vector2(.5f, .5f);
            textRectTransform.anchorMax = new Vector2(.5f, .5f);
            textRectTransform.anchoredPosition = new Vector2(0f, 0f);

            _text = textGo.AddComponent<TextMeshProUGUI>();
            _text.SetText(string.Empty);
            _text.enableWordWrapping = false;
            _text.alignment = TextAlignmentOptions.TopLeft;
            _text.fontSize = _fontSize;

            Hide();
        }

        public void SetText(string text)
        {
            _text.text = text;

            var containerRectTransform = GameObject.GetComponent<RectTransform>();
            var textRectTransform = _text.gameObject.GetComponent<RectTransform>();

            containerRectTransform.sizeDelta = _text.GetPreferredValues() + new Vector2(_margin, _margin);
            textRectTransform.sizeDelta = _text.GetPreferredValues();

            Show();
        }

        public void AppendText(string text)
        {
            if (string.IsNullOrEmpty(_text.text))
                SetText(text);
            else
                SetText($"{_text.text}\n{text}");
        }

        public void Show()
        {
            GameObject.SetActive(true);
        }

        public void Hide()
        {
            SetText("");
            GameObject.SetActive(false);
        }
    }
}
