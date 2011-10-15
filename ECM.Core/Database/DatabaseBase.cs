using System;

namespace ECM
{
    public abstract class DatabaseBase
    {
        public bool IsDirty { get; protected set; }

        protected bool SetProperty<T>(string propertyName, ref T currentValue, T newValue)
        {
            bool propertyChanged = false;

            if (currentValue == null || !currentValue.Equals(newValue))
            {
                currentValue = newValue;
                //OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
                IsDirty = true;
                propertyChanged = true;
            }
    
            return propertyChanged;
        }

        protected abstract void WriteToDatabase();

        public virtual void SaveToDatabase()
        {
            if(IsDirty)
                WriteToDatabase();

            IsDirty = false;
        }

        public DatabaseBase ()
        {
        }
    }
}

