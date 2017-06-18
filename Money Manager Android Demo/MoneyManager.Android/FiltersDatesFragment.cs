using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace MoneyManager.Android
{
	public class FiltersDatesFragment : DialogFragment
	{
		bool filterDates;
		DateTime startDate;
		DateTime endDate;
		Action<Boolean, DateTime, DateTime> handler = delegate { };

		Button sdateButton;
		Button edateButton;

		public static FiltersDatesFragment NewInstance(bool filter, DateTime start, DateTime end, Action<Boolean, DateTime, DateTime> onThingChange) //Action<Boolean> onFilterSet, Action<DateTime> onStartSet, Action<DateTime> onEndSet)
		{
			FiltersDatesFragment frag = new FiltersDatesFragment();

			#region Load Parameters
			// Load Parameters
			frag.filterDates = filter;
			frag.startDate = start;
			frag.endDate = end;
			frag.handler = onThingChange;
			#endregion

			return frag;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			#region Load the Custom View
			// Load the Custom View and Preset Data
			View currView = inflater.Inflate(Resource.Layout.FiltersDates, container, false);
			sdateButton = currView.FindViewById<Button>(Resource.Id.filter_dates_start_button);
			sdateButton.Text = startDate.ToString("d");
			edateButton = currView.FindViewById<Button>(Resource.Id.filter_dates_end_button);
			edateButton.Text = endDate.ToString("d");
			#endregion

			#region Button Functions
			// Button Functions
			currView.FindViewById<Button>(Resource.Id.filter_dates_clear_button).Click += delegate {
				filterDates = false;
				handler(filterDates, new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1), DateTime.Today);
				Dismiss();
				Toast.MakeText(Activity, "Dates Filter Reset", ToastLength.Short).Show();
			};
			sdateButton.Click += delegate {
				DatePickerFragment dpf = DatePickerFragment.NewInstance(startDate, delegate (DateTime date) {
					sdateButton.Text = date.ToString("d");
					startDate = date;
					//handler(filterDates, date, endDate);
				});
				dpf.Show(FragmentManager, DatePickerFragment.TAG);
			};
			edateButton.Click += delegate {
				DatePickerFragment dpf = DatePickerFragment.NewInstance(endDate, delegate (DateTime date) {
					edateButton.Text = date.ToString("d");
					endDate = date;
					//handler(filterDates, startDate, date);
				});
				dpf.Show(FragmentManager, DatePickerFragment.TAG);
			};

			currView.FindViewById<Button>(Resource.Id.filter_dates_cancel).Click += delegate {
				Dismiss();
				Toast.MakeText(Activity, "Changes Cancelled", ToastLength.Short).Show();
			};
			currView.FindViewById<Button>(Resource.Id.filter_dates_ok).Click += delegate {
				filterDates = true;
				handler(filterDates, startDate, endDate);
				Dismiss();
				Toast.MakeText(Activity, "Dates Filter Added", ToastLength.Short).Show();
			};
			#endregion

			return currView;
		}
	}
}