# health-check

Simple dotnet app to check healthyness of a http endpoint

## Usage

`dotnet run --url https://url`

Optionally:
- `--retries 10` times to try
- `--wait 2` wait time between tries in seconds
- `--timeout 5` request timeout in seconds