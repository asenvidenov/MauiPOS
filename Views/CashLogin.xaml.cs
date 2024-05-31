using MauiPOS.Data;
using MauiPOS.Models;
using MauiPOS.Services;

namespace MauiPOS.Views;
public partial class CashLogin : ContentPage
{
    public CashLogin()
    {
        InitializeComponent();
    }

    private void LogIn(object sender, EventArgs args)
    {

        ServerService.InitialSync();

        var getOpID = RetServerServices.RetOpID(pass.Text);
        if (getOpID != null)
        {
            POSGlobals.localOpID=getOpID.OpID;
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
            lblShiftStart.Text = "ÑÌßÍÀ ÎÒ: " + String.Format(ashift.ShiftStart.ToString(), "dd-MM-yyyy hh:mm");

            POSdata.CheckMaxOrderID();
            if (POSGlobals.MaxOrderID != POSGlobals.LocalOrderID)
            {
                NotSynced();
                return;
            }

            var getGoods = ServerService.SyncGoods();
            if (!getGoods)
            {
                NotSynced();
                return;
            }

            var getObjects = ServerService.SyncObjects();
            if (!getObjects)
            { NotSynced(); return; }

            var getCashPrice = ServerService.SyncCashPrice();
            if (!getCashPrice) { NotSynced(); return; }


            var getOrders = new POSRetData().RetActiveOrders(getOpID.OpID);
            if (getOrders != null)
            {
                opOrders.ItemsSource = getOrders;
                opOrders.IsVisible = true;
            }

        }
    }

    private void SyncButton_Clicked(object sender, EventArgs args)
    {
        var getGoods = ServerService.SyncGoods();
        if (!getGoods)
        {
            NotSynced();
            return;
        }

        var getObjects = ServerService.SyncObjects();
        if (!getObjects)
        { NotSynced(); return; }

        var getCashPrice = ServerService.SyncCashPrice();
        if (!getCashPrice) { NotSynced(); return; }
    }


    private void LogOut(object sender, EventArgs args)
    {
        POSGlobals.localOpID = 0;
        POSdata.LogOut();
        pass.IsVisible = true;
        btnLogin.IsVisible = true;
        btnContinue.IsVisible = false;
        btnLogout.IsVisible = false;
        btnAdmin.IsVisible = false;
        Title = "ÂÏÈÑÂÀÍÅ";
        opOrders.ItemsSource = "";
        opOrders.IsVisible = false;
        lblShiftStart.Text = "";
        SyncButton.IsVisible = false;
    }

    private void OnOrderTapped(object sender, ItemTappedEventArgs args)
    {

            var order = (args.Item as ViewActiveOrders).OrderID;
            POSGlobals.CurrentOrderID = (int)order;
            if (POSGlobals.CurrentOrderID > 0) Shell.Current.GoToAsync($"mauiorder");

    }

    private void NotSynced()
    {
        SyncButton.IsVisible = true;
        btnAdmin.IsVisible = false;
        btnContinue.IsVisible = false;
        if(RetServerServices.RetSrvOrders(POSGlobals.localOpID, true, 0)>0)  
        return;
    }

    private void DeleteCommand(object sender, EventArgs args)
    {
        var item = args.ToString();
    }

}
