using System;
using System.Net;
using System.Net.Http;
using BackloggdImporter.Models.Settings;

namespace BackloggdImporter.Factories;

internal static class HttpClientFactory
{
    private static readonly Uri BaseUrl = new("https://backloggd.com");

    public static HttpClient Create(Config config)
    {
        var http = new HttpClient(new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
            UseCookies = false,
        });

        http.BaseAddress = BaseUrl;

        http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        http.DefaultRequestHeaders.Referrer = BaseUrl;
        http.DefaultRequestHeaders.Add("Origin", BaseUrl.OriginalString);
        http.DefaultRequestHeaders.Add("Cookie", $"_backloggd_session={config.SessionCookie}");
        http.DefaultRequestHeaders.Add("Accept", "*/*");
        http.DefaultRequestHeaders.Add("X-Requested-With", "XMLHttpRequest");
        http.DefaultRequestHeaders.Add("X-Csrf-token", config.CsrfToken);

        return http;
    }
}