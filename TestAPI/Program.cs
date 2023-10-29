using ShareInvest.Naver;

using (var papago = new Papago("YOUR-CLIENT-ID", "YOUR-CLIENT-SECRET"))
{
    var response = await papago.TranslateAsync("sentence to be translate");

    Console.WriteLine(response.Value.Result.TranslatedText);
}