using System;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Reactive.Bindings;
using Shiro.Controller;
using Shiro.Library;
using Shiro.Library.Extensions;

namespace Shiro.ViewModel
{
    public class DbPopulatorViewModel : MainViewModel
    {
        //public List<KanjiInfo> Kanjis { get; set; }
        //public RangeObservableCollection<Word> WordsInFile { get; set; }
        //public RangeObservableCollection<Word> WordsOnDb { get; set; }

        public ReactiveProperty<string> KanjiVgFilePath { get; set; }
        public ReactiveProperty<string> KanjiDic2FilePath { get; set; }
        public ReactiveProperty<string> TatoebaSentencesFilePath { get; set; }
        public ReactiveProperty<string> TatoebaSentenceLinksFilePath { get; set; }
        public ReactiveProperty<string> JMdictFilePath { get; set; }

        public ReactiveProperty<int> DictionaryPopulationProgress { get; set; }
        public ReactiveProperty<int> KanjiInfoPopulationProgress { get; set; }
        public ReactiveProperty<int> TatoebaSentencesPopulationProgress { get; set; }
        public ReactiveProperty<int> TatoebaSentencesTokenizationProgress { get; set; }
        public ReactiveProperty<int> TatoebaSentencesCount { get; set; }
        public ReactiveProperty<int> TatoebaSentencesUntokenizedCount { get; set; }

        //public ReactiveProperty<string> CollectionFilePath { get; set; }
        //public ReactiveProperty<string> CollectionName { get; set; }

        private DbPopulator DbPopulator { get; set; }

        #region Commands

        //public RelayCommand CollectionFileSelectCommand { get; set; }
        //public RelayCommand PopulateDbWithCollectionFileDataCommand { get; set; }
        //public RelayCommand RenameCollectionFileCollectionNameCommand { get; set; }
        //public RelayCommand DeleteCollectionCommand { get; set; }
        public RelayCommand KanjiVgClearAllDataCommand { get; set; }
        public RelayCommand KanjiDic2FileSelectCommand { get; set; }
        public RelayCommand KanjiVgFileSelectCommand { get; set; }
        public RelayCommand JMDictClearAllDataCommand { get; set; }
        public RelayCommand JMDictFileSelectCommand { get; set; }
        public RelayCommand TatoebaSentencesClearAllDataCommand { get; set; }
        public RelayCommand TatoebaSentencesClearSentenceTokensCommand { get; set; }
        public RelayCommand TatoebaSentencesFileSelectCommand { get; set; }
        public RelayCommand TatoebaSentenceLinksFileSelectCommand { get; set; }

        public RelayCommand PopulateDictionaryCommand { get; set; }
        public RelayCommand PopulateKanjiInfoCommand { get; set; }
        public RelayCommand PopulateExampleSentencesCommand { get; set; }
        public RelayCommand UpdateSentenceTokenInfoCommand { get; set; }

        #endregion

        public DbPopulatorViewModel()
        {
            if (!IsInDesignMode)
            {
                DbPopulator = new DbPopulator();

                //Observable'da önce nesne oluştur
                //WordsInFile = new RangeObservableCollection<Word>();
                //WordsOnDb = new RangeObservableCollection<Word>();

                PopulateDictionaryCommand = new RelayCommand(
                    () => DbPopulator.PopulateDictionaryDataOnDb(JMdictFilePath.Value, new Progress<int>(p => DictionaryPopulationProgress.Value = p)).Forget(),
                    () => !string.IsNullOrEmpty(JMdictFilePath.Value) &&            //canExecute condition, JMdict file must me selected
                          IsProgressCompleted(DictionaryPopulationProgress.Value)); //canExecute condition, should not be already running

                PopulateKanjiInfoCommand = new RelayCommand(
                    () => DbPopulator.PopulateKanjiInfoDataOnDb(KanjiDic2FilePath.Value, KanjiVgFilePath.Value, new Progress<int>(p => KanjiInfoPopulationProgress.Value = p)).Forget(),
                    () => !string.IsNullOrEmpty(KanjiVgFilePath.Value) &&           //canExecute condition, KanjiVg file must me selected
                          !string.IsNullOrEmpty(KanjiDic2FilePath.Value) &&         //canExecute condition, KanjiDic2 file must me selected
                          IsProgressCompleted(KanjiInfoPopulationProgress.Value));  //canExecute condition, should not be already running

                PopulateExampleSentencesCommand = new RelayCommand(
                    () => DbPopulator.PopulateExampleSentencesOnDb(TatoebaSentencesFilePath.Value, TatoebaSentenceLinksFilePath.Value, new Progress<int>(p => TatoebaSentencesPopulationProgress.Value = p))
                                     .ContinueWith(t => TatoebaSentencesCount.Value = t.Result),
                    () => !string.IsNullOrEmpty(TatoebaSentencesFilePath.Value) &&       //canExecute condition, sentences file must me selected
                          !string.IsNullOrEmpty(TatoebaSentenceLinksFilePath.Value) &&   //canExecute condition, sentenceLinks file must me selected
                          IsProgressCompleted(TatoebaSentencesPopulationProgress.Value));//canExecute condition, should not be already running

                UpdateSentenceTokenInfoCommand = new RelayCommand(
                    () => DbPopulator.UpdateSentenceTokens(new Progress<int>(p => TatoebaSentencesTokenizationProgress.Value = p)).Forget());

                //ClearAllData commands:
                KanjiVgClearAllDataCommand = new RelayCommand(() => KanjiInfoController.ClearAllData());
                JMDictClearAllDataCommand = new RelayCommand(() => ShiroDictionaryController.ClearAllData());
                TatoebaSentencesClearAllDataCommand = new RelayCommand(() =>
                    {
                        //wipe away all sentences and refresh sentence count(expected 0 sentences)
                        TatoebaController.ClearAllData();
                        TatoebaSentencesCount.Value = TatoebaController.GetSentenceCount();
                        TatoebaSentencesUntokenizedCount.Value = TatoebaController.GetUntokenizedCount();
                    });
                TatoebaSentencesClearSentenceTokensCommand =new RelayCommand(() =>
                    Task.Run(()=>TatoebaController.ClearSentenceTokens())
                        .ContinueWith(_=> TatoebaSentencesUntokenizedCount.Value = TatoebaController.GetUntokenizedCount()));

                //File select commands:
                KanjiVgFileSelectCommand = new RelayCommand(() => KanjiVgFilePath.Value = FileHelper.ShowFileSelectDialog());
                KanjiDic2FileSelectCommand = new RelayCommand(() => KanjiDic2FilePath.Value = FileHelper.ShowFileSelectDialog());
                JMDictFileSelectCommand = new RelayCommand(() => JMdictFilePath.Value = FileHelper.ShowFileSelectDialog());
                TatoebaSentencesFileSelectCommand = new RelayCommand(() => TatoebaSentencesFilePath.Value = FileHelper.ShowFileSelectDialog());
                TatoebaSentenceLinksFileSelectCommand = new RelayCommand(() => TatoebaSentenceLinksFilePath.Value = FileHelper.ShowFileSelectDialog());


                KanjiVgFilePath = new ReactiveProperty<string>();
                KanjiDic2FilePath = new ReactiveProperty<string>();
                TatoebaSentencesFilePath = new ReactiveProperty<string>();
                TatoebaSentenceLinksFilePath = new ReactiveProperty<string>();
                JMdictFilePath = new ReactiveProperty<string>();

                DictionaryPopulationProgress = new ReactiveProperty<int>();
                KanjiInfoPopulationProgress = new ReactiveProperty<int>();
                TatoebaSentencesPopulationProgress = new ReactiveProperty<int>();
                TatoebaSentencesTokenizationProgress = new ReactiveProperty<int>();

                TatoebaSentencesCount = new ReactiveProperty<int>(TatoebaController.GetSentenceCount());
                TatoebaSentencesUntokenizedCount = new ReactiveProperty<int>(TatoebaController.GetUntokenizedCount());
            }
        }

        private bool IsProgressCompleted(int progress)
        {
            return progress == 0 || progress == 100;
        }

        protected void DefineCommands()
        {
            //    RenameFileCollectionNameCommand = new DelegateCommand(
            //        delegate
            //        {
            //            ShiroFileRepository.DosyayaKaydet(CollectionFilePath, CollectionName, WordsInFile);
            //            var keyboardFocus = Keyboard.FocusedElement as UIElement;
            //            if (keyboardFocus != null)
            //            {
            //                keyboardFocus.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            //            }
            //        });
            //    PopulateDbWithFileDataCommand = new DelegateCommand(
            //        delegate
            //        {
            //            DbPopulator.PopulateWordCollectionOnDb(CollectionFilePath, CollectionName);
            //            ReadCollectionFromDb(CollectionName);
            //        });

            //    //Task.Factory ile ayrı thread ile veriler alınınca binding hata veriyor AsyncObservableCollection kullanılacakmış
            //    //Hata:This type of CollectionView does not support changes to its SourceCollection from a thread different from the Dispatcher thread
            //    //http://www.thomaslevesque.com/2009/04/17/wpf-binding-to-an-asynchronous-collection/
            //    //FileSelectCommand = new DelegateCommand(o => Task.Factory.StartNew(() => CollectionFilePath = ShowFileSelectDialog()));
            //    FileSelectCommand = new DelegateCommand(
            //        delegate
            //        {
            //            CollectionFilePath = ShowFileSelectDialog();
            //            CollectionName = ShiroFileRepository.GetCollectionNameFromFile(CollectionFilePath);
            //            ReadCollectionFile(CollectionFilePath);
            //            ReadCollectionFromDb(CollectionName);
            //        });

            //    DeleteCollectionCommand = new DelegateCommand(
            //        delegate
            //        {
            //            ShiroRepository.Delete<Word>(a => a.CollectionName == CollectionName);
            //            ReadCollectionFromDb(CollectionName);
            //        });
        }

        //private void ReadCollectionFromDb(string selectedFile)
        //{
        //    WordsOnDb.Clear();
        //    WordsOnDb.AddRange(DbPopulator.ReadWordCollectionFromDb(Path.GetFileName(selectedFile)));
        //}

        //private void ReadCollectionFile(string selectedFile)
        //{
        //    WordsInFile.Clear();
        //    if (!string.IsNullOrEmpty(selectedFile))
        //        WordsInFile.AddRange(DbPopulator.ReadWordCollectionFromFile(selectedFile));
        //}

    }
}