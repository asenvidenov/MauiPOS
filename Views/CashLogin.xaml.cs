using MauiPOS.Data;
using MauiPOS.Models;
using MauiPOS.Services;
using System.Windows.Input;

namespace MauiPOS.Views;
public partial class CashLogin : ContentPage
{
	public CashLogin()
	{
		InitializeComponent();
	}

	private void LogIn(object sender, EventArgs args)
    {
        var getOpID = new ServerService().RetOpID(pass.Text);
		if (getOpID != null)
		{
            Title = getOpID.OpFName;
            POSdata.SavePOSops(getOpID);
            pass.Text = "";
			if (getOpID.OpRole > 50)
			{
                btnAdmin.IsVisible = true;
            }
            pass.IsVisible = false;
            btnLogin.IsVisible = false;
            btnContinue.IsVisible = true;
            btnLogout.IsVisible = true;

            ActiveShift ashift = new();
            RetServerServices.RetActiveShift(getOpID.OpID, ref ashift);
            lblShiftStart.Text = ashift.ShiftStart.ToString();

            POSdata.SaveMaxOrderID();
            if (POSGlobals.MaxOrderID < POSGlobals.LocalOrderID)
            {
				NotSynced();
                return;
            }

            var getGoods = new ServerService().SyncGoods();
			if (!getGoods)
			{
				NotSynced();
				return;
			}

            var getObjects = new ServerService().SyncObjects();
			if (!getObjects)
			{ NotSynced(); return; }

            var getCashPrice = new ServerService().SyncCashPrice();
			if (!getCashPrice) { NotSynced(); return; }


            var getOrders = new ServerService().RetActiveOrders(getOpID.OpID);
            if (getOrders != null)
            {
                opOrders.ItemsSource = getOrders;
                opOrders.IsVisible = true;
            }

        }
    }

	private void SyncButton_Clicked(object sender, EventArgs args)
	{
        var getGoods = new ServerService().SyncGoods();
        if (!getGoods)
        {
            NotSynced();
            return;
        }

        var getObjects = new ServerService().SyncObjects();
        if (!getObjects)
        { NotSynced(); return; }

        var getCashPrice = new ServerService().SyncCashPrice();
        if (!getCashPrice) { NotSynced(); return; }
    }


    private void LogOut(object sender, EventArgs args)
	{
		POSdata.LogOut();
		pass.IsVisible = true;
		btnLogin.IsVisible = true;
		btnContinue.IsVisible = false;
		btnLogout.IsVisible = false;
		btnAdmin.IsVisible	= false;
		Title = "ÂÏÈÑÂÀÍÅ";
		opOrders.ItemsSource = "";
		opOrders.IsVisible = false;
        lblShiftStart.Text = "";
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

	private void NotSynced()
	{
        SyncButton.IsVisible = true;
        btnAdmin.IsVisible = false;
        btnContinue.IsVisible = false;
		return;
    }
}
