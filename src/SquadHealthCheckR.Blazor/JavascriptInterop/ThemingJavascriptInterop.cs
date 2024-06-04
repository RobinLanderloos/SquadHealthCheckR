using Microsoft.JSInterop;

namespace SquadHealthCheckR.JavascriptInterop;

public class ThemingJavascriptInterop
{
    private readonly IJSRuntime _jsRuntime;

    public ThemingJavascriptInterop(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public async Task SetTheme(string theme)
    {
        await _jsRuntime.InvokeVoidAsync("setTheme", theme);
    }

    public async Task<string> GetTheme()
    {
        return await _jsRuntime.InvokeAsync<string>("getTheme");
    }

}