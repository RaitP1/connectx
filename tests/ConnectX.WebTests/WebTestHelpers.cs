using System.Text.RegularExpressions;

namespace ConnectX.WebTests;

internal static class WebTestHelpers
{
    internal static async Task<string> ExtractAntiForgeryToken(HttpResponseMessage response)
    {
        var html = await response.Content.ReadAsStringAsync();
        var match = Regex.Match(
            html, @"name=""__RequestVerificationToken""\s+type=""hidden""\s+value=""([^""]+)""");
        if (!match.Success)
        {
            match = Regex.Match(
                html, @"value=""([^""]+)""\s+name=""__RequestVerificationToken""");
        }

        return match.Success ? match.Groups[1].Value : "";
    }

    internal static string ExtractQueryParam(string url, string param)
    {
        var uri = new Uri("http://localhost" + url);
        var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
        return query[param] ?? "";
    }
}
