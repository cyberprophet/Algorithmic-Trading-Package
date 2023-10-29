using Newtonsoft.Json;

using System.Runtime.Serialization;

namespace ShareInvest.Naver.Models;

public struct DetectLangage
{
    /// <summary>
    /// 1.한국어
    /// 2.일본어
    /// 3.중국어 간체
    /// 4.중국어 번체
    /// 5.힌디어
    /// 6.영어
    /// 7.스페인어
    /// 8.프랑스어
    /// 9.독일어
    /// 10.포르투갈어
    /// 11.베트남어
    /// 12.인도네시아어
    /// 13.페르시아어
    /// 14.아랍어
    /// 15.미얀마어
    /// 16.태국어
    /// 17.러시아어
    /// 18.이탈리아어
    /// 19.알 수 없음
    /// </summary>
    [DataMember, JsonProperty("langCode")]
    public string Langage
    {
        get; set;
    }
}
public enum PapagoLangCode
{
    ko = 1,
    ja = 2,
    zhCN = 3,
    zhTW = 4,
    hi = 5,
    en = 6,
    es = 7,
    fr = 8,
    de = 9,
    pt = 10,
    vi = 11,
    id = 12,
    fa = 13,
    ar = 14,
    mm = 15,
    th = 16,
    ru = 17,
    it = 18,
    unk = 19
}