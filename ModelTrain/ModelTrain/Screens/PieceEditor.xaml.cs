namespace ModelTrain.Screens;

/**
 * This page should be viewed in Landscape for the best experience.
 * 
 * Description: Allows a user to change the image associated with their selected piece,
 * in case they don't like the appearane of the current piece(s).
 * Author: Alex Robinson
 * Date: 10/16/2024
 */
public partial class PieceEditor : ContentPage
{
	public PieceEditor()
	{
		InitializeComponent();

		RotateCCW.Text = IconFont.Rotate_left + " CCW";
		RotateCW.Text = IconFont.Rotate_right + " CW";

		Confirm.Text = IconFont.Check + " CONFIRM";
		ChangeImage.Text = IconFont.Image + " CHANGE IMAGE";
		Cancel.Text = IconFont.Cancel + " CANCEL";
    }
}