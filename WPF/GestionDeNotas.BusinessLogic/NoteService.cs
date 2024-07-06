using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GestionDeNotas.BusinessLogic
{
    public interface INoteService
    {
        /// <summary>
        /// return False if the service is not available
        /// </summary>
        /// <returns></returns>
        Task<bool> CheckServiceAvailable();

        /// <summary>
        /// Return the Saved Noted in case of success, including empty string input (empty input will be the same as Deletion)
        /// Return null in case of failure by any reasion.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        Task<string?> SaveNoteAsync(string note);

        /// <summary>
        /// Return the Saved Noted in case of success (return empty string if there is no saved note in server)
        /// Return null in case of failure by any reasion.
        /// </summary>
        /// <returns></returns>
        Task<string?> ReadNoteAsync();

        /// <summary>
        /// Return True in case of success. 
        /// Return False in case of failure by any reasion.
        /// </summary>
        /// <returns></returns>
        Task<bool> DeleteNoteAsync();


        /// <summary>
        /// Return the Saved Noted in case of success.
        /// Doesn't do anything and return Empty String in case of empty input.
        /// Return null in case of failure by any reasion.
        /// </summary>
        /// <param name="note"></param>
        /// <returns></returns>
        Task<string?> AppendNoteAsync(string note);

        /// <summary>
        /// Return the last Saved Noted in server in case of success.
        /// Return Empty String if there is no saved note in server. 
        /// Return null in case of failure by any reasion.
        /// </summary>
        /// <returns></returns>
        Task<string[]?> ReadNotesAsync();

        /// <summary>
        /// Return True in case of success. 
        /// Return False in case of failure by any reasion.
        /// </summary>
        /// <returns></returns>
        Task<bool> ClearNotesAsync();
    }

    public class NoteServiceWrapper : INoteService
    {
        // https://learn.microsoft.com/en-us/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client
        // https://github.com/dotnet/AspNetDocs/tree/main/aspnet/web-api/overview/advanced/calling-a-web-api-from-a-net-client/sample
        // https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient

        private string _baseURL; // = "http://localhost:3000/";

        public NoteServiceWrapper(string baseUrl)
        {
            this._baseURL = baseUrl;
        }

        private HttpClient? client = null;
        private HttpClient getHttpClient()
        {
            if (client == null)
            {
                client = new HttpClient();

                //client.BaseAddress = new Uri("http://localhost:3000/");
                client.BaseAddress = new Uri(this._baseURL);

                //client.DefaultRequestHeaders.Accept.Clear();
                //client.DefaultRequestHeaders.Accept.Add(
                //    new MediaTypeWithQualityHeaderValue("application/json"));
            }
            return client;
        }

        public async Task<bool> CheckServiceAvailable()
        {
            HttpClient client = getHttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync("");
                if ((response.StatusCode == HttpStatusCode.OK))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // TODO : log error details
                return false;
            }
        }

        #region Obligatorio
        public async Task<string?> SaveNoteAsync(string note)
        {
            HttpClient client = getHttpClient();
            Dictionary<string, string> parameters = new Dictionary<string, string> { { "note", note } };
            var encodedContent = new FormUrlEncodedContent(parameters);
            try
            {
                HttpResponseMessage response = await client.PostAsync($"note", encodedContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    // service will return the Saved Note ==> shoud be the same as input
                    string? savedNote = await response.Content.ReadAsStringAsync();
                    return savedNote;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // TODO : log error details
                return null;
            }

        }

        public async Task<string?> ReadNoteAsync()
        {
            HttpClient client = getHttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync($"note");
                string note = await response.Content.ReadAsStringAsync();
                return note;
            }
            catch (Exception ex)
            {
                // TODO : log error details
                return null;
            }
        }

        public async Task<bool> DeleteNoteAsync()
        {
            HttpClient client = getHttpClient();
            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"note");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // TODO : log error details
                return false;
            }
        }
        #endregion

        #region Opcional
        public async Task<string?> AppendNoteAsync(string note)
        {
            /*if (note.Trim().Length > 0)
            {*/
            HttpClient client = getHttpClient();

            Dictionary<string, string> parameters = new Dictionary<string, string> { { "note", note } };
            var encodedContent = new FormUrlEncodedContent(parameters);

            try
            {
                HttpResponseMessage response = await client.PostAsync($"notes", encodedContent);
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    /*
                    string? savedNote = null;
                    string content = await response.Content.ReadAsStringAsync();
                    Dictionary<string, string>? dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                    if (dict != null)
                    {
                        if (dict.ContainsKey("lastNote"))
                        {
                            savedNote = dict["lastNote"];
                        }
                    }
                    */

                    // service will return the Saved Note ==> shoud be the same as input
                    string savedNote = await response.Content.ReadAsStringAsync();
                    return savedNote;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // TODO : log error details
                return null;
            }
            /*}
            else
            {
                return string.Empty;
            }*/
        }

        /*public async Task<string?> ReadLastNoteAsync()
        {
            HttpClient client = getHttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync($"notes");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    string note = await response.Content.ReadAsStringAsync();
                    return note;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // TODO : log error details
                return null;
            }
        }*/

        public async Task<string[]?> ReadNotesAsync()
        {
            //throw new NotImplementedException();

            HttpClient client = getHttpClient();
            try
            {
                HttpResponseMessage response = await client.GetAsync($"notes");
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    //string? savedNote = null;

                    string content = await response.Content.ReadAsStringAsync();

                    string[]? notes = JsonConvert.DeserializeObject<string[]>(content);

                    //Dictionary<string, string>? dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                    //Dictionary<string, string>? dict = JsonConvert.DeserializeObject<Dictionary<string, string>>(content);

                    /*if (dict != null)
                    {
                        if (dict.ContainsKey("lastNote"))
                        {
                            savedNote = dict["lastNote"];
                        }
                    }*/



                    //string note = await response.Content.Rea .ReadAsStringAsync();
                    //return note;

                    //string[] notes = new string[] { };
                    return notes;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                // TODO : log error details
                return null;
            }

        }

        public async Task<bool> ClearNotesAsync()
        {
            /*// Update port # in the following line.
            client.BaseAddress = new Uri("http://localhost:64195/");
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(
                new MediaTypeWithQualityHeaderValue("application/json"));*/

            HttpClient client = getHttpClient();
            try
            {
                HttpResponseMessage response = await client.DeleteAsync($"notes");
                //var content = response.Content;

                // the service will return the size of the note list ==> Should be 0
                int count = int.Parse(await response.Content.ReadAsStringAsync());

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                // TODO : log error details
                return false;
            }
        }

        #endregion

    }
}
