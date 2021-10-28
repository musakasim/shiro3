using System.Collections.Generic;
using System.Linq;
using GalaSoft.MvvmLight.Command;
using KirazControls;
using Reactive.Bindings;
using Shiro.Control;
using Shiro.Controller;
using Shiro.Model;

namespace Shiro.ViewModel
{
    public class KanjiViewModel : MainViewModel
    {
        public KanjiViewModel()
        {
            if (IsInDesignMode)
            {
                Kanji = new ReactiveProperty<KanjiInfo>(DesignTimeData.GetKanjiDic2Entry1());
                KanjiCompounds = new ReactiveProperty<IEnumerable<ShiroEntryBzzt>>(DesignTimeData.GetDictionaryEntries());
            }
        }

        public KanjiViewModel(ReactiveProperty<KanjiInfo> selectedKanji)
        {
            if (!IsInDesignMode)
            {
                Kanji = new ReactiveProperty<KanjiInfo>();
                KanjiCompounds = new ReactiveProperty<IEnumerable<ShiroEntryBzzt>>();

                ShiroDictionaryController = new ShiroDictionaryController();

                Kanji = selectedKanji.ToReactiveProperty();

                ShowAnimationCommand = new RelayCommand(() =>
                {
                    ModalContentPresenter.ShowModalContent();
                    AnimationElement.BeginAsync();
                });
                HideAnimationCommand = new RelayCommand(() => ModalContentPresenter.HideModalContent());

                // todo: asagidaki PropertyChanged@lar yerine subscription kullanilacak, eg:
                //ModalContentPresenter = BoundView.Select(t => t != null ? (ModalContentPresenter) BoundView.Value.GetFramworkElementByName("ModalPresenter") : null).ToReactiveProperty().Value;
                //AnimationElement = BoundView.Select(t => t != null ? (PathDrawingAnimationCanvas)BoundView.Value.GetFramworkElementByName("DrawingAnimationCanvas") : null).ToReactiveProperty().Value;

                BoundView.PropertyChanged += (sender, args) =>
                {
                    if (BoundView.Value != null)
                    {
                        ModalContentPresenter = (ModalContentPresenter)BoundView.Value.GetFramworkElementByName("ModalPresenter");

                        AnimationElement = (PathDrawingAnimationCanvas)BoundView.Value.GetFramworkElementByName("DrawingAnimationCanvas");
                        AnimationElement.MouseDown += (o, eventArgs) => AnimationElement.BeginAsync();
                    }
                };

                Kanji.PropertyChanged += (sender, args) =>
                {
                    if (Kanji != null && Kanji.Value != null)
                    {
                        ModalContentPresenter.HideModalContent();
                    }
                };

                Kanji.PropertyChanged += (sender, args) =>
                {
                    if (Kanji.Value != null)
                        KanjiCompounds.Value = ShiroDictionaryController.Search(Kanji.Value.Kanji).Select(u => new ShiroEntryBzzt(u));
                };
            }
        }

        private ModalContentPresenter ModalContentPresenter { get; set; }
        private PathDrawingAnimationCanvas AnimationElement { get; set; }


        public ReactiveProperty<KanjiInfo> Kanji { get; set; }

        public ReactiveProperty<IEnumerable<ShiroEntryBzzt>> KanjiCompounds { get; set; }

        public RelayCommand ShowAnimationCommand { get; set; }
        public RelayCommand HideAnimationCommand { get; set; }

    }
}