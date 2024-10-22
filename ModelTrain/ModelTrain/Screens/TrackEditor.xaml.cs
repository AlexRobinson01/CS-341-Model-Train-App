using ModelTrain.Model;
namespace ModelTrain.Screens;

/**
 * This page should be viewed in Landscape for the best experience.
 * 
 * Description: The main track editor page, allows someone to modify the track they have opened.
 * Author: Alex Robinson
 * Date: 10/16/2024
 */
public partial class TrackEditor : ContentPage
{
	public TrackEditor()
	{
		InitializeComponent();

		Back.Text = IconFont.Arrow_back + " BACK";
		EditPieces.Text = IconFont.Settings;

		Save.Text = IconFont.Save + " SETTINGS";
		ChangeBackground.Text = IconFont.Image + " CHANGE BACKGROUND";
	}
}