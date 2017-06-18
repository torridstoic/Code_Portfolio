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
	public class FiltersAmountsFragment : DialogFragment
	{
		bool filterAmounts;
		float minAmount;
		float maxAmount;
		Action<Boolean, float, float> handler = delegate { };

		EditText minInput;
		EditText maxInput;

		public static FiltersAmountsFragment NewInstance(bool filter, float min, float max, Action<Boolean, float, float> func)
		{
			FiltersAmountsFragment frag = new FiltersAmountsFragment();

			#region Load Parameters
			// Load Parameters
			frag.filterAmounts = filter;
			frag.minAmount = min;
			frag.maxAmount = max;
			frag.handler = func;
			#endregion

			return frag;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			#region Load the Custom View
			// Load the Custom View and Preset Data
			View currView = inflater.Inflate(Resource.Layout.FiltersAmounts, container, false);
			minInput = currView.FindViewById<EditText>(Resource.Id.filter_amounts_min);
			//minInput.Text = minAmount.ToString();
			maxInput = currView.FindViewById<EditText>(Resource.Id.filter_amounts_max);
			//maxInput.Text = maxAmount.ToString();
			#endregion

			#region Button Functions
			// Button Functions
			currView.FindViewById<Button>(Resource.Id.filter_amounts_clear_button).Click += delegate {
				filterAmounts = false;
				handler(filterAmounts, 0.00f, float.MaxValue);
				Dismiss();
				Toast.MakeText(Activity, "Amounts Filter Reset", ToastLength.Short).Show();
			};

			currView.FindViewById<Button>(Resource.Id.filter_amounts_cancel).Click += delegate {
				Dismiss();
				Toast.MakeText(Activity, "Changes Cancelled", ToastLength.Short).Show();
			};
			currView.FindViewById<Button>(Resource.Id.filter_amounts_ok).Click += delegate {
				if (minInput.Text == String.Empty || maxInput.Text == String.Empty)
				{
					Toast.MakeText(Activity, "Fill all fields", ToastLength.Short).Show();
					return;
				}

				filterAmounts = true;
				handler(filterAmounts, (float)Convert.ToDouble(minInput.Text), (float)Convert.ToDouble(maxInput.Text));
				Dismiss();
				Toast.MakeText(Activity, "Amounts Filter Added", ToastLength.Short).Show();
			};
			#endregion

			return currView;
		}
	}
}