using MauiPOS.Data;
using MauiPOS.Models;
using System.Windows.Input;

namespace MauiPOS.Views;
public partial class CashLogin : ContentPage
{
	private POSdata _localDB;
	public CashLogin()
	{
		InitializeComponent();
		_localDB = new POSdata();
	}

	private void LogIn(object sender, EventArgs args)
    {
        var getOpID = new Services.ServerService().RetOpID(pass.Text);
		if (getOpID != null)
		{
            Title = getOpID.OpFName;
            _localDB.SavePOSops(getOpID).Wait();
            pass.Text = "";
			if (getOpID.OpRole > 50)
			{
				btnAdmin.IsVisible = true;
				pass.IsVisible = false;
				btnLogin.IsVisible = false;
				btnContinue.IsVisible = true;
				btnLogout.IsVisible = true;

				_localDB.SaveMaxOrderID().Wait();
				if(POSGlobals.MaxOrderID < 0)
				{
					SyncButton.IsVisible = true;
					btnAdmin.IsVisible = false;
					btnContinue.IsVisible= false;
					return;
				}

				var getOrders = new Services.ServerService().RetActiveOrders(getOpID.OpID);
				if(getOrders != null)
				{
					opOrders.ItemsSource = getOrders;
					opOrders.IsVisible = true;
				}
			}
		}
    }

	private void SyncButton_Clicked(object sender, EventArgs args)
	{
		return;
	}


    private void LogOut(object sender, EventArgs args)
	{
		_localDB.LogOut();
		pass.IsVisible = true;
		btnLogin.IsVisible = true;
		btnContinue.IsVisible = false;
		btnLogout.IsVisible = false;
		btnAdmin.IsVisible	= false;
		Title = "ÂÏÈÑÂÀÍÅ";
		opOrders.ItemsSource = "";
		opOrders.IsVisible = false;
	}

	private  void  OnOrderTapped(object sender, ItemTappedEventArgs args)
	{
		try
		{
			var order = (args.Item as ViewActiveOrders).OrderID;
			if ((int)order > 0) return;
		}
		catch { return; }
		
	}
}
