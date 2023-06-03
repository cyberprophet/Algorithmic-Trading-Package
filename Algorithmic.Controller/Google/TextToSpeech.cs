using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;

using System.Text;

namespace ShareInvest.Google;

public class TextToSpeech
{
    public async Task<Stream> SynthesizeSpeechAsync(Stream stream, string text)
    {
        VoiceSelection.Name = await SelectVoice();

        var input = new SynthesisInput
        {
            Text = text,
        };
        var res = await tts.SynthesizeSpeechAsync(input, VoiceSelection, config);

        res.AudioContent.WriteTo(stream);

        stream.Position = 0;

        if (LanguageCodes.Korean.SouthKorea != VoiceSelection.LanguageCode)
        {
            Voices = null;
        }
        return stream;
    }
    public VoiceSelectionParams VoiceSelection
    {
        get;
    }
    public TextToSpeech(byte[] json)
    {
        VoiceSelection = new VoiceSelectionParams
        {
            LanguageCode = LanguageCodes.English.UnitedStates,
            SsmlGender = SsmlVoiceGender.Male
        };
        config = new AudioConfig
        {
            AudioEncoding = AudioEncoding.Linear16
        };
        tts = new TextToSpeechClientBuilder
        {
            JsonCredentials = Encoding.UTF8.GetString(json)
        }
        .Build();
    }
    async Task<string> SelectVoice()
    {
        if (Voices != null && Voices.Any(Predicate))
        {
            var voices = Voices.Where(Predicate).ToArray();

            return voices[DateTime.Now.Second % voices.Length].Name;
        }
        await GetVoicesAsync();

        return await SelectVoice();
    }
    async Task GetVoicesAsync()
    {
        var voices = (await tts.ListVoicesAsync(VoiceSelection.LanguageCode)).Voices;

        if (Voices != null)
        {
            Voices = Voices.Union(voices);
        }
        else
        {
            Voices = voices;
        }
    }
    bool Predicate(Voice p) =>

        VoiceSelection.SsmlGender == p.SsmlGender && p.LanguageCodes.Any(VoiceSelection.LanguageCode.Equals);

    IEnumerable<Voice>? Voices
    {
        get; set;
    }
    readonly AudioConfig config;
    readonly TextToSpeechClient tts;
}