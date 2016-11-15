using OwnRadio.Client.UWP.Model;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace OwnRadio.Client.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private DispatcherTimer _timer;
        private Track CurrentTrack { get; set; }
        private bool IsPlaying { get; set; }

        public MainPage()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(1);
            _timer.Tick += Timer_Tick;
            this.InitializeComponent();
        }

        #region MainPage Events
        /// <summary>
        /// Get first track
        /// </summary>
        private async void Page_Loaded(object sender, RoutedEventArgs e)
        {
            await GetNextTrackAsync();
        }

        /// <summary>
        /// Update Progress bar end Elapsed time
        /// </summary>
        private void Timer_Tick(object sender, object e)
        {
            var ts = Player.Position;
            PlayingProgress.Value = ts.TotalSeconds;
            string fmt = ts.Days >= 1 ? "d'.'h':'m':'ss" : ts.Hours >= 1 ? "h':'m':'ss" : "m':'ss";
            Elapsed.Text = Player.Position.ToString(fmt);
        }
        #endregion

        #region WebClient
        /// <summary>
        /// Get track from server. 
        /// </summary>
        private async Task GetNextTrackAsync()
        {
            ReadyProgress.IsActive = true;
            ReadyProgress.Visibility = Visibility.Visible;

            while(true)
            try
            {
                CurrentTrack = await App.WebClient.GetNextTrackAsync();
                break;
            }
            catch (Exception ex)
            {
                // Shows dialog
                var dialog = new MessageDialog(ex.Message);
                dialog.Commands.Add(new UICommand { Label = "Try again", Id = 0 });
                    dialog.Commands.Add(new UICommand { Label = "Close", Id = 1 });
                var res = await dialog.ShowAsync();
                    if ((int)res.Id == 0)
                        continue;
                    if ((int)res.Id == 1)
                        Application.Current.Exit();
            }

            ReadyProgress.IsActive = false;
            ReadyProgress.Visibility = Visibility.Collapsed;

            Player.Source = CurrentTrack.Uri;
            TrackId.Text = CurrentTrack.Id;
        }

        /// <summary>
        /// Send status to server
        /// </summary>
        /// <param name="isListen">"True" - track have been listened, "False" - skipped</param>
        private async Task NextTrackAsync(bool isListen)
        {
            DownloadProgress.Value = 0;
            try
            {
                await App.WebClient.SetStatusTrackAsync(CurrentTrack.Id, isListen);
            }
            catch
            {}
            
            await GetNextTrackAsync();
        }
        #endregion

        #region Player Events
        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (IsPlaying)
                Player.Play();

            PlayingProgress.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
            PlayingProgress.Value = 0;
        }

        private void Player_DownloadProgressChanged(object sender, RoutedEventArgs e)
        {
            DownloadProgress.Value = Player.DownloadProgress;
        }

        private async void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            await NextTrackAsync(true);
        }
        #endregion

        #region Button Handler
        private void Play_Click(object sender, RoutedEventArgs e)
        {
            PlayButton.Visibility = Visibility.Collapsed;
            PauseButton.Visibility = Visibility.Visible;

            Player.Play();
            _timer.Start();

            IsPlaying = true;
        }

        private void Pause_Click(object sender, RoutedEventArgs e)
        {
            PauseButton.Visibility = Visibility.Collapsed;
            PlayButton.Visibility = Visibility.Visible;

            _timer.Stop();
            Player.Pause();

            IsPlaying = false;
        }

        private async void Next_Click(object sender, RoutedEventArgs e)
        {
            await NextTrackAsync(false);
        }

        private void Mute_Click(object sender, RoutedEventArgs e)
        {
            Player.IsMuted = true;
            MuteButton.Visibility = Visibility.Collapsed;
            UnmuteButton.Visibility = Visibility.Visible;
        }

        private void Unmute_Click(object sender, RoutedEventArgs e)
        {
            Player.IsMuted = false;
            UnmuteButton.Visibility = Visibility.Collapsed;
            MuteButton.Visibility = Visibility.Visible;
        }
        #endregion
    }
}
