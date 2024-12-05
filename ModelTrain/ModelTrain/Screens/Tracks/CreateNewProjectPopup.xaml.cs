using ModelTrain.Model;
namespace ModelTrain.Screens;

public partial class CreateNewProjectPopup : ContentPage
{
	public CreateNewProjectPopup()
	{
		InitializeComponent();
	}

    public event Action<string, string, string> ProjectCreated;

    private async void OnSubmitProjectClicked(object sender, EventArgs e)
    {
        string projectName = ProjectNameEntry.Text;

        if (!string.IsNullOrWhiteSpace(projectName))
        {
            string projectId = await BusinessLogic.Instance.GetUniqueGuid(); 
            string date = DateTime.Now.ToString("MM/dd/yyyy");

            // Unfocus the entry to close the keyboard
            ProjectNameEntry.Unfocus();

            // Notify the main page about the new project
            ProjectCreated?.Invoke(projectName, projectId, date);

            // Close the modal
            await Navigation.PopModalAsync();
        }
        else
        {
            await DisplayAlert("Error", "Project name cannot be empty.", "OK");
        }
    }

    private async void OnCancelProjectClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync();
    }
}