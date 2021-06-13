using System.ComponentModel;

namespace WowCursor.MVVM
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected void Set<T>(string propertyName, ref T oldValue, T newValue)
        {
            if ((oldValue == null && newValue != null) || (oldValue != null && !oldValue.Equals(newValue)))
            {
                oldValue = newValue;
                OnPropertyChanged(propertyName);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
