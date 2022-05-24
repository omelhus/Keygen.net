namespace license;

public class Attributes
{
    public string Name { get; set; }
    public string Code { get; set; }
    public string Key { get; set; }
    public DateTime? Expiry { get; set; }
    public string Status { get; set; }
    public int? Uses { get; set; }
    public bool Protected { get; set; }
    public bool Suspended { get; set; }
    public string Scheme { get; set; }
    public bool encrypted { get; set; }
    public bool floating { get; set; }
    public bool concurrent { get; set; }
    public bool strict { get; set; }
    public int? maxMachines { get; set; }
    public int? maxCores { get; set; }
    public int? maxUses { get; set; }
    public bool requireHeartbeat { get; set; }
    public bool requireCheckIn { get; set; }
    public DateTime? lastValidated { get; set; }
    public DateTime? lastCheckIn { get; set; }
    public DateTime? nextCheckIn { get; set; }
    public Dictionary<string, object> metadata { get; set; }
    public DateTime? created { get; set; }
    public DateTime? updated { get; set; }
}
