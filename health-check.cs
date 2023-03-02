var parameters = new Dictionary<string, string>();

try
{
    string name = null;

    foreach (var arg in args)
    {
        if (arg.StartsWith("--"))
        {
                
            if (name != null)
                throw new Exception($"no value provided for {name}");
                
            name = arg[2..];
        }
        else if (name != null)
        {
            parameters[name] = arg;
            name = null;
        }
        else
            throw new Exception($"orphan value {arg}");
    }
        
    if (name != null)
        throw new Exception($"no value provided for {name}");
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    Environment.Exit(-1);
    return;
}

string GetParameter(string name, string def = null)
{
    if (parameters.ContainsKey(name))
        return parameters[name];
    
    if (def != null)
        return def;
    
    Console.Error.WriteLine($"{name} not defined");
    Environment.Exit(-1);
    return null;
}

var url = GetParameter("url");
var maxRetries = uint.Parse(GetParameter("retries", "10"));
var retries = maxRetries;
var wait = uint.Parse(GetParameter("wait", "2"));
var timeout = uint.Parse(GetParameter("timeout", "5"));


var client = new HttpClient();
client.Timeout = TimeSpan.FromSeconds(timeout);
var lastResponse = "";

while (retries > 0)
{
    try
    {
        Console.WriteLine($"[{maxRetries - retries + 1,2}/{maxRetries}] Trying {url}");
        var resp = await client.GetAsync(url);
        if (!resp.IsSuccessStatusCode)
        {
            Console.WriteLine($"Response: {resp.StatusCode}");
            if (retries > 1)
            {
                Console.WriteLine($"Sleeping for {wait}s...");
                Thread.Sleep(TimeSpan.FromSeconds(wait));
            }

            try
            {
                lastResponse = await resp.Content.ReadAsStringAsync();
            }
            catch { }
        }
        else
        {
            Console.WriteLine("Response OK");
            Environment.Exit(0);
        }
    }
    catch (Exception e)
    {
        Console.WriteLine($"Error: {e.Message}");

        if (retries > 1)
        {
            Console.WriteLine($"Sleeping for {wait}s...");
            Thread.Sleep(TimeSpan.FromSeconds(wait));
        }

    }
    --retries;
}
Console.Error.WriteLine($"Failed to get a successful result from {url}");
Console.Error.WriteLine("Last response:");
Console.Error.WriteLine();
Console.Error.WriteLine(lastResponse);

Environment.Exit(-1);