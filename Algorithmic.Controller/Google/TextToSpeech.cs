using Google.Cloud.Speech.V1;
using Google.Cloud.TextToSpeech.V1;

using System.Text;

namespace ShareInvest.Google;

public class TextToSpeech
{
    public async Task<Stream> SynthesizeSpeechAsync(Stream stream, string text)
    {
        foreach (var voice in (await tts.ListVoicesAsync(VoiceSelection.LanguageCode)).Voices)
        {
            if (voice.SsmlGender == VoiceSelection.SsmlGender)
            {
                VoiceSelection.Name = voice.Name;

                if (stream.Length % 3 == DateTime.Now.Ticks % 3)
                {
                    break;
                }
            }
        }
        var input = new SynthesisInput
        {
            Text = text,
        };
        var res = await tts.SynthesizeSpeechAsync(input, VoiceSelection, config);

        res.AudioContent.WriteTo(stream);

        stream.Position = 0;

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
            LanguageCode = LanguageCodes.English.UnitedStates
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
    readonly AudioConfig config;
    readonly TextToSpeechClient tts;
}