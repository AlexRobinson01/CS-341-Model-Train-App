namespace ModelTrain.Screens;
/*
 * This class is the background functionality/methods the New Track page
 * Author: Taylor Showalter
 * Date: October 16, 2024
 */
public partial class NewTrack : BasePage
{
    public NewTrack()
    {
        InitializeComponent();
    }
    private async void OnHomeButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}