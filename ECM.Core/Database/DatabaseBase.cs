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

        static bool writingToDB = false;
        public virtual void SaveToDatabase()
        {
            if (IsDirty && !writingToDB)
            {
                writingToDB = true;
                WriteToDatabase();
                writingToDB = false;

                IsDirty = false;
            }
        }

        public DatabaseBase ()
        {
        }
    }
}

