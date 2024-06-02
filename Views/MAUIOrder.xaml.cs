using MauiPOS.Data;
using MauiPOS.Models;
using MauiPOS.Services;
using System.Collections.ObjectModel;

namespace MauiPOS.Views;

public partial class MAUIOrder : ContentPage
{
    private int _maxID;

    public List<POSOrderDetailsView> Result { get; set; }
    public MAUIOrder()
	{
        InitializeComponent();
        Result = new(RetServerServices.RetSrvOrderDetails(POSGlobals.CurrentOrderID));
        if(Result.Count>0) { 
            oDetails.ItemsSource = Result;
            oDetails.IsVisible = true;

            }
        BtnGoodsCommand = new MyCommand();
        BtnGoodsCommand.CanExecuteFunc = obj => true;
        BtnGoodsCommand.ExecuteFunc = BtnGoodsFunc;

        BtnConfirmCommand = new MyCommand();
        BtnConfirmCommand.CanExecuteFunc = obj => true;
        BtnConfirmCommand.ExecuteFunc = BtnConfirmFunc;

        BtnPaymentCommand = new MyCommand();
        BtnPaymentCommand.CanExecuteFunc = obj => true;
        BtnPaymentCommand.ExecuteFunc = BtnPaymentFunc;

        BtnChronoCommand = new MyCommand();
        BtnChronoCommand.CanExecuteFunc = obj => true;
        BtnChronoCommand.ExecuteFunc = BtnChronoFunc;
        //SetGoodsGroupView(0);
        //GoodsCommand(0);
        MenuCommand();
        DebugInit();
        BindingContext = this;
    }

    public MyCommand BtnGoodsCommand
    {
        get;
        set;
    }
    public MyCommand BtnConfirmCommand
    {
        get;
        set;
    }

    public MyCommand BtnPaymentCommand
    {
        get;
        set;
    }

    public MyCommand BtnChronoCommand
    {
        get;
        set;
    }
    private void MenuCommand(object sender, EventArgs args)
    {
        GoodsCommand(0);
    }

    private void AddToLocalDB(ref POSOrderDetailsView res)
    {
        POSOrderDetails _insGood = new()
        {
            ID = res.ID,
            CashPrice = res.CashPrice,
            Annul = res.Annul,
            Cnt = res.Cnt,
            GoodsID = res.GoodsID,
            OpID = res.OpID,
            OrderID = res.OrderID,
            Modiff = res.Modiff

        };
        POSdata.SaveOrderDetail(ref _insGood);
    }
    private void AddCommand(object sender, EventArgs args)
    {
        var swipeview = sender as SwipeItem;    
        var item = (POSOrderDetailsView)swipeview.CommandParameter;
        POSOrderDetailsView? res = Result.Find(res => res.ID == item.ID);
        if(res != null)
        {
            res.Cnt+= 1;
            AddToLocalDB(ref res);
        }
        //oDetails.ItemsSource = null;
        //oDetails.ItemsSource = result;
    }
    private void DeleteCommand(object sender, EventArgs args)
    {
        var swipeview = sender as SwipeItem;
        var item = (POSOrderDetailsView)swipeview.CommandParameter;
        POSOrderDetailsView? res = Result.Find(res => res.ID == item.ID);
        if (res != null)
        {
            res.Cnt -= 1;
            res.Annul += 1;
            res.ItemColor = Color.Parse("LightSalmon");
            if(res.Cnt == 0)
            {
                res.ItemColor = Color.Parse("Salmon");
                Result.Remove(res); 
            }
            AddToLocalDB(ref res);
        }
    }

    private void OnDetailTapped(object sender, EventArgs args)
    {

    }

    private void AddGoodToList(int goodsID, int cnt = 1, string? modiff=null)
    {
        (decimal, decimal) gPrice = new POSRetData().RetGoodPrice(goodsID);
        var _color = cnt%2==0 ? Color.Parse("AntiqueWhite") : Color.Parse("GhostWhite");
        var _newGood = new POSOrderDetailsView()
        {
            ID = cnt,
            OpID = POSGlobals.localOpID,
            OrderID = POSGlobals.CurrentOrderID,
            GoodsID = goodsID,
            CashName = new POSRetData().RetCashName(goodsID),
            CashPrice = gPrice.Item1,
            Cnt = cnt,
            Annul = 0,
            Modiff = modiff,
            ItemColor = _color
        };
        Result.Add(_newGood);
        AddToLocalDB(ref _newGood);
        oDetails.IsVisible = true;
    }

    private void MenuCommand()
    {
        Button btnGoods = new()
        {
            ImageSource = "plate_utensils.png",
            Command = BtnGoodsCommand,
            CommandParameter =0
        };
        GoodsFlex.Add(btnGoods);

        Button btnConfirm = new()
        {
            ImageSource = "order_chrono.png",
            Command = BtnChronoCommand,
            CommandParameter = POSGlobals.CurrentOrderID
        };
        GoodsFlex.Add(btnConfirm);

        Button btnPayment = new()
        {
            ImageSource = "money_check.png",
            Command = BtnPaymentCommand,
            CommandParameter = POSGlobals.CurrentOrderID
        };
        GoodsFlex.Add(btnPayment);

        Button btnChrono = new()
        {
            ImageSource = "order_chrono.png",
            Command = BtnChronoCommand,
            CommandParameter = POSGlobals.CurrentOrderID
        };
        GoodsFlex.Add(btnChrono);
    }
    private void GoodsCommand(int GoodsID)
    {
        int _GParent= new POSRetData().RetGoodParent(GoodsID);
        Button btnGood;
        var result = new POSRetData().RetGoodsByGroup(GoodsID);
        try
        {
            foreach (var item in result)
            {
                btnGood = new()
                {
                    Text = item.CashName,
                    BackgroundColor = item.IsGroup ? Color.Parse("Blue") : Color.Parse("Green"),
                    Command = BtnGoodsCommand,
                    CommandParameter = item.GoodsID
                };
                btnGood.HorizontalOptions = LayoutOptions.FillAndExpand;
                btnGood.VerticalOptions = LayoutOptions.CenterAndExpand;
                btnGood.Margin = 5;
                GoodsFlex.Add(btnGood);
            }
            if (GoodsID > 0)
            {
                btnGood = new()
                {
                    Text = "..",
                    BackgroundColor = Color.Parse("Blue"),
                    Command = BtnGoodsCommand,
                    CommandParameter = _GParent
                };
                btnGood.WidthRequest = 150;
                btnGood.HeightRequest = 50;
                btnGood.Margin = 10;
                GoodsFlex.Add(btnGood);
            }
            //btnGood = new()
            //{
            //    Text = "X",
            //    BackgroundColor = Color.Parse("Red"),
            //    Command = ActionCommand,
            //    CommandParameter = -1
            //};
            //btnGood.WidthRequest = 150;
            //btnGood.HeightRequest = 50;
            //btnGood.Margin = 10;
            //GoodsFlex.Add(btnGood);
            //GoodsFlex.IsVisible= true;
        }
        catch { }
        }

    private void OnDetailSelected(object sender, SelectedItemChangedEventArgs args)
    {

    }
    //Sometime, when SwipeItemView will work...
    private void SetGoodsGroup(int GoodsID)
    {
        SwipeItem swipeItem = null;
        SwipeItems swipeItems = new();
        var result = new POSRetData().RetGoodsByGroup(GoodsID);
        try
        {
            foreach(var item in result)
            {
                swipeItem = new()
                {
                    Text = item.CashName,
                    BackgroundColor = item.IsGroup ? Color.Parse("Blue") : Color.Parse("Green"),
                    CommandParameter = item.GoodsID,
                    Command = BtnGoodsCommand,
                };
                swipeItems.Add(swipeItem);
                swipeItems.SwipeBehaviorOnInvoked = SwipeBehaviorOnInvoked.RemainOpen;
                swipeItems.Mode = SwipeMode.Reveal;
            }
            if(swipeItems.Count > 0)
            {
                //GoodsView.TopItems= swipeItems;
            }
        }
        catch { }
    }

    public void BtnGoodsFunc(object parameter)
    {
        int goodID = (int)parameter;
        //SetGoodsGroup((int)parameter);
        GoodsFlex.Children.Clear();
        if(goodID <0)
        {
            GoodsFlex.IsVisible = false;
            return;
        }
        if (! new POSRetData().RetGoodIsGroup(goodID))
        {
            GoodsFlex.Children.Clear();
            //GoodsFlex.IsVisible = false;
            AddGoodToList(goodID);
            GoodsCommand(0);
            return;
        }
        GoodsCommand(goodID);
    }

    private void DebugInit()
    {
        for (int n = 0; n < 22; n++)
        {
            try
            {
                AddGoodToList(n,n);
            }
            catch { }
        }
    }

    private void BtnConfirmFunc(object parameter) { }

    private void BtnPaymentFunc(object parameter) { }

    private void BtnChronoFunc(object parameter) { }
    
}

