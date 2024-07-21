namespace Weather.UI.Data;

public class WeatherForecastService(HttpClient httpClient)
{
    public async Task<WeatherForecast[]> GetForecastAsync(DateTime startDate)
    {
        var response = await httpClient.GetFromJsonAsync<WeatherForecast[]>(
            $"/weatherforecast?startDate={startDate:yyyy-MM-dd}"
        );

        if (response == null)
        {
            return [];
        }

        return response;
    }
}
