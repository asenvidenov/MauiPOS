namespace MauiPOS
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
        }

        private void OnCounterClicked(object sender, EventArgs e)
        {
            var getOpID = new Services.ServerService().RetOpID("1");
            if(getOpID.OpID != 0) { CounterBtn.Text = getOpID.OpFName; }
            SemanticScreenReader.Announce(CounterBtn.Text);
        }
    }

}
