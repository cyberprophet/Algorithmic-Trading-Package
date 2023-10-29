using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Naver.Models;

public struct TranslateSyntax
{
    [DataMember, JsonProperty("@type")]
    public string Type
    {
        get; set;
    }
    [DataMember, JsonProperty("@service")]
    public string Service
    {
        get; set;
    }
    [DataMember, JsonProperty("@version")]
    public string Version
    {
        get; set;
    }
    [DataMember, JsonProperty("result")]
    public Result Result
    {
        get; set;
    }
}
public struct Result
{
    [DataMember, JsonProperty("srcLangType")]
    public string SrcLangType
    {
        get; set;
    }
    [DataMember, JsonProperty("tarLangType")]
    public string TarLangType
    {
        get; set;
    }
    [DataMember, JsonProperty("translatedText")]
    public string TranslatedText
    {
        get; set;
    }
}
public struct PapagoResponse
{
    [DataMember, JsonProperty("message")]
    public TranslateSyntax TranslateSyntax
    {
        get; set;
    }
}