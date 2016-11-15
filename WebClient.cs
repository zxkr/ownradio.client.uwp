using OwnRadio.Client.UWP.Model;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace OwnRadio.Client.UWP
{
    /// <summary>
    /// Class for interaction with WebAPI service
    /// </summary>
    public class WebClient
    {
        // WebAPI's HttpClient object
        private HttpClient _client = null;

        public WebClient()
        {
            _client = new HttpClient();
        }

        /// <summary>
        /// Gets next track Id
        /// </summary>
        /// <returns>Track info</returns>
        public async Task<Track> GetNextTrackAsync()
        {
            var resp = await _client.GetAsync(App.ServiceUri + "track/GetNextTrackID/" + App.DeviceId);
            Track track = null;
            if (resp.IsSuccessStatusCode)
            {
                track = new Track();
                track.Id = (await resp.Content.ReadAsStringAsync()).Replace("\"", "");
                track.Uri = new Uri(App.ServiceUri + "track/GetTrackByID/" + track.Id);
            }
            else
                throw new HttpRequestException("Failed to get next track ID");

            return track;
        }

        /// <summary>
        /// Send track's listen status
        /// </summary>
        /// <param name="trackId">Listened trackId</param>
        /// <param name="isListen">"True" - track have been listened, "False" - skipped</param>
        public async Task SetStatusTrackAsync(string trackId, bool isListen)
        {
            var date = DateTime.Now.ToString("dd.MM.yyyy H:mm");
            var listen = isListen ? "1" : "-1";
            var resp = await _client.GetAsync(App.ServiceUri + "track/SetStatusTrack/" +
                App.DeviceId + "," + trackId + "," + listen + "," + date);
            if (!resp.IsSuccessStatusCode)
                throw new HttpRequestException("Failed to send track status");
        }
    }
}
