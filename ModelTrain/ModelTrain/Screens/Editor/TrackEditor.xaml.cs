using ModelTrain.Model;
using ModelTrain.Model.Track;
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
	private readonly IBusinessLogic businessLogic;

	private readonly PersonalProject loadedProject;
	private readonly ActionHandler actionHandler;

	// TODO: generalize project to allow both PersonalProject's and SharedProject's
	public TrackEditor(PersonalProject? project = null)
	{
		InitializeComponent();

		Back.Text = IconFont.Arrow_back + " BACK";
		EditPieces.Text = IconFont.Settings;

		Save.Text = IconFont.Save + " SETTINGS";
		ChangeBackground.Text = IconFont.Image + " CHANGE BACKGROUND";

		businessLogic = new BusinessLogic();
		project ??= new();

		loadedProject = project;
		actionHandler = new(project.Track);
	}

	private async void OnPieceEditButtonClicked(object sender, EventArgs e)
	{
		// Opens Piece Catalog
		await Navigation.PushAsync(new PieceCatalog());
	}

	private async void OnBackButtonClicked(object sender, EventArgs e)
	{
		// Will return to either My Tracks or Shared Tracks depending on how the user got here
		await Navigation.PopAsync();
	}

	private async void OnBackgroundButtonClicked(object sender, EventArgs e)
	{
		// Opens Change Background
		await Navigation.PushAsync(new ChangeBackground());
	}

	private async void OnSaveButtonClicked(object sender, EventArgs e)
	{
		// TODO: descriptive error messages
		if (businessLogic.SaveProject(loadedProject))
			await DisplayAlert("Success", "Track saved successfully!", "OK");
		else
			await DisplayAlert("Failure", "Track failed to save!", "OK");
	}

	private void OnUndoButtonClicked(object sender, EventArgs e)
	{
		actionHandler.Undo();
	}

	private void OnRedoButtonClicked(object sender, EventArgs e)
	{
		actionHandler.Redo();
	}
}