using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;


namespace ApiTests;

public class IntegrationTest : IAsyncLifetime
{
    private HttpClient? _httpClient;

    protected HttpClient HttpClient
    {
        get
        {
            if (_httpClient == default)
            {
                _httpClient = new HttpClient
                {
                    //task: update your port if necessary - check
                    BaseAddress = new Uri("http://localhost:5124")
                };
                _httpClient.DefaultRequestHeaders.Add("accept", "text/plain");
            }

            return _httpClient;
        }
    }

    /// <summary>
    /// Resetting the database between runs, ideally this would be handled a different way
    /// </summary>
    /// <returns></returns>
    public async Task InitializeAsync()
    {
        await HttpClient.GetAsync("/api/v1/test");
    }

    public async Task DisposeAsync()
    {
        HttpClient.Dispose();
    }
}

