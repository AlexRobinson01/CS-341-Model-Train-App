namespace ModelTrain.Screens;

/**
 * This page should be viewed in Landscape for the best experience.
 * 
 * Description: Allows the user to change which pieces they see in the list to the right
 * of the track editor page, in case they don't like their current list's order.
 * Author: Alex Robinson
 * Date: 10/16/2024
 */
public partial class PieceCatalog : ContentPage
{
	public PieceCatalog()
	{
		InitializeComponent();

		Edit.Text = IconFont.Settings + " EDIT PIECE";
		Back.Text = IconFont.Arrow_back + " BACK";
	}
}