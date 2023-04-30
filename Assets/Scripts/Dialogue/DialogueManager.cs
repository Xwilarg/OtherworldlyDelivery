using LudumDare53.Audio;
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
        private Image _nameContainer;

        [SerializeField]
        private NameAssociation<Sprite>[] _sprites;

        [SerializeField]
        private NameAssociation<AudioClip>[] _sounds;

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
                return;
            }
            _callback = () =>
            {
                BGMManager.Instance.SetBGM(_gameClip);
                _cardsManager.SpawnCards();
            };
            _gameUI.SetActive(false);
            _nameContainer.gameObject.SetActive(false);
            _lines = _story.text.Replace("\r", "").Split('\n', StringSplitOptions.RemoveEmptyEntries);
            ShowNext();
        }

        private void Start()
        {
            if (!_isEnabled)
            {
                _cardsManager.SpawnCards();
            }
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
                    _nameContainer.gameObject.SetActive(!string.IsNullOrEmpty(_nameText.text));
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
                        _previewSprite.preserveAspect = true;
                    }
                }
                else if (txt.StartsWith("COLOR:"))
                {
                    var target = txt[6..];
                    _nameText.color = _colors[target];
                }
                else if (txt.StartsWith("SFX:"))
                {
                    var target = txt[4..];
                    BGMManager.Instance.PlayOneShot(_sounds.FirstOrDefault(x => x.Name == target).Sprite);
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
