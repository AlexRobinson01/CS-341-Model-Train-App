namespace ModelTrain.Screens
{
    /*
     * This class is the background functionality/methods for the properties page.
     * Author: Krystal Schneider
     * Date: October 16, 2024
     */
    public partial class PropertiesScreen : ContentPage
    {

        public PropertiesScreen()
        {
            InitializeComponent();

            //Creating temporary dummy data. Will pull from real project when finished
            PersonalProject DummyVariable = new PersonalProject { ProjectName = "Project 1", LastEditor = "John Doe", DateModified = "1/1/1999", Size = "24kb" };

            sizeLabel.Text = DummyVariable.Size;
            lastEditedLabel.Text = DummyVariable.LastEditor;
            lastEditedByLabel.Text = DummyVariable.DateModified;
        }

        private async void OnShareButtonClicked(object sender, EventArgs e)
        {
            // TODO: Add sharing features
        }
    }
}