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

using MoneyManager.Data;

namespace MoneyManager.Android
{
	public class EditWalletFragment : DialogFragment
	{
		Wallet currWallet;
		Action Refresh = delegate { };

		EditText wName;

		public static EditWalletFragment NewInstance(int wId, Action response)
		{
			#region Load Parameters
			EditWalletFragment frag = new EditWalletFragment();
			if (wId < 0)
				frag.currWallet = new Wallet(Global.user, Global.GetUniqueId());
			else
				foreach (Wallet w in Global.gWallets)
					if (w.Id == wId)
					{
						frag.currWallet = w;
						break;
					}
			frag.Refresh = response;
			#endregion
			
			return frag;
		}

		public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
		{
			#region Load the Custom View
			// Load the Custom View
			View currView = inflater.Inflate(Resource.Layout.EditWallet, container, false);
			wName = currView.FindViewById<EditText>(Resource.Id.edit_wallet_name);
			// Get the Wallet to edit, or create a new one
			if (currWallet.Name == null)
				currView.FindViewById<TextView>(Resource.Id.edit_wallet_title).Text = "New Wallet";
			else
			{
				currView.FindViewById<TextView>(Resource.Id.edit_wallet_title).Text = "Edit Wallet";
				currView.FindViewById<TextView>(Resource.Id.edit_wallet_name).Text = currWallet.Name;
			}
			#endregion

			#region Button Functions
			// Button Functions
			currView.FindViewById<Button>(Resource.Id.edit_wallet_cancel).Click += delegate {
				Dismiss();
				Toast.MakeText(Activity, "Cancelled", ToastLength.Short).Show();
			};
			currView.FindViewById<Button>(Resource.Id.edit_wallet_ok).Click += delegate {
				// Validation Check
				if (String.Empty == wName.Text)
				{
					Toast.MakeText(Activity, "Enter a wallet name.", ToastLength.Short).Show();
					return;
				}

				// Update DB, and Exit
				currWallet.Name = wName.Text;
				for(int i=0; i<Global.gWallets.Count; ++i)
					if (Global.gWallets[i].Id == currWallet.Id)
					{
						Global.gWallets[i] = currWallet;
						Dismiss();
						Toast.MakeText(Activity, "Success!", ToastLength.Short).Show();
						return;
					}
				// else
				Global.gWallets.Add(currWallet);
				Refresh();
				Dismiss();
				Toast.MakeText(Activity, "Success!", ToastLength.Short).Show();
			};
			#endregion

			return currView;
		}
	}
}