using MauiPOS.Data;
using MauiPOS.Models;
using MauiPOS.Services;
using System.Collections.ObjectModel;

namespace MauiPOS.Views;

public partial class MAUIOrder : ContentPage
{
    public ObservableCollection<POSOrderDetailsView> Result { get; set; }
    public MAUIOrder()
	{
        InitializeComponent();
        Result = new(RetServerServices.RetSrvOrderDetails(POSGlobals.CurrentOrderID));
        if(Result.Count>0) { 
            oDetails.ItemsSource = Result;
            oDetails.IsVisible = true;

            }
        ActionCommand = new MyCommand();
        ActionCommand.CanExecuteFunc = obj => true;
        ActionCommand.ExecuteFunc = MyActionFunc;
        //SetGoodsGroupView(0);
        GoodsCommand(0);
        DebugInit();
        BindingContext = this;
    }
    private void MenuCommand(object sender, EventArgs args)
    {
        GoodsCommand(0);
    }

    private void AddCommand(object sender, EventArgs args)
    {
        var swipeview = sender as SwipeItem;    
        var item = (POSOrderDetailsView)swipeview.CommandParameter;
        POSOrderDetailsView? res = Result.ToList().Find(res => res.ID == item.ID);
        if(res != null)
        {
            res.Cnt+= 1;
        }
        //oDetails.ItemsSource = null;
        //oDetails.ItemsSource = result;
    }
    private void DeleteCommand(object sender, EventArgs args)
    {
        var swipeview = sender as SwipeItem;
        var item = (POSOrderDetailsView)swipeview.CommandParameter;
        POSOrderDetailsView? res = Result.ToList().Find(res => res.ID == item.ID);
        if (res != null)
        {
            res.Cnt -= 1;
            if(res.Cnt == 0) {Result.Remove(res); }
        }
    }

    private void OnDetailTapped(object sender, EventArgs args)
    {

    }

    private void AddGoodToList(int goodsID, int cnt = 1)
    {
        (decimal, decimal) gPrice = new POSRetData().RetGoodPrice(goodsID);
        var _newGood = new POSOrderDetailsView()
        {
            ID=cnt,
            OpID=POSGlobals.localOpID,
            OrderID = POSGlobals.CurrentOrderID,
            GoodsID = goodsID,
            CashName = new POSRetData().RetCashName(goodsID),
            CashPrice=gPrice.Item1,
            Cnt = cnt,
            Annul = 0
        };
        Result.Add(_newGood);
        //oDetails.ItemsSource = null;
        //oDetails.ItemsSource = Result;
        oDetails.IsVisible = true;

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
                    Command = ActionCommand,
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
                    Command = ActionCommand,
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
                    Command = ActionCommand,
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
    public MyCommand ActionCommand
    {
        get;
        set;
    }

    public void MyActionFunc(object parameter)
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


    
}

