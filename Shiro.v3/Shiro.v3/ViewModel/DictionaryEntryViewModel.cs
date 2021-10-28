using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using GalaSoft.MvvmLight.Messaging;
using Reactive.Bindings;
using Shiro.Model;

namespace Shiro.ViewModel
{
    public class DictionaryEntryViewModel : MainViewModel
    {
        public class SearchDictMessage
        {
            public string SearchText { get; set; }
        }

        public DictionaryEntryViewModel()
        {
            if (IsInDesignMode)
            {
                DictionaryEntry = new ReactiveProperty<ShiroEntryBzzt>(DesignTimeData.GetDictionaryEntry1());
                ExampleSentences = new ReactiveProperty<List<TatoebaSentence>>(new List<TatoebaSentence> { DesignTimeData.GetExampleSentence1() });
                BookmarkMiniViewModel = new BookmarkMiniViewModel();
            }
            else
            {
                // Xaml'da
                //    <UserControl.Resources>
                //          < viewModel1:DictionaryEntryViewModel x:Key = "DictionaryEntryViewModelProxy" />
                //    </ UserControl.Resources >
                // kısmında parametresiz olarak xaml parse edildiğinde oluşturulacak(default ctor kullanılarak oluşur)
                // DictionaryEntryViewModel bu MouseMove event binding'i için eklendi.
                // todo: harici bir class'a alalım mı? eg:ViewModelMouseBasedExtensions : MainViewModel
                // event binding from UI
                MouseMoveOnTextBox = new ReactiveProperty<MouseEventArgs>(mode: ReactivePropertyMode.None);
                MouseMoveOnTextBox.Subscribe(TextBoxSelectWordUnderMouse);
            }
        }

        public DictionaryEntryViewModel(ReactiveProperty<ShiroEntryBzzt> dictionaryEntry)
        {
            if (!IsInDesignMode)
            {
                DictionaryEntry = dictionaryEntry.ToReactiveProperty();
                ExampleSentences = DictionaryEntry
                    .Select(t => t?.Spellings.SelectMany(p => TatoebaController.Search(p.Value)).ToList() ?? new List<TatoebaSentence>())
                    .ToReactiveProperty();
                BookmarkMiniViewModel = new BookmarkMiniViewModel(DictionaryEntry);


                SelectedSentenceToken = new ReactiveProperty<SentenceToken>();

                //SelectedSentenceToken.Subscribe(t =>
                //{
                //    Messenger.Default.Send(new SearchDictMessage { SearchText = t.Token });
                //});

                SelectedSentenceToken.PropertyChanged += (sender, args) =>
                {
                    if (SelectedSentenceToken.Value != null)
                        //todo:Rx MessageBroker'ı kullan
                        Messenger.Default.Send(new SearchDictMessage { SearchText = SelectedSentenceToken.Value.Token });
                };
            }
        }

        public BookmarkMiniViewModel BookmarkMiniViewModel { get; set; }
        public ReactiveProperty<ShiroEntryBzzt> DictionaryEntry { get; set; }
        public ReactiveProperty<List<TatoebaSentence>> ExampleSentences { get; set; }

        public ReactiveProperty<MouseEventArgs> MouseMoveOnTextBox { get; private set; }

        public ReactiveProperty<SentenceToken> SelectedSentenceToken { get; set; }

        /// <summary>
        ///     textbox uzerine fare gezdirildiginde fare imleci altindaki kelimeyi secer
        ///     MouseMove event'ına bağlanmalı
        /// </summary>
        private void TextBoxSelectWordUnderMouse(MouseEventArgs mouseEventArgs)
        {
            Console.WriteLine($"TextBoxMouseMove:{mouseEventArgs.LeftButton}");
            var textBox = mouseEventArgs.Source as TextBox;
            Point mousePoint = Mouse.GetPosition(textBox);
            if (textBox == null)
                return;
            int charPosition = textBox.GetCharacterIndexFromPoint(mousePoint, true);
            Console.WriteLine($"TextBoxMouseMove charPosition:{charPosition}");
            if (charPosition > 0)
            {
                textBox.Focus();
                int index = 0;
                int i = 0;
                var text = textBox.Text;
                //todo:specialChars parametre olarak gelsin
                var specialChars = new List<char> { '【', '】', ',' };
                specialChars.ForEach(a => text = text.Replace(a, ' '));
                var strings = text.Split(' ');
                while (index + strings[i].Length < charPosition && i < strings.Length)
                {
                    index += strings[i].Length + 1;
                    i++;
                }
                textBox.Select(index, strings[i].Length);
            }
        }
    }
}