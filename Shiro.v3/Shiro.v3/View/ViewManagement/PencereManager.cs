using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;

namespace Shiro.View.ViewManagement
{
    public class PencereManager : INotifyPropertyChanged
    {
        private Pencere _etkinPencere;

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged([CallerMemberName] string caller = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null) handler(this, new PropertyChangedEventArgs(caller));
        }

        #endregion


        public PencereManager()
        {
            PencereTypes = new List<Type>(8);
            Pencereler = new List<Pencere>();
        }

        private List<Type> PencereTypes { get; set; }
        public ContentControl Container { get; set; }
        public List<Pencere> Pencereler { get; set; }

        public Pencere EtkinPencere
        {
            get { return _etkinPencere; }
            private set
            {
                _etkinPencere = value;
                RaisePropertyChanged();
            }
        }

        public void AddNewPencereType(Type type)
        {
            if (!PencereTypes.Contains(type))
                PencereTypes.Add(type);
        }


        public void ChangeToWindow(Type targetWindowType)
        {
            if (EtkinPencere != null && EtkinPencere.GetType() == targetWindowType)
                return;
            if (PencereTypes.Contains(targetWindowType))
            {
                if (Pencereler.All(a => a.GetType() != targetWindowType))
                    Pencereler.Add((Pencere)Activator.CreateInstance(targetWindowType));

                SetActiveWindow(GetPencereByType(targetWindowType));
            }
        }

        private Pencere GetPencereByType(Type targetWindowType)
        {
            return Pencereler.FirstOrDefault(a => a.GetType() == targetWindowType);
        }


        public void HideAllImmediately()
        {
            foreach (Pencere window in Pencereler)
            {
                window.Visibility = Visibility.Hidden;
                window.Opacity = 0;
            }
        }

        private void Show(Pencere pencere)
        {
            pencere.Show();
            //pencere.Visibility = Visibility.Visible;
            //pencere.Opacity = 100;
        }

        private void SetActiveWindow(Pencere targetWindow)
        {
            // ReSharper disable once PossibleUnintendedReferenceComparison
            if (EtkinPencere == targetWindow)
                return;

            if (EtkinPencere != null)
                EtkinPencere.Hide();

            EtkinPencere = targetWindow;

            if (EtkinPencere != null)
            {
                //HideAllImmediately();
                Show(EtkinPencere);
            }
        }
    }
}