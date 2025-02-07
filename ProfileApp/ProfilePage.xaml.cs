

using System.IO;
using System.Text.Json;
using Microsoft.Maui;
using Microsoft.Maui.Controls;
using ProfileApp.Model;
using Microsoft.Maui.Storage;



namespace ProfileApp
{
    public partial class ProfilePage : ContentPage
    {
        private const string FileName = "profile.json";
        private string _filePath;

        public ProfilePage()
        {
            InitializeComponent();
            _filePath = Path.Combine(FileSystem.AppDataDirectory, FileName);
            LoadProfile();
        }

        // Load profile from file and pre-populate the form
        private void LoadProfile()
        {
            if (File.Exists(_filePath))
            {
                var json = File.ReadAllText(_filePath);
                var profile = JsonSerializer.Deserialize<Profile>(json);
                if (profile != null)
                {
                    NameEntry.Text = profile.Name;
                    SurnameEntry.Text = profile.Surname;
                    EmailEntry.Text = profile.Email;
                    BioEditor.Text = profile.Bio;
                    if (!string.IsNullOrEmpty(profile.ImagePath) && File.Exists(profile.ImagePath))
                    {
                        ProfileImage.Source = ImageSource.FromFile(profile.ImagePath);
                    }
                }
            }
        }

        // Save profile to file
        private void SaveProfile()
        {
            var profile = new Profile
            {
                Name = NameEntry.Text,
                Surname = SurnameEntry.Text,
                Email = EmailEntry.Text,
                Bio = BioEditor.Text
            };

            var json = JsonSerializer.Serialize(profile);
            File.WriteAllText(_filePath, json);
        }

        // Button click handler for saving the profile
        private void OnSaveButtonClicked(object sender, EventArgs e)
        {
            SaveProfile();
            DisplayAlert("Success", "Profile saved successfully!", "OK");
        }
        // Pick an image from the gallery
        private async void OnPickImageClicked(object sender, EventArgs e)
        {
            try
            {
                var result = await MediaPicker.PickPhotoAsync(new MediaPickerOptions
                {
                    Title = "Pick a profile picture"
                });

                if (result != null)
                {
                    // Save the image locally
                    var filePath = Path.Combine(FileSystem.AppDataDirectory, result.FileName);
                    using (var stream = await result.OpenReadAsync())
                    using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        await stream.CopyToAsync(fileStream);
                    }

                    // Set the image source to display it
                    ProfileImage.Source = ImageSource.FromFile(filePath);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"An error occurred: {ex.Message}", "OK");
            }
        }

        // Helper to get the image file path
        private string GetImagePath()
        {
            return ProfileImage.Source is FileImageSource fileImageSource ? fileImageSource.File : null;
        }
    }
}

    
