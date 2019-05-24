using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EnhancedAI.Features
{
    public static class AIPausePopup
    {
        //private static GameObject _canvasParent;
        private static GameObject _container;
        private static TextMeshProUGUI _text;

        private static float _verticalPosition = -150f;
        private static float _margin = 20f;
        private static float _fontSize = 16f;
        private static string _parentName = "EnhancedAIBehaviorTreeVisualizationParent";


        public static void Setup()
        {
            //_canvasParent = new GameObject(_parentName);
            //var canvasRectTransform = _canvasParent.AddComponent<RectTransform>();
            //var canvas = _canvasParent.AddComponent<Canvas>();
            //canvas.renderMode = RenderMode.ScreenSpaceOverlay;

            _container = new GameObject("EnhancedAITextContainer");
            var containerRectTransform = _container.AddComponent<RectTransform>();
            containerRectTransform.SetParent(GameObject.Find("PopupRoot").transform);
            containerRectTransform.anchorMin = new Vector2(.5f, .5f);
            containerRectTransform.anchorMax = new Vector2(.5f, .5f);
            containerRectTransform.anchoredPosition = new Vector2(0f, _verticalPosition);
            var background = _container.AddComponent<Image>();
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
        }

        public static void SetText(string text)
        {
            if (GameObject.Find(_parentName) == null)
                Setup();

            _text.text = text;

            var containerRectTransform = _container.GetComponent<RectTransform>();
            var textRectTransform = _text.gameObject.GetComponent<RectTransform>();

            containerRectTransform.sizeDelta = _text.GetPreferredValues() + new Vector2(_margin, _margin);
            textRectTransform.sizeDelta = _text.GetPreferredValues();

            Show();
        }

        public static void Show()
        {
            _container.SetActive(true);
        }

        public static void Hide()
        {
            _container.SetActive(false);
        }
    }
}
