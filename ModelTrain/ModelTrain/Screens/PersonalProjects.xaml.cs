namespace ModelTrain.Screens;

public partial class PersonalProjects : ContentPage
{
	public PersonalProjects()
	{
		InitializeComponent();

		collectionView.ItemsSource = new List<PersonalProject>
		{
		new PersonalProject { ProjectName = "Project 1", DateCreated = DateTime.Now.ToString("dd/MM/yyyy"), ProjectID = "0001"},
        new PersonalProject { ProjectName = "Project 2", DateCreated = DateTime.Now.ToString("dd/MM/yyyy"), ProjectID = "0002"},
        new PersonalProject { ProjectName = "Project 3", DateCreated = DateTime.Now.ToString("dd/MM/yyyy"), ProjectID = "0003"},
        new PersonalProject { ProjectName = "Project 4", DateCreated = DateTime.Now.ToString("dd/MM/yyyy"), ProjectID = "0004"}
        };
	}

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        // Login logic
    }

    private async void OnConfigureButtonClicked(object sender, EventArgs e)
    {
        // Login logic
    }
}