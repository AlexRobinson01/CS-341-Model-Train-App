namespace ModelTrain.Screens;

public class BasePage : ContentPage
{
	public BasePage()
	{
		ToolbarItems.Add(new ToolbarItem
		{
			IconImageSource = "account.png",
			Order = ToolbarItemOrder.Primary,
			Priority = 0,
			Command = new Command(async () => await Navigation.PushAsync(new Account()))
		});
	}
}
