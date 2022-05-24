namespace license
{

    public class Entitlement
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public Attributes Attributes { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
