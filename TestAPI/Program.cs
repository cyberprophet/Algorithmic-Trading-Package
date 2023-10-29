using ShareInvest.Naver;

const string sentence = "번역할 문장을 넣어주세요~";

using (var papago = new Papago("YOUR-CLIENT-ID", "YOUR-CLIENT-SECRET"))
{
    var langCode = await papago.DetectLanguage(sentence);

    var response = await papago.TranslateAsync(langCode, sentence);

    Console.WriteLine(response.Value.Result.TranslatedText);
}