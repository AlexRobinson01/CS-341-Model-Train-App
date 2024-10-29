using ModelTrain.Model;
using ModelTrain.Model.Track;
namespace ModelTrain.Screens;

/*
 * This class is the background functionality/methods for the properties page.
 * Author: Krystal Schneider
 * Date: October 16, 2024
 */
public partial class PropertiesScreen : ContentPage
{
    private readonly IBusinessLogic businessLogic;

    private readonly PersonalProject loadedProject;
    private readonly ActionHandler actionHandler;

    public PropertiesScreen(PersonalProject project)
    {
        InitializeComponent();

        businessLogic = new BusinessLogic();

        loadedProject = project;
        actionHandler = new(project.Track);
    }

    private async void OnShareButtonClicked(object sender, EventArgs e)
    {
        // Login logic
    }
}