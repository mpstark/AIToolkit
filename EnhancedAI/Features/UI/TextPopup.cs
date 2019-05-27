using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnhancedAI.Features.UI
{
    public class TextPopup
    {
        public readonly GameObject ParentObject;
        private readonly TextMeshProUGUI _text;

        private float _verticalPosition = -150f;
        private float _margin = 20f;
        private float _fontSize = 16f;


        public TextPopup(string name, bool isTooltip)
        {
            ParentObject = new GameObject(name);

            var parentRectTransform = ParentObject.AddComponent<RectTransform>();
            parentRectTransform.SetParent(GameObject.Find("PopupRoot").transform);

            var background = ParentObject.AddComponent<Image>();
            background.color = new Color(0f,0f,0f);

            var textGo = new GameObject("Text");
            var textRectTransform = textGo.AddComponent<RectTransform>();
            textRectTransform.SetParent(parentRectTransform);

            _text = textGo.AddComponent<TextMeshProUGUI>();
            _text.SetText(string.Empty);
            _text.enableWordWrapping = false;
            _text.alignment = TextAlignmentOptions.TopLeft;
            _text.fontSize = _fontSize;
            _text.richText = true;

            if (!isTooltip)
            {
                parentRectTransform.anchorMin = new Vector2(.5f, .5f);
                parentRectTransform.anchorMax = new Vector2(.5f, .5f);
                parentRectTransform.anchoredPosition = new Vector2(0f, _verticalPosition);
                textRectTransform.anchorMin = new Vector2(.5f, .5f);
                textRectTransform.anchorMax = new Vector2(.5f, .5f);
                textRectTransform.anchoredPosition = new Vector2(0f, 0f);
            }
            else
            {
                parentRectTransform.pivot = new Vector2(0, 1);
                ParentObject.AddComponent<TooltipBehavior>();
            }

            Hide();
        }

        public void SetText(string text)
        {
            _text.text = text;

            var containerRectTransform = ParentObject.GetComponent<RectTransform>();
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
            ParentObject.SetActive(true);
        }

        public void Hide()
        {
            SetText("");
            ParentObject.SetActive(false);
        }
    }

    public class TooltipBehavior : MonoBehaviour
    {
        public void LateUpdate()
        {
            transform.position = Input.mousePosition;

            if (Input.GetKeyDown(KeyCode.Escape))
                gameObject.SetActive(false);
        }
    }
}
