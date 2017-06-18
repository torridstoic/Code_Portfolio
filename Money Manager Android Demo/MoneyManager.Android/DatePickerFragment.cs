using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Util;
using Android.Runtime;
using Android.Views;
using Android.Widget;

// NOTE: This is taken directly from Xamarin's website

namespace MoneyManager.Android
{
	class DatePickerFragment : DialogFragment, DatePickerDialog.IOnDateSetListener
	{
		// TAG can be any string that you desire.
		public static readonly string TAG = "X:" + typeof(DatePickerFragment).Name.ToUpper();
		Action<DateTime> _dateSelectedHandler = delegate { };
		DateTime current;

		public void OnDateSet(DatePicker view, int year, int monthOfYear, int dayOfMonth)
		{
			// Note: monthOfYear is a value between 0 and 11, not 1 and 12!
			DateTime selectedDate = new DateTime(year, monthOfYear + 1, dayOfMonth);
			Log.Debug(TAG, selectedDate.ToLongDateString());
			_dateSelectedHandler(selectedDate);
		}

		public static DatePickerFragment NewInstance(DateTime d, Action<DateTime> onDateSelected)
		{
			DatePickerFragment frag = new DatePickerFragment();
			frag._dateSelectedHandler = onDateSelected;
			frag.current = d;
			return frag;
		}

		public override Dialog OnCreateDialog(Bundle savedInstanceState)
		{
			DatePickerDialog dialog = new DatePickerDialog(Activity, this, current.Year, current.Month - 1, current.Day);
			return dialog;
		}
	}
}