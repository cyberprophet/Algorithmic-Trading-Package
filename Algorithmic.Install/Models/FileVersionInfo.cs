using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShareInvest.Models;

class FileVersionInfo
{
    [NotMapped]
    public byte[]? File
    {
        get; set;
    }
    [StringLength(0x80), Key]
    public string? App
    {
        get; set;
    }
    [StringLength(0x80)]
    public string? CompanyName
    {
        get; set;
    }
    public int FileBuildPart
    {
        get; set;
    }
    [StringLength(0x80)]
    public string? FileDescription
    {
        get; set;
    }
    public int FileMajorPart
    {
        get; set;
    }
    public int FileMinorPart
    {
        get; set;
    }
    [StringLength(0x80), Key]
    public string? Path
    {
        get; set;
    }
    [StringLength(0x80), Key]
    public string? FileName
    {
        get; set;
    }
    public int FilePrivatePart
    {
        get; set;
    }
    [StringLength(0x80)]
    public string? FileVersion
    {
        get; set;
    }
    public long Publish
    {
        get; set;
    }
    [StringLength(0x80)]
    public string? InternalName
    {
        get; set;
    }
    [StringLength(0x80)]
    public string? OriginalFileName
    {
        get; set;
    }
    [StringLength(0x80)]
    public string? PrivateBuild
    {
        get; set;
    }
    public int ProductBuildPart
    {
        get; set;
    }
    public int ProductMajorPart
    {
        get; set;
    }
    public int ProductMinorPart
    {
        get; set;
    }
    [StringLength(0x80)]
    public string? ProductName
    {
        get; set;
    }
    public int ProductPrivatePart
    {
        get; set;
    }
    [StringLength(0x80)]
    public string? ProductVersion
    {
        get; set;
    }
}