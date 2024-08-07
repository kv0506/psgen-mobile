﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PsGen.Common;

public class ViewModelBase : INotifyPropertyChanged
{
	bool _isBusy = false;
	public bool IsBusy
	{
		get => _isBusy;
		set => SetProperty(ref _isBusy, value);
	}

	protected bool SetProperty<T>(ref T backingStore, T value,
		[CallerMemberName] string propertyName = "",
		Action onChanged = null)
	{
		if (EqualityComparer<T>.Default.Equals(backingStore, value))
			return false;

		backingStore = value;
		onChanged?.Invoke();
		OnPropertyChanged(propertyName);
		return true;
	}

	#region INotifyPropertyChanged

	public event PropertyChangedEventHandler PropertyChanged;

	protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
	{
		var changed = PropertyChanged;

		changed?.Invoke(this, new PropertyChangedEventArgs(propertyName));
	}

	#endregion
}