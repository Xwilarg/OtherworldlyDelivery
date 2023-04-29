using LudumDare53.Card;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace LudumDare53.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance { get; private set; }

        [Header("Debug")]
        [SerializeField]
        private bool _isEnabled;

        [Header("Configuration")]
        [SerializeField]
        private TextAsset _story;

        [SerializeField]
        private GameObject _textContainer;

        [SerializeField]
        private TMP_Text _dialogueText;

        [SerializeField]
        private TMP_Text _nameText;

        [SerializeField]
        private NameAssociation[] _sprites;

        [SerializeField]
        private SpriteRenderer _door;

        [SerializeField]
        private Sprite _openedDoor;

        [SerializeField]
        private GameObject _preview;

        [SerializeField]
        private Image _previewSprite;

        [SerializeField]
        private SpriteRenderer _spirit;

        [SerializeField]
        private GameObject _gameUI;

        [SerializeField]
        private AudioSource _bgm;

        [SerializeField]
        private AudioClip _gameClip;

        private string[] _lines;
        private int _index;

        private CardsManager _cardsManager;

        private Action _callback;

        private void Awake()
        {
            Instance = this;
            _cardsManager = GetComponent<CardsManager>();
            if (!_isEnabled)
            {
                _textContainer.SetActive(false);
                _cardsManager.SpawnCards();
                return;
            }
            _callback = _cardsManager.SpawnCards;
            _gameUI.SetActive(false);
            _lines = _story.text.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            ShowNext();
        }

        private readonly Dictionary<string, Color> _colors = new()
        {
            { "GREY", Color.grey },
            { "BLUE", Color.blue },
            { "NONE", Color.black }
        };

        public void ShowText(string name, string color, string text, Action callback)
        {
            _textContainer.SetActive(true);
            _isEnabled = true;
            _lines = new[]
            {
                $"NAME:{name}",
                $"COLOR:{color}",
                text
            };
            _index = 0;
            _callback = callback;
            ShowNext();
        }

        public void ShowNext()
        {
            if (_index == _lines.Length)
            {
                _textContainer.SetActive(false);
                _gameUI.SetActive(true);
                var pos = _bgm.time;
                _bgm.clip = _gameClip;
                _bgm.Play();
                _bgm.time = pos;
                _callback?.Invoke();
                return;
            }
            while (true)
            {
                var txt = _lines[_index];
                _index++;
                if (txt.StartsWith("NAME:"))
                {
                    _nameText.text = txt[5..];
                }
                else if (txt.StartsWith("DOOR:"))
                {
                    if (txt[5..] == "OPEN")
                    {
                        _door.sprite = _openedDoor;
                    }
                }
                else if (txt.StartsWith("SHOW:"))
                {
                    var target = txt[5..];
                    if (target == "NONE")
                    {
                        _preview.SetActive(false);
                    }
                    else
                    {
                        _preview.SetActive(true);
                        _previewSprite.sprite = _sprites.FirstOrDefault(x => x.Name == target).Sprite;
                    }
                }
                else if (txt.StartsWith("COLOR:"))
                {
                    var target = txt[6..];
                    _nameText.color = _colors[target];
                }
                else if (txt.StartsWith("SPIRIT:"))
                {
                    var target = txt[7..];
                    if (target == "NONE")
                    {
                        _spirit.gameObject.SetActive(false);
                    }
                    else
                    {
                        _spirit.gameObject.SetActive(true);
                        _spirit.color = _colors[target];
                    }
                }
                else
                {
                    _dialogueText.text = txt;
                    break;
                }
            }
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed && _isEnabled && _textContainer.activeInHierarchy)
            {
                ShowNext();
            }
        }
    }
}
