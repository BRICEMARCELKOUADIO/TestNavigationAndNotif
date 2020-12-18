using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Kola.Infrastructure.Controls
{
    public class MaskedBehavior : Behavior<Entry>
        {
            private const string numberRegex = @"^[a-zA-Z0-9]+$";

            private string _mask = "";
            public string Mask
            {
                get => _mask;
                set
                {
                    _mask = value;
                    SetPositions();
                }
            }

            private string _separator1 = "";
            public string Separator1
            {
                get => _separator1;
                set
                {
                    _separator1 = value;
                }
            }

            private string _separator2 = "";
            public string Separator2
            {
                get => _separator2;
                set
                {
                    _separator2 = value;
                }
            }

            protected override void OnAttachedTo(Entry entry)
            {
                entry.TextChanged += OnEntryTextChanged;
                base.OnAttachedTo(entry);
            }

            protected override void OnDetachingFrom(Entry entry)
            {
                entry.TextChanged -= OnEntryTextChanged;
                base.OnDetachingFrom(entry);
            }

            IDictionary<int, char> _positions;

            void SetPositions()
            {
                if (string.IsNullOrEmpty(Mask))
                {
                    _positions = null;
                    return;
                }

                var list = new Dictionary<int, char>();
                for (var i = 0; i < Mask.Length; i++)
                    if (Mask[i] != 'X')
                        list.Add(i, Mask[i]);

                _positions = list;
            }


            private void OnEntryTextChanged(object sender, TextChangedEventArgs args)
            {
                var entry = sender as Entry;

                var text = entry?.Text?.ToUpper();


                if (string.IsNullOrEmpty(text) || string.IsNullOrWhiteSpace(text) || _positions == null)
                    return;

                string str = text;
                if (Separator1 != "")
                    str = text.Replace(Separator1, "");
                if (Separator2 != "")
                    str = str.Replace(Separator2, "");

                bool isValidText = Regex.IsMatch(str, numberRegex, RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));

                if (!isValidText)
                {
                    entry.Text = text.Remove(text.Length - 1);
                    return;
                }

                if (text.Length > _mask.Length)
                {
                    entry.Text = text.Remove(text.Length - 1);
                    return;
                }

                foreach (var position in _positions)
                    if (text.Length >= position.Key + 1)
                    {
                        var value = position.Value.ToString();
                        if (text.Substring(position.Key, 1) != value)
                            text = text.Insert(position.Key, value);
                    }

                if (entry.Text != text)
                    entry.Text = text;
            }
        }
    }

