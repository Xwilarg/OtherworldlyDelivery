﻿using LudumDare53.Dialogue;
using LudumDare53.Game;
using LudumDare53.SO;
using System;
using TMPro;
using UnityEngine;

namespace LudumDare53.Card
{
    public class CardInstance : MonoBehaviour
    {
        [SerializeField]
        private TMP_Text _title, _content;

        private CardInfo _info;
        public CardInfo Info
        {
            set
            {
                _info = value;
                _title.text = value.Title;
                _content.text = "Content Text";
            }
            get => _info;
        }

        public void OnClick()
        {
            CardsManager.Instance.DoAction(Info);
        }
    }
}
