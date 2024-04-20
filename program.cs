using RestSharp;
using System.Security.Cryptography;

// Replace with your actual values
string apiKey = "YOUR_API_KEY";
string apiSecret = "YOUR_API_SECRET";
string baseUrl = "https://bpay.binanceapi.com/binancepay/openapi/v3/order";

// Order data object
var orderData = new
{
    env = new
    {
        terminalType = "WEB"
    },
    merchantTradeNo = DateTime.Now.ToString("yyyyMMddHHmmssfff"),
    orderAmount = 20,
    currency = "USDT",
    description = "cmd500",
    goodsDetails = new[]
    {
        new
        {
            goodsType = "02",
            goodsCategory = "F000",
            referenceGoodsId = "cmd520",
            goodsName = "cmd501",
            goodsDetail = "cmd502"
        }
    },
    returnUrl = "https://www.cmd5.org",
    webhookUrl = "https://www.cmd5.org"
};
string body=System.Text.Json.JsonSerializer.Serialize(orderData);

// Timestamp in milliseconds
long timestamp = GetCurrentTimestampInMilliseconds();
string nonce = "da0cb659d3ea1a098414bcc288d8456c";
String payload = timestamp + "\n" + nonce + "\n" + body + "\n";
string signature = GenerateSignature(payload, apiSecret);

// Create the REST request
var client = new RestClient(baseUrl);
var request = new RestRequest(baseUrl, Method.Post);//POST

// Add headers (refer to Binance Pay API documentation for specific headers)
request.AddHeader("Content-Type", "application/json");
request.AddHeader("BinancePay-Timestamp", timestamp.ToString());
request.AddHeader("BinancePay-Nonce", nonce);
request.AddHeader("BinancePay-Certificate-SN", apiKey);
request.AddHeader("BinancePay-Signature", signature);

request.AddBody(body);


// Execute the request and handle the response
var response = await client.ExecuteAsync<dynamic>(request);

if (response.IsSuccessful)
{
    // Order creation successful, handle the response data (refer to API documentation)
    Console.WriteLine("Order created successfully!");
}
else
{
    // Handle errors based on the response status code and error message
    Console.WriteLine($"Error creating order: {response.StatusCode} - {response.Content}");
}

// Helper function to get current timestamp in milliseconds
long GetCurrentTimestampInMilliseconds()
{
    return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
}

string GenerateSignature(string data, string secret)
{
    byte[] keyBytes = System.Text.Encoding.UTF8.GetBytes(secret);
    byte[] messageBytes = System.Text.Encoding.UTF8.GetBytes(data);
    HMACSHA512 hmacsha512 = new HMACSHA512(keyBytes);
    byte[] bytes = hmacsha512.ComputeHash(messageBytes);

    return BitConverter.ToString(bytes).Replace("-", "").ToUpper();
}
