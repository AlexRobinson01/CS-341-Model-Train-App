namespace ModelTrain.Screens
{
	/*
     * This class is a base page so that all screens that use it can 
	 * use the toolbar and thus the account button should they want it
     * Author:  Taylor Showalter
     * Date: October 29, 2024
     */
	public class BasePage : ContentPage
	{
		/// <summary>
		/// Initializes the BasePage.
		/// Adds a toolbar item for navigating to the Account page.
		/// </summary>
		public BasePage()
		{
			// Add a toolbar item to navigate to the Account page
			ToolbarItems.Add(new ToolbarItem
			{
				IconImageSource = "account.png", // Icon for the toolbar item
				Order = ToolbarItemOrder.Primary,
				Priority = 0, // Highest priority for display order
				Command = new Command(async () =>
					// Command to navigate to the Account page
					await Navigation.PushAsync(new Account()))
			});
		}
	}
}