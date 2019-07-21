using System;
using System.Collections.Generic;
using System.ComponentModel;



namespace Interfaces
{
	public interface ISession : INotifyPropertyChanged // "to support OneWay/TwoWay bindings, the underlying data must implement INotifyPropertyChanged"
	{
        string GetSessionWorkingDirectory();
    }
}
